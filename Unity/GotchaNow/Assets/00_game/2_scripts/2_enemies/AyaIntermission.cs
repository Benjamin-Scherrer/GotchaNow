using UnityEngine;

public class AyaIntermission : MonoBehaviour
{
    public Animator animator;
    public bool isDefeated = false;

    void OnEnable()
    {
        animator.SetTrigger("startIntermission");
    }

    // Update is called once per frame
    public void EndIntermission()
    {
        animator.SetTrigger("endIntermission");
    }

    public void Defeated()
    {
        animator.SetTrigger("defeated");
    }
}
