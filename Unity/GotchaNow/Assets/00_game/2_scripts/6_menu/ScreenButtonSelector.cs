using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

namespace GotchaNow
{
	public class ScreenButtonSelector : MonoBehaviour
	{
		private enum ScrollDirection { Up, Down }
				
		[Header("Input Actions")]
		[SerializeField] private InputActionReference up;
		[SerializeField] private InputActionReference down;
		[SerializeField] private InputActionReference submit;

		[Header("Variables")]
		[SerializeField] private float maxScrollSpeed = 0.1f;

		[Header("References")]
		[SerializeField] private List<Button> buttonReferences;

		private int selectedButtonIndex;
		private bool upScrolling = false;
		private bool downScrolling = false;
		
		private void OnEnable()
		{
			if(selectedButtonIndex < 0 || selectedButtonIndex >= buttonReferences.Count) selectedButtonIndex = 0;
			if (buttonReferences != null && buttonReferences.Count > 0)
			{
				HighlightButton();
			}

			if (buttonReferences == null)
			{
				// throw new Exception("Button References not assigned in OptionButtonManager.");
				Debug.Log("Button References not assigned in OptionButtonManager.");
			}
		}

		private void Update()
		{
			if(!gameObject.activeInHierarchy) 
				throw new Exception("ScreenButtonSelector active but GameObject is not active in hierarchy.");
			if (buttonReferences == null || buttonReferences.Count == 0) enabled = false;

			//scrolling
			if (up.action.IsPressed() && !upScrolling)
			{
				Debug.Log("Up button pressed");
				StartCoroutine(UpScroll());
			}
			else if (down.action.IsPressed() && !downScrolling)
			{
				Debug.Log("Down button pressed");
				StartCoroutine(DownScroll());
			}

			//submit
			if (submit.action.WasPressedThisFrame())
			{
				buttonReferences[selectedButtonIndex].onClick.Invoke();
			}
		}

		//Direction
		private IEnumerator UpScroll()
		{
			float timer = 0;
			float scrollSpeed = 0.5f;

			upScrolling = true;

			ScrollRequest(ScrollDirection.Up);

			while (up.action.IsPressed())
			{
				timer += Time.unscaledDeltaTime;

				if (timer > scrollSpeed)
				{
					ScrollRequest(ScrollDirection.Up);

					scrollSpeed /= 2;
					if (scrollSpeed < maxScrollSpeed) scrollSpeed = maxScrollSpeed;

					timer = 0;
				}

				yield return null;
			}

			upScrolling = false;
		}

		private IEnumerator DownScroll()
		{
			float timer = 0;
			float scrollSpeed = 0.5f;

			downScrolling = true;

			ScrollRequest(ScrollDirection.Down);

			while (down.action.IsPressed())
			{
				timer += Time.unscaledDeltaTime;

				if (timer > scrollSpeed)
				{
					ScrollRequest(ScrollDirection.Down);

					scrollSpeed /= 2;
					if (scrollSpeed < maxScrollSpeed) scrollSpeed = maxScrollSpeed;

					timer = 0;
				}

				yield return null;
			}

			downScrolling = false;
		}

		//Scroll 
		private void ScrollRequest(ScrollDirection dir)
		{
			if (buttonReferences.Count == 1) 
			{
				HighlightButton();
				return;
			
			}
			// SanityCheck();
			Debug.Log("Scrolling " + dir.ToString());
			
			DeselectButton();

			switch (dir)
			{
				case ScrollDirection.Up:
					selectedButtonIndex -= 1;
					if (selectedButtonIndex < 0) selectedButtonIndex = buttonReferences.Count - 1;
					Debug.Log("Scrolled Up to index " + selectedButtonIndex);
					break;

				case ScrollDirection.Down:
					selectedButtonIndex += 1;
					if (selectedButtonIndex > buttonReferences.Count - 1) selectedButtonIndex = 0;
					Debug.Log("Scrolled Down to index " + selectedButtonIndex);
					break;
				default:
					throw new Exception("Invalid scroll direction");
			}

			HighlightButton();
		}
		private void HighlightButton()
		{
			// SanityCheck();

			Button button = buttonReferences[selectedButtonIndex];
			button.Select();
		}

		private void DeselectButton()
		{
			// SanityCheck();

			Button button = buttonReferences[selectedButtonIndex];
			button.OnDeselect(null);
		}

	}
}
