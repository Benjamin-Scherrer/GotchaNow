using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace GotchaNow
{
	public class OptionButtonManager : MonoBehaviour, IPointerEnterHandler
	{
		private enum ScrollDirection { Up, Down }

		[Header("Input action references")]
		[SerializeField] private InputActionReference up;
		[SerializeField] private InputActionReference down;
		[SerializeField] private InputActionReference submit;

		[Header("Variables")]
		[SerializeField] private float maxScrollSpeed = 0.1f;
		
		// [SerializeField] private float openTime = 0.3f;
		private List<Button> _buttonReferences;
		private List<Button> _activeButtonReferences;
		private bool upScrolling = false;
		private bool downScrolling = false;
		private int selectedButtonIndex = 0;

		public List<Button> ButtonReferences
		{
			private get => _buttonReferences;
			set
			{
				_buttonReferences = value;
				// initialize selection when new list assigned
				if (_buttonReferences != null && _buttonReferences.Count > 0)
				{
					_activeButtonReferences.Clear();
					foreach (Button btn in _buttonReferences)
					{
						if (btn.gameObject.name == "Button_DefaultDisabled(Clone)") continue;
						_activeButtonReferences.Add(btn);
					}
					selectedButtonIndex = Mathf.Clamp(selectedButtonIndex, 0, _activeButtonReferences.Count - 1);

					HighlightButton(_activeButtonReferences[selectedButtonIndex]);
				}
			}
		}

		//PUBLIC METHODS
		public void OnPointerEnter(PointerEventData eventData)
		{
			Debug.Log("OnPointerEnter event received in OptionButtonManager.");
			if (eventData == null) throw new Exception("PointerEventData is null in OnPointerEnter.");
			if (eventData.pointerEnter == null) throw new Exception("PointerEventData.pointerEnter is null in OnPointerEnter.");
			Debug.Log("OnPointerEnter event received from " + eventData.pointerEnter.name);
			if (!eventData.pointerEnter.TryGetComponent<Selectable>(out Selectable selectable)) return;
			Debug.Log("OnPointerEnter called on " + selectable.gameObject.name);
			foreach (Button button in _activeButtonReferences)
			{
				if (selectable as Button == button)
				{
					HighlightButton(button);
					Debug.Log("Pointer entered " + button.gameObject.name);
					continue;
				}
				if (button == null) continue;
				if (button.isActiveAndEnabled == false) continue;
				if (button.gameObject.activeInHierarchy == false) continue;
				DeselectButton(button);
				Debug.Log("Deselecting " + button.gameObject.name);
			}
		}

		//PRIVATE METHODS
		
		private void Awake()
		{
			_activeButtonReferences = new List<Button>();
		}

		private void OnEnable()
		{
			selectedButtonIndex = 0;
			if (_activeButtonReferences != null && _activeButtonReferences.Count > 0)
			{
				HighlightButton(_activeButtonReferences[selectedButtonIndex]);
			}

			if (_activeButtonReferences == null)
			{
				// throw new Exception("Button References not assigned in OptionButtonManager.");
				Debug.Log("Button References not assigned in OptionButtonManager.");
			}
		}

        private void Update()
		{
			if (_activeButtonReferences == null || _activeButtonReferences.Count == 0) enabled = false;
			
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
				_activeButtonReferences[selectedButtonIndex].onClick.Invoke();
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
			// SanityCheck();
			Debug.Log("Scrolling " + dir.ToString());

			DeselectButton(_activeButtonReferences[selectedButtonIndex]);

			switch (dir)
			{
				case ScrollDirection.Up:
					selectedButtonIndex -= 1;
					if (selectedButtonIndex < 0) selectedButtonIndex = _activeButtonReferences.Count - 1;
					Debug.Log("Scrolled Up to index " + selectedButtonIndex);
					break;

				case ScrollDirection.Down:
					selectedButtonIndex += 1;
					if (selectedButtonIndex > _activeButtonReferences.Count - 1) selectedButtonIndex = 0;
					Debug.Log("Scrolled Down to index " + selectedButtonIndex);
					break;
				default:
					throw new Exception("Invalid scroll direction");
			}

			HighlightButton(_activeButtonReferences[selectedButtonIndex]);
		}
		private void HighlightButton(Button buttonToHighlight)
		{
			// SanityCheck();
			buttonToHighlight.Select();
			buttonToHighlight.OnPointerEnter(null);
		}

		private void DeselectButton(Button buttonToDeselect)
		{
			// SanityCheck();
			buttonToDeselect.OnDeselect(null);
			buttonToDeselect.OnPointerExit(null);
		}

		// private void SanityCheck()
		// {
		// 	if (ButtonReferences == null || ButtonReferences.Count == 0) 
		// 		throw new Exception("No buttons to highlight");
		// 	if (selectedButtonIndex < 0 || selectedButtonIndex >= ButtonReferences.Count)
		// 		throw new Exception("Selected button index out of range");
		// }
	}
}
