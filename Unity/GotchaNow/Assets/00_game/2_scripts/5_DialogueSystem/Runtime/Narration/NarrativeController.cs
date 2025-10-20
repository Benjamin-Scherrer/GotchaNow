using System;
using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Command;
using DialogueSystem.Runtime.Interaction;
using DialogueSystem.Runtime.UI;
using DialogueSystem.Runtime.Utility;
using DialogueSystem.Utility;
// using FMOD;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;
using GotchaNow;

namespace DialogueSystem.Runtime.Narration
{
    [RequireComponent(typeof(NarrativeLoader)), RequireComponent(typeof(CommandExecutionHandler))]
    public class NarrativeController : MonoBehaviour
    {
        public static NarrativeController instance { get; private set; }
        [SerializeField] private NarrativeUI narrativeUI;
        [SerializeField] private NarrativeLoader narrativeLoader;
        [SerializeField] private CommandExecutionHandler commandExecutionHandler;
        [SerializeField] private OptionButtonManager optionButtonManager;
        
        [Header("Options")]
        [SerializeField] private bool resetNarrativeOnLoad;

        [SerializeField] private UnityEvent onNarrativeStart;
        [SerializeField] private UnityEvent onNarrativeEnd;

        [Space, Header("Default Values"), SerializeField]
        private CharacterData defaultCharacterData;
        
        private string NarrativePathID { get; set; }
    
        public bool IsChoosing { get; private set; }
        public bool IsNarrating { get; private set; }
        
        public UnityEvent OnNarrativeStart => onNarrativeStart;
        public UnityEvent OnNarrativeEnd => onNarrativeEnd;
        
        private NarrativeNode _currentNarrative;
        private Queue<DialogueMessage> _narrativeQueue;
        private Narrative _narrative;

        private const string PathSeparator = ".";

        private DialogueMonoBehaviour.DialogueEvent[] _events;


        public void BeginNarration(DialogueContainer narrativeToLoad, DialogueMonoBehaviour.DialogueEvent[] dialogueEvents)
        {
            //This is only called once, as it should.
            // Debug.Log("Begin narration");

            _events = dialogueEvents;
            _narrative = narrativeLoader.LoadNarrative(narrativeToLoad);

            if (_narrative == null)
            {
                LogHandler.Alert("Can't start narrative because the narrative was not loaded properly.");
                return;
            }

            if (resetNarrativeOnLoad)
            {
                narrativeLoader.ResetNarrative();
            }

            StartNarrative();
        }

