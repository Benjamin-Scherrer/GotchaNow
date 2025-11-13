using UnityEngine;

namespace GotchaNow
{
	public class CopyRect : MonoBehaviour
	{
		[SerializeField] private RectTransform sourceRect;
		[SerializeField] private bool xScale;
		[SerializeField] private bool yScale;
		[SerializeField] private bool xPosition;
		[SerializeField] private bool yPosition;

		private void LateUpdate()
		{
			if (sourceRect == null)
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

			targetRect.anchoredPosition = sourceRect.anchoredPosition;
			Vector2 sourceScale = sourceRect.lossyScale;
			Vector2 targetScale = targetRect.lossyScale;
			if (targetScale.x == 0f || targetScale.y == 0f)
			{
				Debug.LogWarning("CopyRect detected zero scale on targetRect, skipping sizeDelta update to avoid division by zero.");
				return;
			}
			Vector2 adjustedScale = new (
				sourceScale.x /  targetScale.x,
				sourceScale.y /  targetScale.y
				);

			Vector2 sourceSizeDelta = sourceRect.sizeDelta;
			Vector2 finalSizeDelta = new(
				sourceSizeDelta.x * adjustedScale.x,
				sourceSizeDelta.y * adjustedScale.y
				);
			if (!xScale) finalSizeDelta.x = targetRect.sizeDelta.x;
			if (!yScale) finalSizeDelta.y = targetRect.sizeDelta.y;
			targetRect.sizeDelta = finalSizeDelta;
			// Debug.Log("CopyRect updated sizeDelta to: " + finalSizeDelta);

			Vector2 finalAnchoredPosition = new(
				sourceRect.anchoredPosition.x * adjustedScale.x,
				sourceRect.anchoredPosition.y * adjustedScale.y
				);
			if (!xPosition) finalAnchoredPosition.x = targetRect.anchoredPosition.x;
			if (!yPosition) finalAnchoredPosition.y = targetRect.anchoredPosition.y;
			targetRect.anchoredPosition = finalAnchoredPosition;
		}
	}
}
