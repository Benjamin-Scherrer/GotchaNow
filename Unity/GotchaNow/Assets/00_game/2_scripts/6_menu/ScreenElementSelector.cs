using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEssentials;

namespace GotchaNow
{
	public class ScreenElementSelector : MonoBehaviour, IPointerEnterHandler
	{
		[Header("Input Actions")]
		[SerializeField] private InputActionReference movement;
		[SerializeField] private InputActionReference up;
		[SerializeField] private InputActionReference down;
		[SerializeField] private InputActionReference submit;

		[Header("Variables")]
		[SerializeField] private float maxScrollSpeed = 0.1f;

		[Header("References")]
		[SerializeField] private List<Selectable> selectablesReferences;
		[SerializeField] private Button startingButton;

		private Selectable selectedSelectable;
		private bool navigating = false;
		private Vector2 navigationInput = Vector2.zero;
		private Vector2 previousNavigationInput = Vector2.zero;

		// PUBLIC
		public void OnPointerEnter(PointerEventData eventData)
		{
			if (eventData == null) throw new Exception("PointerEventData is null in OnPointerEnter.");
			if (eventData.pointerEnter == null) throw new Exception("PointerEventData.pointerEnter is null in OnPointerEnter.");
			Debug.Log("OnPointerEnter event received from " + eventData.pointerEnter.name);
			if (!eventData.pointerEnter.TryGetComponent<Selectable>(out selectedSelectable)) return;
			Debug.Log("OnPointerEnter called on " + selectedSelectable.gameObject.name);
			foreach (Selectable selectable in selectablesReferences)
			{
				if (selectedSelectable == selectable)
				{
					SelectSelectable(selectable);
					Debug.Log("Pointer entered " + selectable.gameObject.name);
					continue;
				}
				if (selectable == null) continue;
				if (selectable.isActiveAndEnabled == false) continue;
				if (selectable.gameObject.activeInHierarchy == false) continue;
				DeselectSelectable(selectable);
				Debug.Log("Deselecting " + selectable.gameObject.name);
			}
		}

		// PRIVATE
		private void OnEnable()
		{
			if (selectablesReferences == null)
			{
				// throw new Exception("Button References not assigned in OptionButtonManager.");
				Debug.Log("Button References not assigned in OptionButtonManager.");
			}

			if (selectablesReferences != null && selectablesReferences.Count > 0)
			{
				if (startingButton != null 
				&& startingButton.gameObject.activeInHierarchy
				&& startingButton.enabled)
				{
					selectedSelectable = startingButton;
				}
                else
                {
                    foreach(Selectable s in selectablesReferences)
                    {
                        if(s == null) continue;
						if(s.gameObject.activeInHierarchy == false) continue;
						if(s.enabled == false) continue;
						selectedSelectable = s;
						break;
                    }
                }

				foreach (Selectable s in selectablesReferences)
				{
					if (s != selectedSelectable)
					{
						DeselectSelectable(s);
						continue;
					}
					if (s == selectedSelectable)
					{
						SelectSelectable(s);
						continue;
					}
				}
				
				Debug.Log($"ScreenButtonSelector enabled, highlighting {selectedSelectable}");
			}
		}

		private void Update()
		{
			if (!gameObject.activeInHierarchy)
				throw new Exception("ScreenButtonSelector active but GameObject is not active in hierarchy.");
			if (selectablesReferences == null || selectablesReferences.Count == 0) enabled = false;

			// Get navigation direction.
			//if (movement.action.ReadValue<Vector2>().normalized.magnitude > 0.25f)
			navigationInput = movement.action.ReadValue<Vector2>();
			//navigationInput.y += up.action.ReadValue<float>() - down.action.ReadValue<float>();
			//navigationInput.Normalize();

			if (navigationInput.magnitude > 0.75f && !navigating)
			{
				if (navigationInput.x.Abs() >= navigationInput.y.Abs()) navigationInput.y = 0;
				else if (navigationInput.y.Abs() > navigationInput.x.Abs()) navigationInput.x = 0;
				StartCoroutine(Navigate());
				Debug.Log("Navigation started " + navigationInput.magnitude);
			}

			//submit
			if (submit.action.WasPressedThisFrame())
			{
				Debug.Log("Submit pressed on " + selectedSelectable);
				if(selectedSelectable is Button selectedButton)
				{
					Debug.Log("Invoking button " + selectedButton);
					selectedButton.onClick.Invoke();
                }
                    
				if(selectedSelectable is Slider slider)
				{
					Debug.Log("Focusing slider " + slider);
					slider.value = ((slider.value + 0.1f) % slider.maxValue) + slider.minValue;
				}
			}
		}

