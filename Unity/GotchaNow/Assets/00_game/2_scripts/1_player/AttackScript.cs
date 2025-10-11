using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject hitbox;

    public void StartAttack()
    {
        hitbox.SetActive(true);
    }

    public void EndAttack()
    {
        hitbox.SetActive(false);
    }
}
