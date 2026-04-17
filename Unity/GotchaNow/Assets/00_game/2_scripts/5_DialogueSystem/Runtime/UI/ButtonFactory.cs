using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogueSystem.Runtime.UI
{
    public class ButtonFactory : MonoBehaviour
    {
        public static Button CreateButton(Button buttonPrefab, Button disabledButtonPrefab, Transform parent, bool isDisabled, string buttonText, UnityAction onClickAction)
        {
            var newButton = Instantiate(isDisabled ? disabledButtonPrefab : buttonPrefab, parent);
            newButton.interactable = !isDisabled;

            var optionTextContainer = newButton.transform.GetComponentInChildren<TextMeshProUGUI>();
            optionTextContainer.text = buttonText;

            newButton.onClick.AddListener(onClickAction);

            return newButton;
        }

        // public static void PlaceButton(RectTransform button, Rect parentRect, int columnIndex, int rowIndex, int numberOfButtonsInRow, Vector2 buttonOffset)
        // {
        //     var buttonRect = button.rect;

        //     var initialButtonXPosition = (buttonRect.width * (1 - numberOfButtonsInRow) - numberOfButtonsInRow * buttonOffset.x + buttonOffset.x) / 2;
        //     var initialButtonYPosition = parentRect.height / 2 - buttonRect.height / 2;

        //     var xOffset = columnIndex * (buttonRect.width + buttonOffset.x);
        //     var yOffset = rowIndex * (buttonRect.height + buttonOffset.y);

        //     button.localPosition = new Vector3(initialButtonXPosition + xOffset, initialButtonYPosition - yOffset, 0);
        // }

        public static void PlaceButton(RectTransform button, Rect parentRect, int columnIndex, int rowIndex, int numberOfButtonsInRow, Vector2 buttonOffset)
        {
            var buttonRect = button.rect;
            float buttonWidth = buttonRect.width;
            float buttonHeight = buttonRect.height;

            // Total width of all buttons and gaps in this row
            float totalRowWidth = numberOfButtonsInRow * buttonWidth + (numberOfButtonsInRow - 1) * buttonOffset.x;
            // Start X: center the entire row around 0, then move to the center position of the left-most button
            float startX = -totalRowWidth / 2 + buttonWidth / 2;

            // Start Y: top edge of parent minus half button height (assuming parent center at 0)
            float startY = parentRect.height / 2 - buttonHeight / 2;

            float xPos = startX + columnIndex * (buttonWidth + buttonOffset.x);
            float yPos = startY - rowIndex * (buttonHeight + buttonOffset.y);

            // Prefer anchoredPosition for UI RectTransforms if pivot/anchors are standard
            button.anchoredPosition = new Vector2(xPos, yPos);
        }
    }
}