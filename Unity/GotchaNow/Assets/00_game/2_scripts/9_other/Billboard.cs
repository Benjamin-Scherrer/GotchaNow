using UnityEngine;

namespace GotchaNow
{
	public class Billboard : MonoBehaviour
	{
		[SerializeField] private Camera[] targetCameras;

        private void LateUpdate()
        {
            foreach (Camera cam in targetCameras)
			{
				if (cam == null) continue;
				if(cam.gameObject.activeInHierarchy == false) continue;
				transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward,
					cam.transform.rotation * Vector3.up);
			}
        }
    }
}
