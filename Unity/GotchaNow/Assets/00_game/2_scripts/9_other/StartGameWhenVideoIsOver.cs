using UnityEngine;
using UnityEngine.Video;

namespace GotchaNow
{
	public class StartGameWhenVideoIsOver : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] private GameObject videoPreview;
		[SerializeField] private VideoPlayer videoPlayer;
		[SerializeField] private GameObject managers;
		[SerializeField] private GameObject Game;
		[SerializeField] private GameObject Phone;

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
		}

		private void Update()
        {	
			Debug.Log("Video time: " + videoPlayer.time);          
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
