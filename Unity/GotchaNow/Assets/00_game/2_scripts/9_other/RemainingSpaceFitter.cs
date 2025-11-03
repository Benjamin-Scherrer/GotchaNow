using UnityEngine;

namespace GotchaNow
{
	public class RemainingSpaceFitter : MonoBehaviour
	{
		//Rect transform to affect
		[SerializeField] private RectTransform targetRectTransform;

		//Rect transform that takes up space
		[SerializeField] private RectTransform referenceRectTransform;

		//Rect transform of the parent object
		[SerializeField] private RectTransform parentRectTransform;

		//Canvas reference for rendering event
		[SerializeField] private MainCanvasEvents mainCanvasEvents;

		private bool hasToUpdate = false;
		
		private void Start()
		{
			if (enabled == false)
            {
                throw new System.Exception("RemainingSpaceFitter is disabled.");
            }

			if (targetRectTransform == null)
			{
				throw new System.Exception("Target RectTransform is not assigned.");
			}
			if (referenceRectTransform == null)
			{
				throw new System.Exception("Reference RectTransform is not assigned.");
			}

			if (parentRectTransform == null)
			{
				throw new System.Exception("Parent RectTransform is not found.");
			}

			UpdateRectSize();
		}

        
        private void OnEnable()
		{
			 mainCanvasEvents.rectTransformChanged += SetHasToUpdate;
		}

		private void OnDisable()
		{
			mainCanvasEvents.rectTransformChanged -= SetHasToUpdate;
		}

		private void SetHasToUpdate()
		{
			hasToUpdate = true;
		}

		private void UpdateRectSize()
		{
			float referenceHeight = referenceRectTransform.rect.height;
			float referenceWidth = referenceRectTransform.rect.width;

			float parentHeight = parentRectTransform.rect.height;
			float parentWidth = parentRectTransform.rect.width;

			var newHeight = parentHeight - referenceHeight;
			var newWidth = parentWidth - referenceWidth;

			if (newHeight < 0) newHeight = 0;
			if (newWidth < 0) newWidth = 0;

			targetRectTransform.sizeDelta = new Vector2(newWidth, newHeight);
		}

		private void LateUpdate()
		{
			if (hasToUpdate)
			{
				UpdateRectSize();
				hasToUpdate = false;
			}
		}

#if UNITY_EDITOR
		//Debugging purposes
		[Header("Debugging")]
		[SerializeField] private bool updateInEditor = false;
		private void OnValidate()
		{
			if (updateInEditor)
			{
				UpdateRectSize();
				updateInEditor = false;
			}
		}
		#endif
	}
}
