using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public readonly static HashSet<Enemy> Pool = new HashSet<Enemy>();

    //essential values
    //public GameObject dmgNumbers;

    public float health = 100;
    public float knockback = 0;
    public int proximity = 0;

    [HideInInspector] public bool hit = false;

    //private GameObject HitBloom;

    private void OnEnable()
    {
        Enemy.Pool.Add(this); //add to pool of alive enemies

        //HitBloom = GameObject.Find("VFXBloomWhite");
    }

    private void OnDisable()
    {
        //remove from pool of alive enemies
        Enemy.Pool.Remove(this);
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

