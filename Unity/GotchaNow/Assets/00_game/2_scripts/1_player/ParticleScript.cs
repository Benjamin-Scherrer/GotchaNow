using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public float lifetime = 1f;
    private float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (timer > lifetime)
        {
            Destroy(this.gameObject);
        }
    }
}
