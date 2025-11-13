using UnityEngine;

namespace GotchaNow
{
	public class SquishToBottom : MonoBehaviour
	{
		[SerializeField] private RectTransform sourceRect;
		[SerializeField] private RectTransform containerRectTransform;
		[SerializeField] private RectTransform squisherRectTransform;
		[SerializeField] private float bottomPadding = 0f;
		private void LateUpdate()
		{
			if (containerRectTransform == null || squisherRectTransform == null)
			{
				enabled = false;
				return;
			}
			if (this == null || this.gameObject == null)
			{
				enabled = false;
				return;
			}
			RectTransform targetRect = this.transform as RectTransform;
			if (targetRect == null)
			{
				enabled = false;
				return;
			}

			//put position beneath squisher
			float squisherHeight = squisherRectTransform.rect.height;
			float squisherBottomY = squisherRectTransform.anchoredPosition.y - squisherHeight;

				// Debug.Log("SquishToBottom squisherHeight: " + squisherHeight);
				// Debug.Log("Squisher anchoredPosition.y: " + squisherRectTransform.anchoredPosition.y);
				// Debug.Log("SquishToBottom squisherBottomY: " + squisherBottomY);
			

			//X hack
			Vector2 sourceScale = sourceRect.lossyScale;
			Vector2 targetScale = targetRect.lossyScale;
			Vector2 adjustedScale = new (
				sourceScale.x /  targetScale.x,
				sourceScale.y /  targetScale.y
				);
			

			targetRect.anchoredPosition = new(
				sourceRect.anchoredPosition.x * adjustedScale.x,
				squisherBottomY - bottomPadding
				);
				

				// Debug.Log("SquishToBottom set anchoredPosition.y to: " + targetRect.anchoredPosition.y);

			//AdjustSize.delta to fit between squisher and container bottom
			Vector2 containerSizeDelta = containerRectTransform.sizeDelta;
			float containerHeight = containerSizeDelta.y;
			float finalHeight = containerHeight - squisherHeight + squisherRectTransform.anchoredPosition.y;
			// Debug.Log("SquishToBottom squisherHeight: " + squisherHeight);
			// Debug.Log("SquishToBottom containerHeight: " + containerHeight);
			// Debug.Log("SquishToBottom calculated finalHeight: " + finalHeight);
			Vector2 sourceSizeDelta = sourceRect.sizeDelta;
			Vector2 finalSizeDelta = new (
				sourceSizeDelta.x * adjustedScale.x,
				finalHeight
				);
			// Debug.Log("targetRectTransform offset Bottom: " + targetRectTransform.offsetMin.y);
			targetRect.sizeDelta = finalSizeDelta;

			// Debug.Log("CopyRect updated sizeDelta to: " + finalSizeDelta);
			// Debug.Log("targetRectTransform offset Bottom: " + targetRectTransform.offsetMin.y);

			// targetRectTransform.anchoredPosition = new(
			// 	targetRectTransform.anchoredPosition.x,
			// 	squisherBottomY - bottomPadding
			// 	);
				
				// Debug.Log("//SquishToBottom set anchoredPosition.y to: " + targetRect.anchoredPosition.y);
			
		}
	}
}
