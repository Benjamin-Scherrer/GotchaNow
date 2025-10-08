using UnityEngine;
using System.Collections.Generic;
using UnityEditor.UIElements;

public class Enemy : MonoBehaviour
{
    //public readonly static HashSet<Enemy> Pool = new HashSet<Enemy>();

    //essential values
    public float health = 100;
    public float knockback = 0;
    public int proximity = 0;
    public BattleManager bm;
    public bool isLockOnTarget = false;

    [HideInInspector] public bool hit = false;

    //public GameObject dmgNumbers;
    //private GameObject HitBloom;

    private void Start()
    {
        bm = BattleManager.instance;
        bm.AddToEnemyList(this.gameObject);

        //Enemy.Pool.Add(this); //add to pool of alive enemies

        //HitBloom = GameObject.Find("VFXBloomWhite");
    }

    private void OnDisable()
    {
        bm.RemoveFromEnemyList(this.gameObject);
        //Enemy.Pool.Remove(this); //remove from pool of alive enemies
    }

    private void FixedUpdate()
    {
        if (health <= 0) //check if alive
        {
            //HitBloom.gameObject.GetComponent<HitBloom>().killCheck = true;

            Destroy(gameObject);
        }
    }
}

