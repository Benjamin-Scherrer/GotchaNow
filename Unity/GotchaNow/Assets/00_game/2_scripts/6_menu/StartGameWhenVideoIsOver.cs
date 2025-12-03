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
				if(move.action.WasPerformedThisFrame() || click.action.WasPerformedThisFrame())
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
					if (move.action.WasPerformedThisFrame())
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
                    if (move.action.WasPerformedThisFrame())
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
