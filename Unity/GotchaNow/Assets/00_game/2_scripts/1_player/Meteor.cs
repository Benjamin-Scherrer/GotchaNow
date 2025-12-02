using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public float damage = 50;
    public float knockback = 10;
    public float speed = 2;
    public GameObject targetEnemy;
    private Vector3 enemyPos;
    private Vector3 aimVector;
    private bool falling = false;
    public LockOnTarget lockOnTarget;
    public GameObject explosionVFX;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {        
        ProgressionManager.instance.EndBattleEvent.AddListener(EndBattle);
        
        lockOnTarget = LockOnTarget.instance;
        targetEnemy = BattleManager.instance.activeEnemy[0];

        Vector3 displacement = targetEnemy.transform.position - PlayerBattle.Instance.gameObject.transform.position;

        transform.position += displacement.normalized * 10;

        StartCoroutine(Fall());
    }

    private IEnumerator Fall()
    {
        falling = true;

        PlayerBattle.Instance.meteorExists = true;

        if (PlayerBattle.Instance.lockedOn == false)
        {
            PlayerBattle.Instance.lockedOn = true;
            PlayerBattle.Instance.LockOn();
        }

        lockOnTarget.targetEnemy = this.gameObject;
        
        while (falling)
        {
            aimVector = (targetEnemy.transform.position - transform.position).normalized;
            
            transform.position += speed * Time.fixedDeltaTime * aimVector;

            yield return new WaitForFixedUpdate();
        }

        PlayerBattle.Instance.meteorExists = false;

        //PlayerBattle.Instance.lockedOn = false;
        //PlayerBattle.Instance.LockOn();

        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log("hit");
            
            Enemy enemy = other.GetComponent<Enemy>();

            lockOnTarget.targetEnemy = other.gameObject;

            float damageCalc = damage;
            float knockbackCalc = knockback;

            /* GameObject dmgNumber = Instantiate(other.GetComponent<Enemy>().dmgNumbers, other.transform.position, Quaternion.identity);
            dmgNumber.GetComponentInChildren<TextMesh>().text = damageCalc.ToString(); */

            enemy.HitByAttack(damageCalc, knockbackCalc);
            Instantiate(explosionVFX, transform.position, Quaternion.identity);

            //hit audio;

            //HitBloom.gameObject.GetComponent<HitBloom>().hitCheck = true;

            falling = false;
        }
    }

    void EndBattle() //triggered through event
    {
        Destroy(this.gameObject);
    }
}
