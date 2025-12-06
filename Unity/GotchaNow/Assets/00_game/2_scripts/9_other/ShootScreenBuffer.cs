using System.Collections;
using UnityEngine;

namespace GotchaNow
{
	public class ShootScreenBuffer : MonoBehaviour
	{
		[SerializeField] private bool QuickButton;

		[SerializeField] private Camera camMain;
		[SerializeField] private Camera camOverride;

		//PUBLIC METHODS
		// public IEnumerator ShootScreen()
		// {
		// 	// yield return Wait();
		// 	yield break;
		// }

		//PRIVATE METHODS
		private void Awake()
		{
			StartCoroutine(WaitAndDisable());
		}

		private IEnumerator WaitAndDisable()
		{
			camOverride.enabled = true;
			yield return new WaitForSecondsRealtime(5f);
			camOverride.enabled = false;
		}
		// private void WaitAndShoot()
		// {
		// 	StartCoroutine(Wait());
		// }

		// private IEnumerator Wait()
		// {
		// 	yield return new WaitForEndOfFrame();
		// 	Shoot();
		// }

		// private void Shoot()
        // {
        //     Debug.Log("QuickButton pressed, shooting screen buffer");
		// 	camOverride.Render();
		// 	camMain.enabled = false;
		// 	camMain.Render();
		// 	camMain.enabled = true;
		// }

		// private void OnValidate()
        // {
		// 	if (QuickButton)
		// 	{
		// 		WaitAndShoot();

		// 		QuickButton = false;
		// 	}
        // }
	}
}
