using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace GotchaNow
{
	[Serializable]
	public class ChatMessage : MonoBehaviour
	{
		[Header("UI References")]
		[SerializeField] private Image backgroundImage;
		[SerializeField] private TextMeshProUGUI messageSenderUI;
		[SerializeField] private Image messageSenderAvatarUI;
		[SerializeField] private TextMeshProUGUI messageTextUI;
		// [SerializeField] private RectTransform scalingRectTransform;

		[Header("Variables")]
		[SerializeField] private float messageFoldedHeight = 32f;
		[SerializeField] private float messageUnfoldedHeight = 64f;
		[SerializeField] private float imageMargin = 12f;
		[SerializeField] private float textMargin = 12f;
		[SerializeField] private float messageFoldingCoef = 1.0f;

		public bool GettingSwipedAway = false;

		public bool Written = false;

		//PUBLIC PROPERTIES
		public float getHeight => Mathf.Lerp(messageFoldedHeight, messageUnfoldedHeight, messageFoldingCoef);
		// public float getHeight => (scalingRectTransform != null) ? scalingRectTransform.rect.height : 0f;
		// public Vector2 getLossyScale => (scalingRectTransform != null) ? scalingRectTransform.lossyScale : Vector2.one;

		//PUBLIC METHODS
		public void InitializeMessage(ChatMessageData messageData)
		{
			if (messageData == null)
			{
				throw new System.ArgumentNullException("messageData", "ChatMessageData provided to InitializeMessage is null.");
			}
			if (messageData.GetSenderName == null)
			{
				throw new System.Exception("SenderName in ChatMessageData is null.");
			}
			if (messageData.MessageContent == null)
			{
				throw new System.Exception("MessageContent in ChatMessageData is null.");
			}
			if (messageData.SenderAvatar == null)
			{
				throw new System.Exception("SenderAvatar in ChatMessageData is null.");
			}

			if(backgroundImage == null)
			{
				throw new System.Exception("backgroundImage is not assigned in ChatMessage.");
			}
			if (messageSenderUI == null)
			{
				throw new System.Exception("messageSenderUI is not assigned in ChatMessage.");
			}
			if (messageSenderAvatarUI == null)
			{
				throw new System.Exception("messageSenderAvatarUI is not assigned in ChatMessage.");
			}
			if (messageTextUI == null)
			{
				throw new System.Exception("messageTextUI is not assigned in ChatMessage.");
			}
			messageSenderUI.text = messageData.GetSenderName;
			messageSenderAvatarUI.sprite = messageData.SenderAvatar;
			messageTextUI.text = messageData.MessageContent;

			Written = false;
		}

		public void SetSenderText(string newText)
		{
			if (messageSenderUI == null)
			{
				throw new System.Exception("messageSenderUI is not assigned in ChatMessage.");
			}
			messageSenderUI.text = newText;
		}

		public void SetMessageText(string newText)
		{
			if (messageTextUI == null)
			{
				throw new System.Exception("messageTextUI is not assigned in ChatMessage.");
			}
			messageTextUI.text = newText;
		}

		public void ScaleMessageRespectingFontSize(Vector3 scaleVector)
		{
			ScaleMessageRespectingFontSize(scaleVector.y);
		}

		public void ScaleMessageRespectingFontSize(float scaleFactor)
		{
			if (messageTextUI == null)
			{
				throw new System.Exception("messageTextUI is not assigned in ChatMessage.");
			}
			// scalingRectTransform.localScale = scaleFactor * Vector2.one;

			messageFoldingCoef = scaleFactor;
			float backgroundHeight = Mathf.Lerp(messageFoldedHeight, messageUnfoldedHeight, messageFoldingCoef);
			float backgroundWidth = backgroundImage.rectTransform.sizeDelta.x;

			// Set background size
			backgroundImage.rectTransform.sizeDelta = new(backgroundWidth, backgroundHeight);

			//calculate Image size and position
			float imagePosX = imageMargin;
			float imagePosY = -imageMargin;
			messageSenderAvatarUI.rectTransform.anchoredPosition = new(imagePosX, imagePosY);

			float imageHeight = Math.Max(backgroundHeight - 2 * imageMargin, 0f);
			float imageWidth = imageHeight; //keep square
			messageSenderAvatarUI.rectTransform.sizeDelta = new(imageWidth, imageHeight);

			//calculate Sender Text size and position
			float senderTextPosX = imageWidth + 2 * imageMargin;
			float senderTextPosY = -textMargin;
			messageSenderUI.rectTransform.anchoredPosition = new(senderTextPosX, senderTextPosY);

			float senderTextWidth = Mathf.Max(backgroundWidth - senderTextPosX, 0f);
			float senderTextHeight = messageSenderUI.fontSize; //keep font size
			messageSenderUI.rectTransform.sizeDelta = new(senderTextWidth, senderTextHeight);

			//calculate Message Text size and position
			float messageTextPosX = senderTextPosX;
			float messageTextPosY = senderTextPosY - senderTextHeight - textMargin;
			messageTextUI.rectTransform.anchoredPosition = new(messageTextPosX, messageTextPosY);

			float messageTextWidth = Mathf.Max(backgroundWidth - messageTextPosX - textMargin, 0f);
			float messageTextHeight = Math.Max(backgroundHeight + messageTextPosY - textMargin, 0f);
			messageTextUI.rectTransform.sizeDelta = new(messageTextWidth, messageTextHeight);
		}

		//EDITOR
		#if(UNITY_EDITOR)
		public void OnValidate()
		{
			ScaleMessageRespectingFontSize(messageFoldingCoef);
		}
		#endif
	}
}
