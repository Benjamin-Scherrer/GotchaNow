using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace GotchaNow
{
	public class DialogueEventScriptableObjectEditorScript
	{
		[MenuItem("Assets/Create/ScriptableObjects/DialogueEventScriptableObject")]
		public static void CreateDialogueEventScriptableObject()
		{
			var asset = ScriptableObject.CreateInstance<DialogueEventScriptableObject>();
			string path = AssetDatabase.GenerateUniqueAssetPath("Assets/NewDialogueEventScriptableObject.asset");
			AssetDatabase.CreateAsset(asset, path);
			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
			Selection.activeObject = asset;
		}
	}
}
#endif