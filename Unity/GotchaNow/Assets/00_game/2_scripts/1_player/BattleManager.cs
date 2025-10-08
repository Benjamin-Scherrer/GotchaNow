using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<GameObject> activeEnemy = new List<GameObject>();
    public Text debugText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        debugText.text = "";
        for (int i = 0; i < activeEnemy.Count; i++)
        {
            debugText.text += activeEnemy[i].name.ToString() + "\n";
        }
    }

    public void AddToEnemyList(GameObject newEnemy)
    {
        activeEnemy.Add(newEnemy);
    }

    public void RemoveFromEnemyList(GameObject newEnemy)
    {
        activeEnemy.Remove(newEnemy);
    }
}