		//Direction
		private IEnumerator Navigate()
		{
			float timer = 0;
			float scrollSpeed = 0.5f;

			navigating = true;
			previousNavigationInput = navigationInput;
			ScrollRequest(navigationInput);

			while (navigating)
			{
				if (navigationInput.magnitude < 0.5f)
				{
					navigating = false;
					// Debug.Log("Navigation stopped");
					yield break;
				}
				bool isConsistent;
				isConsistent = IsNavigationInputConsistent(navigationInput, previousNavigationInput);
				if (!isConsistent)
				{
					navigating = false;
					// Debug.Log("Navigation stopped due to inconsistent input");
					yield break;
				}

				timer += Time.unscaledDeltaTime;
				if (timer > scrollSpeed)
				{
					ScrollRequest(navigationInput);
					scrollSpeed /= 2;
					if (scrollSpeed < maxScrollSpeed) scrollSpeed = maxScrollSpeed;
					timer = 0;
				}

				previousNavigationInput = navigationInput;
				yield return null;
			}
		}

		// Check if input direction changed

		private bool IsNavigationInputConsistent(Vector2 navigationInput, Vector2 previousNavigationInput)
		{
			// Debug.Log("Checking navigation input consistency between " + navigationInput + " and " + previousNavigationInput);
			float maxAngle = 45f;
			float angle = Vector2.Angle(navigationInput, previousNavigationInput);
			if (angle > maxAngle)
			{
				return false;
			}
			return true;
		}

		//Scroll 
		private void ScrollRequest(Vector2 navigationInput)
		{
			 Debug.Log("Scroll Request received with input " + navigationInput);
			if (selectablesReferences.Count == 1)
			{
				SelectSelectable(selectablesReferences[0]);
				return;
			}

			//Differentiate between Slider and other Selectables
			if (selectedSelectable is Slider slider 
			&& (Mathf.Abs(navigationInput.x) - Mathf.Abs(navigationInput.y) > 0))
			{
				SliderAdjustment(slider, navigationInput);
				return;
			}

			// get Closest Selectable in direction
			Selectable selectable = null;
			float distance = float.MaxValue;
			float angle = 80f;

			foreach(Selectable sel in selectablesReferences)
			{
				if (sel == null) continue;
				if (sel.gameObject.activeInHierarchy == false) continue;
				if (sel.enabled == false) continue;
				// Debug.Log("Selectable " + sel.gameObject.name + " is active and enabled.");

				if (selectedSelectable == null) selectedSelectable = sel;
				if (selectedSelectable.gameObject.activeInHierarchy == false) selectedSelectable = sel;
				if (selectedSelectable.enabled == false) selectedSelectable = sel;
				if (sel == selectedSelectable) continue;
				// Debug.Log("Selectable " + sel.gameObject.name + " is not the currently selected selectable.");

				Vector2 currentPosition = selectedSelectable.transform.position;
				Vector2 selPosition = sel.transform.position;
				Vector2 directionToSelectableUnnormalized = selPosition - currentPosition;
				float distanceToSelectable = directionToSelectableUnnormalized.magnitude;
				Vector2 directionToSelectable = directionToSelectableUnnormalized / distanceToSelectable;
				// Debug.Log("Direction to selectable " + sel.gameObject.name + " is " + directionToSelectable);

				float a = Vector2.Angle(navigationInput, directionToSelectable);
				// Debug.DrawLine(currentPosition, selPosition, Color.red, 2f);
				// Debug.DrawLine(currentPosition, currentPosition + navigationInput * distanceToSelectable, Color.blue, 2f);
				if (a > angle) continue;
				// Debug.Log("Selectable " + sel.gameObject.name + " is within angle threshold with angle " + a);

				if (a == angle && distanceToSelectable >= distance) continue;
				//  Debug.Log("Selectable " + sel.gameObject.name + " is the closest selectable so far with distance " + distanceToSelectable);

				angle = a;
				distance = distanceToSelectable;
				selectable = sel;
			}
			if (selectable == null) return;
			// SanityCheck();
			//  Debug.Log("Scrolling " + navigationInput + " to selectable " + selectable);
			//  Debug.DrawLine(selectedSelectable.transform.position, selectable.transform.position, Color.green, 2f);

			// Smoothly scroll to the new selectable
			DeselectSelectable(selectedSelectable);

			if(selectable is SelectableLink selectableLink)
			{
				selectable = selectableLink.GetSelectableLink;
				Debug.Log("Following SelectableLink to " + selectable);
			}
			selectedSelectable = selectable;
			SelectSelectable(selectedSelectable);
		}

		private void SelectSelectable(Selectable selectable)
		{
			selectable.OnSelect(null);
			selectable.OnPointerEnter(null);

			if(selectable.TryGetComponent(out ButtonTextHider buttonTextHider))
			{
				buttonTextHider.ShowButtonText();
			}
		}

		private void DeselectSelectable(Selectable selectable)
		{
			selectable.OnDeselect(null);
			selectable.OnPointerExit(null);

			if(selectable.TryGetComponent(out ButtonTextHider buttonTextHider))
			{
				buttonTextHider.HideButtonText();
			}
		}

		private void SliderAdjustment(Slider slider, Vector2 navigationInput)
		{
			float adjustment = Mathf.Sign(navigationInput.x) * (slider.maxValue - slider.minValue) * 0.1f;
			SliderAdjustment(slider, adjustment);
		}

		private void SliderAdjustment(Slider slider, float adjustment)
		{
			slider.value = Mathf.Clamp(slider.value + adjustment, slider.minValue, slider.maxValue);
		}
	}
}
