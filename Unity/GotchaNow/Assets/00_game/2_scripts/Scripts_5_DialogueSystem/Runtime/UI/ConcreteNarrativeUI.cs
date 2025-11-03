using System.Collections.Generic;
using DialogueSystem.Data;
using DialogueSystem.Runtime.Command;
using DialogueSystem.Runtime.Narration;
using DialogueSystem.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GotchaNow;

namespace DialogueSystem.Runtime.UI
{
    [RequireComponent(typeof(OptionButtonManager))]
    public class ConcreteNarrativeUI : NarrativeUI
    {
        [SerializeField] private TMP_Text speakerNameText;
        [SerializeField] private TMP_Text messageTextContainer;
        
        [Space, Header("UI Buttons")] 
        [SerializeField] private Button buttonPrefab;
        [SerializeField] private Button disabledButtonPrefab;
        [SerializeField] private Transform buttonsParent;
        [SerializeField] private Button nextMessageButton;
        [SerializeField] private Vector2 buttonOffset;
        [SerializeField, Min(1)] private int numberOfColumns = 2;

        [Space, Header("UI Rendering")]
        [SerializeField] private Optional<Image> characterSprite;
        [SerializeField] private Image dialogueBubble;
    
        private List<Button> _currentOptionButtonList;
        private bool _textTyperNotNull;

        private OptionButtonManager _optionButtonManager;
    
        private void Awake()
        {
            _currentOptionButtonList = new List<Button>();
            _textTyperNotNull = textTyper != null;
            SetUIActive(false);

            _optionButtonManager = GetComponent<OptionButtonManager>();
            _optionButtonManager.enabled = false;
        }
        
        public override void DisplayOptions(List<DialogueOption> options, bool disableChosenOptions,
            ChoosePathDelegate choosePathFunction)
        {
            // Debug.Log("1234567_Options: " + options.Count);

            DisableNextNarrationUI();
            var parentRect = buttonsParent.GetComponent<RectTransform>().rect;

            //Get parent rect dimensions
            float parentWidth = parentRect.width;
            float parentHeight = parentRect.height;

            int columnIndex = 0;
            int rowIndex = 0;

            int numberOfOptionsLeft = options.Count;
            int numberOfOptionsInRow = Mathf.Min(numberOfOptionsLeft, numberOfColumns);

            //Calculate button size based on number of columns
            float buttonWidth = (parentWidth - (buttonOffset.x * (numberOfOptionsInRow - 1))) / numberOfOptionsInRow;
            int numberOfRows = Mathf.CeilToInt((float)numberOfOptionsLeft / numberOfColumns);
            float buttonHeight = (parentHeight - (buttonOffset.y * (numberOfRows - 1))) / numberOfRows;

            // Get the upper left position of the parent rect in local space
            Vector2 originPosition = new(parentRect.xMin, parentRect.yMax);

            // Debug.Log("Starting to place buttons");
            foreach (var option in options)
            {
                var isDisabledOption = disableChosenOptions && option.HasAlreadyBeenChosen;

                var newOptionButton = ButtonFactory.CreateButton(
                    buttonPrefab,
                    disabledButtonPrefab,
                    buttonsParent,
                    isDisabledOption,
                    DialogueCommandParser.ReplaceVariableTagsByValue(option.Text),
                    delegate
                    {
                        choosePathFunction(options.IndexOf(option));
                        RemoveOptions();
                        EnableNextNarrationUI();
                    });

                var buttonRect = newOptionButton.GetComponent<RectTransform>();
                buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, buttonWidth);
                buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonHeight);



                ButtonFactory.PlaceButton(buttonRect, parentRect, columnIndex, rowIndex, numberOfOptionsInRow, buttonOffset);
                // Debug.Log($"Placed button {option.Text} at column {columnIndex}, row {rowIndex}");

                columnIndex++;
                numberOfOptionsLeft--;

                _currentOptionButtonList.Add(newOptionButton);

                if (columnIndex != numberOfColumns)
                {
                    continue;
                }
                numberOfOptionsInRow = Mathf.Min(numberOfOptionsLeft, numberOfColumns);
                columnIndex = 0;
                rowIndex++;
            }

            _optionButtonManager.ButtonReferences = _currentOptionButtonList;
            _optionButtonManager.enabled = true;
            // Debug.Log("Finished placing buttons");
        }

        private void RemoveOptions()
        {
            _currentOptionButtonList.ForEach(button => Destroy(button.gameObject));
            _currentOptionButtonList.Clear();

            _optionButtonManager.enabled = false;
            _optionButtonManager.ButtonReferences = null;
        }

        private void DisplayCharacterName(string speakerName, bool hide)
        {
            speakerNameText.text = speakerName;
            speakerNameText.maxVisibleCharacters = hide ? 0 : int.MaxValue;
        }
    
        public override void DisplayDialogueBubble(DialogueMessage messageData, CharacterData characterData)
        {
            DisplayCharacterName(messageData.CharacterName, messageData.HideCharacter);
            DisplayCharacter(characterData.DefaultState.CharacterFace, messageData.HideCharacter);
        }

        public override void DisplayMessage(string text)
        {
            messageTextContainer.text = text;
            if (textTyper)
            {
                textTyper.TypeText(text, messageTextContainer);
            }
        }

        public override void DisplayCharacter(Optional<Sprite> sprite, bool hideCharacter)
        {
            if (!characterSprite.Enabled)
            {
                return;
            }

            characterSprite.Value.sprite = sprite.Value;
            characterSprite.Value.gameObject.SetActive(!hideCharacter && sprite.Enabled);
        }

        private void EnableNextNarrationUI() => nextMessageButton.gameObject.SetActive(true);
        private void DisableNextNarrationUI() => nextMessageButton.gameObject.SetActive(false);
        
        public override void SetUIActive(bool active)
        {
            speakerNameText.gameObject.SetActive(active);
            messageTextContainer.gameObject.SetActive(active);
            buttonsParent.gameObject.SetActive(active);
            nextMessageButton.gameObject.SetActive(active);
            dialogueBubble.gameObject.SetActive(active);
            characterSprite.Value.gameObject.SetActive(active);
        }
    
        public override void InitializeUI()
        {
            _currentOptionButtonList = new List<Button>();
            _textTyperNotNull = textTyper != null;
            messageTextContainer.text = string.Empty;
            speakerNameText.text = string.Empty;
            DisplayCharacter(new Optional<Sprite>(), true);
        }

        public override bool IsMessageDisplaying() => _textTyperNotNull && textTyper.IsTyping;

        public override void DisplayAllMessage()
        {
            // Debug.Log("12_Display all messages");
            textTyper.FinishTyping();
            messageTextContainer.maxVisibleCharacters = int.MaxValue;
        }
    }
}