        //PRIVATE
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                throw new Exception("Multiple instances of NarrativeController detected. There should only be one instance per scene.");
            }
            instance = this;
        }

        private void StartNarrative()
        {
            onNarrativeStart?.Invoke();
            IsNarrating = true;
        
            narrativeUI.SetUIActive(true);
            narrativeUI.InitializeUI();

            SetupNarrativeEvents();
        
            var startNode = GetStartNode();
            StartNewDialogue(startNode);
        }

        private NarrativeNode GetStartNode()
        {
            NarrativePathID = narrativeLoader.GetSavedNarrativePathID();
            return _narrative.FindStartNodeFromPath(NarrativePathID);
        }

        private void ContinueToChoiceAutomatically()
        {
            //is this being called twice?
            // Debug.Log("1234_Continue to choice automatically");
            var continueAutomatically = _narrativeQueue.Count == 0 && 
                                        (_currentNarrative.HasNextChoice() || _currentNarrative.HasChoiceAfterSimpleNode() 
                                            && !_currentNarrative.IsCheckpoint);

            if (!continueAutomatically)
            {
                return;
            }
            //Thus, this is called twice
            FindNextPath();
        }

        private void StartNewDialogue(NarrativeNode narrative)
        {
            if (narrative == null)
            {
                return;
            }
            _currentNarrative = narrative;
            _narrativeQueue = new Queue<DialogueMessage>(narrative.Dialogue);
            NextNarrative();
        }

        public void NextNarrative()
        {
            //Is this running twice?
            // Debug.Log("0_Next narrative called");

            IsChoosing = false;
            if (narrativeUI.IsMessageDisplaying())
            {
                SkipCurrentMessage();
                LogHandler.Log("Skip", LogHandler.Color.Yellow);
                return;
            }
        
            ContinueNarrative();
        }

        private void ContinueNarrative()
        {
            if (_narrativeQueue == null)
            {
                FinishDialogue();
                return;
            }
        
            if (_narrativeQueue.Count == 0)
            {
                FindNextPath();
                return;
            }

            var currentDialogueMessage = _narrativeQueue.Dequeue();
            ShowNextMessage(currentDialogueMessage);
        }

        private void SkipCurrentMessage()
        {
            //Is this running twice?
            // Debug.Log("1_Skip current message");
            narrativeUI.DisplayAllMessage();
            commandExecutionHandler.ExecuteAllCommands();
        }

        private void FindNextPath()
        {
            //Thusly this is called twice
            // Debug.Log("12345_Find next path");
            if (_currentNarrative.IsCheckpoint)
            {
                FinishAtCheckpoint();
                return;
            }
        
            if (_currentNarrative.IsSimpleNode())
            {
                StartNewDialogue(_currentNarrative.DefaultPath);
                return;
            }

            if (_currentNarrative.HasNextChoice())
            {
                
                SetupDialogueOptions();
                return;
            }

            if (!_currentNarrative.IsTipNarrativeNode())
            {
                return;
            }
            FinishDialogue();
        }

        private void SetupDialogueOptions()
        {
            //This is called twice
            // Debug.Log("123456_Setup dialogue options");
            IsChoosing = true;
            narrativeUI.DisplayOptions(_currentNarrative.Options, _currentNarrative.DisableAlreadyChosenOptions, ChooseNarrativePath);
        }
        
        private CharacterData GetCharacter(string characterName)
        {
            var character = _narrative.FindCharacter(characterName);
            return character ? character : defaultCharacterData;
        }

        private void ShowNextMessage(DialogueMessage nextDialogueMessage)
        {
            var currentSpeakerData = GetCharacter(nextDialogueMessage.CharacterName);
            
            //Gather message commands and data
            commandExecutionHandler.GatherCommandData(nextDialogueMessage, currentSpeakerData, _events);
            commandExecutionHandler.ExecuteDefaultCommands();
            
            var messageWithoutCommands = commandExecutionHandler.ParseDialogueCommands(nextDialogueMessage.Content);
            
            //Display dialogue ui
            narrativeUI.DisplayDialogueBubble(nextDialogueMessage, currentSpeakerData);
            narrativeUI.DisplayMessage(messageWithoutCommands);
        }

        private void ChooseNarrativePath(int choiceIndex)
        {
            NarrativePathID += choiceIndex.ToString();
        
            UnsetNarrativeEvents();

            _currentNarrative.Options[choiceIndex].HasAlreadyBeenChosen = true;
            var nextNarrative = _currentNarrative.Options[choiceIndex].TargetNarrative;

            if (nextNarrative != null)
            {
                SetupNarrativeEvents();
                StartNewDialogue(nextNarrative);
                return;
            }
        
            FinishDialogue();
        }

        private void SetupNarrativeEvents()
        {
            // Debug.Log("Setup narrative events");
            narrativeUI.OnMessageEnd += ContinueToChoiceAutomatically;

            //DEBUG
            // narrativeUI.LogOnMessageEndInvocations();
            //DEBUG END
        }

        private void UnsetNarrativeEvents()
        {
            // Debug.Log("Unset narrative events");
            narrativeUI.OnMessageEnd -= ContinueToChoiceAutomatically;

            //DEBUG
            // narrativeUI.LogOnMessageEndInvocations();
            //DEBUG END
        }

        private void FinishAtCheckpoint()
        {
            NarrativePathID += PathSeparator;
            FinishDialogue();
        }
        
        private void FinishDialogue()
        {
            //FIX
            // Debug.Log("Dialogue finished");
            UnsetNarrativeEvents();
            //FIX END

            narrativeUI.SetUIActive(false);
            IsNarrating = false;

            narrativeLoader.SaveNarrativePath(NarrativePathID, _currentNarrative?.IsTipNarrativeNode() ?? false);
        
            onNarrativeEnd?.Invoke();
            LogResults();
        }

        private void LogResults()
        {
            LogHandler.Log("Dialogue finished!", LogHandler.Color.Blue);
            LogHandler.Log($"Final narrative path ID: {NarrativePathID}", LogHandler.Color.Blue);
        }
    }
}
