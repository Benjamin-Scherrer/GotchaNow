using UnityEngine;

namespace GotchaNow
{
	public class TitleAnimation : MonoBehaviour
	{
	[SerializeField] float speed = 2f;
    [SerializeField] float scaleAmount = 0.05f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.PingPong(Time.time * speed, scaleAmount);
        transform.localScale = baseScale * scale;
    }
}
}
