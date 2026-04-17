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

		// //Canvas reference for rendering event
		// [SerializeField] private MainCanvasEvents mainCanvasEvents;

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
			//  mainCanvasEvents.rectTransformChanged += SetHasToUpdate;
			GameObject referenceGameObject = referenceRectTransform.gameObject;
			if (!referenceGameObject.TryGetComponent<SpaceFitterReference>(out var spaceFitterReference)) return;

			spaceFitterReference.OnDisableEvent += SetHasToUpdate;
			spaceFitterReference.OnEnableEvent += SetHasToUpdate;
			// spaceFitterReference.OnRectTransformDimensionsChangeEvent += SetHasToUpdate;
		}

		private void OnDisable()
		{
			// mainCanvasEvents.rectTransformChanged -= SetHasToUpdate;$
			GameObject referenceGameObject = referenceRectTransform.gameObject;
			if (!referenceGameObject.TryGetComponent<SpaceFitterReference>(out var spaceFitterReference)) return;

			spaceFitterReference.OnDisableEvent -= SetHasToUpdate;
			spaceFitterReference.OnEnableEvent -= SetHasToUpdate;
			// spaceFitterReference.OnRectTransformDimensionsChangeEvent -= SetHasToUpdate;
		}

		private void SetHasToUpdate()
		{
			Debug.Log($"SetHasToUpdate called on {gameObject.name}");
			hasToUpdate = true;
		}

		private void UpdateRectSize()
		{
			Debug.Log($"UpdateRectSize called on {gameObject.name}");

			// float referenceHeight = 0;
			float referenceWidth; ;
			if (referenceRectTransform.gameObject.activeInHierarchy == true)
			{
				// referenceHeight = referenceRectTransform.rect.height;
				referenceWidth = referenceRectTransform.rect.width;
			}
			else
			{
				// referenceHeight = 0;
				referenceWidth = 0;
			}
		
			// float parentHeight = parentRectTransform.rect.height;
			float parentWidth = parentRectTransform.rect.width;

			// var newHeight = parentHeight - referenceHeight;
			var newWidth = parentWidth - referenceWidth;

			// if (newHeight < 0) newHeight = 0;
			if (newWidth < 0) newWidth = 0;

			targetRectTransform.sizeDelta = new Vector2(newWidth, 0);
			Debug.Log($"New sizeDelta for {targetRectTransform.gameObject.name}: {targetRectTransform.sizeDelta}");
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
