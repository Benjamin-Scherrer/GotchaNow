using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

namespace GotchaNow
{
	public class StartGameWhenVideoIsOver : MonoBehaviour
	{
		[Header("Inputs")]
		[SerializeField] private InputActionReference move;
		[SerializeField] private InputActionReference click;

		[Header("References")]
		[SerializeField] private GameObject videoPreview;
		[SerializeField] private VideoPlayer videoPlayer;
		[SerializeField] private GameObject managers;
		[SerializeField] private GameObject Game;
		[SerializeField] private GameObject Phone;

		[Header("UI")]
		[SerializeField] private GameObject ui;
		[SerializeField] private UnityEngine.UI.Button uiSkipButton;
		private bool buttonSelected = false;

		private Vector2 previousMoveInput = Vector2.zero;

		// PRIVATE
		private void Awake()
		{
			videoPlayer.loopPointReached += OnVideoEnd;
			videoPlayer.Prepare();
			videoPlayer.prepareCompleted += StartVideo;

			videoPreview.SetActive(true);
			videoPlayer.gameObject.SetActive(true);
			managers.SetActive(false);
			Game.SetActive(false);
			Phone.SetActive(false);

			Time.timeScale = 1f;

			ui.SetActive(false);
			buttonSelected = false;
		}

		private void Update()
        {	
			// Debug.Log("Video time: " + videoPlayer.time);  
			if (ui.activeInHierarchy == false)
            {
				if(MoveCheck() || click.action.WasPerformedThisFrame())
				{
					ui.SetActive(true);
					return;
				}
				return;
            }
			if(ui.activeInHierarchy == true)
            {
				if(buttonSelected == false)
				{
					if (MoveCheck())
					{
						uiSkipButton.OnDeselect(null);
						uiSkipButton.OnPointerExit(null);
						buttonSelected = false;
						ui.SetActive(false);
						return;
					}
					if (click.action.WasPerformedThisFrame())
					{
						uiSkipButton.Select();
						uiSkipButton.OnSelect(null);
						uiSkipButton.OnPointerEnter(null);
						buttonSelected = true;
						return;
					}
					return;
				}
                if (buttonSelected)
                {
                    if (MoveCheck())
					{
						uiSkipButton.OnDeselect(null);
						uiSkipButton.OnPointerExit(null);
						buttonSelected = false;
						return;
					}
					if (click.action.WasPerformedThisFrame())
					{
						OnVideoEnd(videoPlayer);
						return;
					}
					return;
                }
			}
        }

		private bool MoveCheck()
		{
			// Debug.Log("MoveCheck | previousMoveInput: " + previousMoveInput + " | currentMoveInput: " + move.action.ReadValue<Vector2>());
			if(previousMoveInput.magnitude > 0 && move.action.WasPerformedThisFrame()) return false;
			// Debug.Log("Passed first check");
			if(move.action.ReadValue<Vector2>().magnitude == 0)
			{
				// Debug.Log("MoveCheck | Move input magnitude is zero");
				previousMoveInput = Vector2.zero;
				return false;
			}
			// Debug.Log("Passed second check");
			if(!move.action.WasPerformedThisFrame()) return false;
			// Debug.Log("Passed third check");
			previousMoveInput = move.action.ReadValue<Vector2>();
			return true;
		}

		private void StartVideo(VideoPlayer vp)
		{
			videoPreview.SetActive(false);
			vp.Play();
		}

		private void OnVideoEnd(VideoPlayer vp)
		{
			gameObject.SetActive(false);
			vp.gameObject.SetActive(false);

			managers.SetActive(true);
			Game.SetActive(true);
			Phone.SetActive(true);
		}
	}
}
