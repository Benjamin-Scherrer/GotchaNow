using UnityEngine;

namespace GotchaNow
{
	public class ChatMessageSelector : MonoBehaviour
	{
		[Header("Intermission Dialogues")]
		[SerializeField] protected ChatMessageHistory intro;
		[SerializeField] protected ChatMessageHistory preBattle1;
		[SerializeField] protected ChatMessageHistory intermission1;
		[SerializeField] protected ChatMessageHistory intermission1TrueEndingPath;
		[SerializeField] protected ChatMessageHistory preSecretBoss;
		[SerializeField] protected ChatMessageHistory trueEnding;
		[SerializeField] protected ChatMessageHistory trueEndingFailed;
		[SerializeField] protected ChatMessageHistory neutralEnding;
		[SerializeField] protected ChatMessageHistory badEnding;
		[SerializeField] protected ChatMessageHistory gameOver;

		[Header("Battle Dialogues")]
		[SerializeField] protected ChatMessageHistory tutorial;
		[SerializeField] protected ChatMessageHistory battle1;
		[SerializeField] protected ChatMessageHistory battle2;
		[SerializeField] protected ChatMessageHistory battle2trueEndingPath;
		[SerializeField] protected ChatMessageHistory battle3;


        public ChatMessageHistory GetChatMessageHistory()
		{
			if(enabled == false)
			{
				Debug.Log("ChatMessageHistory is disabled.");
				return null;
			}

			if (ProgressionManager.instance == null) throw new System.Exception("ProgressionManager instance is null");
			string gameState = ProgressionManager.instance.gameState;

			if (gameState == "intermission") return IntermissionDialogue();
			if (gameState == "battle") return BattleDialogue();
			throw new System.Exception("Invalid game state: " + gameState);
		}

		private ChatMessageHistory IntermissionDialogue()
		{
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
                "gameOver" => gameOver,
                _ => throw new System.Exception("Invalid intermission ID: " + intermissionID),
            };
        }
		
		private ChatMessageHistory BattleDialogue()
		{
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
