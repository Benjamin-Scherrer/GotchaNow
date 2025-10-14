using System;
using UnityEngine;

namespace GotchaNow
{
	public class MainCanvasEvents : MonoBehaviour
	{
		public Action rectTransformChanged;

		private void OnRectTransformDimensionsChange()
		{
			Debug.Log("Canvas RectTransform changed");
			rectTransformChanged?.Invoke();
		}
	}
}
