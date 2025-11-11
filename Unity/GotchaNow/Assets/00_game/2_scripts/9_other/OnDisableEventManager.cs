using System;
using UnityEngine;

namespace GotchaNow
{
	public class SpaceFitterReference : MonoBehaviour
	{
		public event Action OnDisableEvent;
		public event Action OnEnableEvent;

		public event Action OnRectTransformDimensionsChangeEvent;

		private void OnDisable()
		{
			if (OnDisableEvent != null)
			{
				OnDisableEvent.Invoke();
				Debug.Log($"OnDisableEvent invoked on {gameObject.name}");
			}
		}

		private void OnEnable()
		{
			if(OnEnableEvent != null)
			{
				OnEnableEvent.Invoke();
				Debug.Log($"OnEnableEvent invoked on {gameObject.name}");
			}
		}

        private void OnRectTransformDimensionsChange()
        {
			if(OnRectTransformDimensionsChangeEvent != null)
			{
				OnRectTransformDimensionsChangeEvent.Invoke();
				Debug.Log($"OnRectTransformDimensionsChangeEvent invoked on {gameObject.name}");
			}
		}
    }
}
