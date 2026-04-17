using DialogueSystem.Data;
using DialogueSystem.Runtime.Narration;
using UnityEngine;

namespace GotchaNow
{
	public class DialogueSelector : MonoBehaviour
	{

		// [SerializeField] protected NarrativeController narrativeController;

		[Header("Intermission Dialogues")]
		[SerializeField] protected DialogueContainer intro;
		[SerializeField] protected DialogueContainer preBattle1;
		[SerializeField] protected DialogueContainer intermission1;
		[SerializeField] protected DialogueContainer intermission1TrueEndingPath;
		[SerializeField] protected DialogueContainer preSecretBoss;
		[SerializeField] protected DialogueContainer trueEnding;
		[SerializeField] protected DialogueContainer trueEndingFailed;
		[SerializeField] protected DialogueContainer neutralEnding;
		[SerializeField] protected DialogueContainer badEnding;
		[SerializeField] protected DialogueContainer quotaFilledAyaBattle;
		[SerializeField] protected DialogueContainer gameOver;
		[SerializeField] protected DialogueContainer gameOverAyaBattle;

		[Header("Battle Dialogues")]
		[SerializeField] protected DialogueContainer tutorial;
		[SerializeField] protected DialogueContainer battle1;
		[SerializeField] protected DialogueContainer battle2trueEndingPath;
		[SerializeField] protected DialogueContainer battle2;
		[SerializeField] protected DialogueContainer battle3;

        // private void Start()
        // {
        //     if (narrativeController == null)
        //     {
        //         narrativeController = NarrativeController.Instance;
        //     }
        // }

        public DialogueContainer GetNarrativeScriptableObject()
		{
			Debug.Log("Getting narrative scriptable object for Gameobject: " + gameObject.name);
			if(enabled == false)
			{
				// Debug.Log("DialogueSelector is disabled.");
				return null;
			}

			if (ProgressionManager.instance == null) throw new System.Exception("ProgressionManager instance is null");
			string gameState = ProgressionManager.instance.gameState;

			if (gameState == "intermission") return IntermissionDialogue();
			if (gameState == "battle") return BattleDialogue();
			throw new System.Exception("Invalid game state: " + gameState);
		}

		private DialogueContainer IntermissionDialogue()
		{
			Debug.Log("Selecting intermission dialogue for Gameobject: " + gameObject.name);
			if (ProgressionManager.instance == null) throw new System.Exception("ProgressionManager instance is null");
			string intermissionID = ProgressionManager.instance.intermissionID;

            return intermissionID switch
            {
                "intro" => intro,//default intro dialogue
                "preBattle1" => preBattle1,
                "intermission1TrueEndingPath" => intermission1TrueEndingPath,
                "intermission1" => intermission1,
                "preSecretBoss" => preSecretBoss,
                "trueEnding" => trueEnding,
                "trueEndingFailed" => trueEndingFailed,
                "neutralEnding" => neutralEnding,
                "badEnding" => badEnding,
				"quotaFilledAyaBattle" => quotaFilledAyaBattle,
                "gameOver" => gameOver,
				"gameOverAyaBattle" => gameOverAyaBattle,
                _ => throw new System.Exception("Invalid intermission ID: " + intermissionID),
            };
        }
		
		private DialogueContainer BattleDialogue()
		{
			Debug.Log("Selecting battle dialogue for Gameobject: " + gameObject.name);
			if (ProgressionManager.instance == null) throw new System.Exception("ProgressionManager instance is null");
			string battleID = ProgressionManager.instance.battleID;

            return battleID switch
            {
                "tutorial" => tutorial,//default intro dialogue
                "battle1" => battle1,
                "battle2trueEndingPath" => battle2trueEndingPath,
                "battle2" => battle2,
                "battle3" => battle3,
                _ => throw new System.Exception("Invalid battle ID: " + battleID),
            };
        }
	}
}
