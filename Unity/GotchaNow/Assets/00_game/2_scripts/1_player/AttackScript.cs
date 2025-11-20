using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackScript : MonoBehaviour
{
    public GameObject hitbox;
    public GameObject hitbox2;
    public string attackType;
    public float windupTime;
    public float windup2Time;
    public float endTime;
    public float hitboxDmg;
    public float hitbox2Dmg;

    void Start()
    {
        ProgressionManager.instance.EndBattleEvent.AddListener(EndBattle);
    }

    public void StartAttack()
    {   
        if (attackType == "playerAttack")
        {
            StartCoroutine(PlayerAttack());
        }
        else if (attackType == "playerHeavyAttack")
        {
            StartCoroutine(PlayerHeavyAttack());
        }
        else
        {
            hitbox.SetActive(true);
        }
    }

    public void EndAttack()
    {
        hitbox.SetActive(false);
        if (hitbox2 != null)
        {
            hitbox2.SetActive(false);
        }
    }

    private IEnumerator PlayerAttack()
    {
        AttackBox atkBox = hitbox.GetComponent<AttackBox>();
        
        float timer = 0;

        atkBox.damage = hitboxDmg;

        while (timer < windupTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        atkBox.baseDmg = hitboxDmg;
        atkBox.duration = endTime - windupTime;

        hitbox.SetActive(true);
    }

    private IEnumerator PlayerHeavyAttack()
    {
        float timer = 0;

        AttackBox atkBox1 = hitbox.GetComponent<AttackBox>();
        AttackBox atkBox2 = hitbox2.GetComponent<AttackBox>();

        while (timer < windupTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        atkBox1.baseDmg = hitboxDmg;
        atkBox1.duration = 0.3f;
        hitbox.SetActive(true);

        while (timer < windup2Time)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        atkBox2.baseDmg = hitbox2Dmg;
        atkBox2.duration = 0.25f;
        hitbox2.SetActive(true);
    }

    void EndBattle() //triggered through event
    {
        StopAllCoroutines();
        EndAttack();
    }
}
