using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace GotchaNow
{
    public class ChatMessageHistoryEditorScript
	{
		[MenuItem("Assets/Create/ScriptableObjects/ChatMessageHistory")]
		public static void CreateChatMessageHistory()
		{
			var asset = ScriptableObject.CreateInstance<ChatMessageHistory>();
			string path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewChatMessageHistory.asset");
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
    }
}
#endif