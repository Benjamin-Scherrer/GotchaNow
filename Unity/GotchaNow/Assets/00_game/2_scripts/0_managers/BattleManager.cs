using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;
    public List<GameObject> activeEnemy = new List<GameObject>();
    public Text debugText;
    public Image playerSprite;
    public Image enemySprite;
    public Image playerHP;
    public Image enemyHP;
    public Image notifBar;
    private Vector3 playerSpritePos;
    private Vector3 enemySpritePos;
    public float atkAnimationTime = 0.4f;
    public float HPdrainTime = 0.2f;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        playerSpritePos = playerSprite.rectTransform.position;
        enemySpritePos = enemySprite.rectTransform.position;
    }

    public IEnumerator SetTimeScale(float newTimeScale, float transitionTime)
    {
        float timer = 0;
        float currentTimeScale = Time.timeScale;

        while(timer < transitionTime)
        {
            timer += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(currentTimeScale, newTimeScale, timer/transitionTime);

            yield return new WaitForSecondsRealtime(0.016f);
        }

        Debug.Log("new TimeScale set");
        //Time.timeScale = newTimeScale;
    }

    public IEnumerator PlayerAttackUI()
    {
        float timer = 0;

        while (timer < atkAnimationTime)
        {
            timer += Time.deltaTime;

            if (timer < atkAnimationTime / 3)
            {
                playerSprite.rectTransform.position = Vector3.Lerp(playerSprite.rectTransform.position, enemySprite.rectTransform.position + new Vector3(-4, 0, 0), 0.75f);
            }
            else if (timer > atkAnimationTime / 2)
            {
                playerSprite.rectTransform.position = Vector3.Lerp(playerSprite.rectTransform.position, playerSpritePos, 0.33f);
                enemySprite.rectTransform.position = enemySpritePos + new Vector3(0, 2 * MathF.Sin(16 * Time.fixedTime), 0);
            }
            yield return null;
        }
        playerSprite.rectTransform.position = playerSpritePos;
        enemySprite.rectTransform.position = enemySpritePos;
    }

    public IEnumerator EnemyAttackUI()
    {
        float timer = 0;

        while (timer < atkAnimationTime)
        {
            timer += Time.deltaTime;

            if (timer < atkAnimationTime / 3)
            {
                enemySprite.rectTransform.position = Vector3.Lerp(enemySprite.rectTransform.position, playerSprite.rectTransform.position + new Vector3(4, 0, 0), 0.75f);
            }
            else if (timer > atkAnimationTime / 2)
            {
                enemySprite.rectTransform.position = Vector3.Lerp(enemySprite.rectTransform.position, enemySpritePos, 0.33f);
                playerSprite.rectTransform.position = playerSpritePos + new Vector3(0, 2 * MathF.Sin(16 * Time.fixedTime), 0);
            }
            yield return null;
        }

        playerSprite.rectTransform.position = playerSpritePos;
        enemySprite.rectTransform.position = enemySpritePos;
    }
    
    public IEnumerator UpdatePlayerHP(float oldHP, float newHP) //update player health on UI
    {
        float timer = 0;
        //Vector3 originalPos = enemyHP.rectTransform.position;

        playerHP.fillAmount = oldHP;

        while (timer < HPdrainTime)
        {
            timer += Time.deltaTime;
            playerHP.fillAmount = Mathf.Lerp(oldHP, newHP, timer / HPdrainTime); //HP drain animation
            yield return null;

            //enemyHP.rectTransform.position = originalPos + new Vector3(0, Mathf.Sin(4*Time.fixedTime), 0);
        }

        playerHP.fillAmount = newHP;
        //enemyHP.rectTransform.position = originalPos;
        
        //Debug.Log("fillAmount: " + playerHP.fillAmount);
    }

    public IEnumerator UpdateEnemyHP(float oldHP, float newHP) //update enemy health on UI
    {
        float timer = 0;
        //Vector3 originalPos = enemyHP.rectTransform.position;

        enemyHP.fillAmount = oldHP;

        while (timer < HPdrainTime)
        {
            timer += Time.deltaTime;
            enemyHP.fillAmount = Mathf.Lerp(oldHP, newHP, timer / HPdrainTime); //HP drain animation
            yield return null;

            //enemyHP.rectTransform.position = originalPos + new Vector3(0, Mathf.Sin(4*Time.fixedTime), 0);
        }

        enemyHP.fillAmount = newHP;
        //enemyHP.rectTransform.position = originalPos;

        //Debug.Log("fillAmount: " + enemyHP.fillAmount);
    }
    
    //handle list of active enemies for lock on
    public void AddToEnemyList(GameObject newEnemy) 
    {
        activeEnemy.Add(newEnemy);
    }

    public void RemoveFromEnemyList(GameObject newEnemy)
    {
        activeEnemy.Remove(newEnemy);
    }
}
