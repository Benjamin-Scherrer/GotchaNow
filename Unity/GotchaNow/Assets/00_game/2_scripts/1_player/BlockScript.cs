using System.Collections;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public GameObject hitbox;
    private bool parryActive = false;
    private bool parryRefreshing = false;
    public float parryWindow = 0.1f;
    public float parryRefresh = 0.5f;

    public void StartBlock()
    {
        hitbox.SetActive(true);

        if(!parryRefreshing)
        {
            StartCoroutine(ParryWindow());
        }
    }

    public void EndBlock()
    {
        if (parryActive)
        {
            StopCoroutine(ParryWindow());

            parryActive = false;
            hitbox.GetComponent<Renderer>().material.color = Color.blue;
        }

        hitbox.SetActive(false);
    }

    private IEnumerator ParryWindow()
    {
        parryActive = true;
        float timer = 0f;

        hitbox.GetComponent<Renderer>().material.color = Color.yellow;

        while (timer < parryWindow)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        parryActive = false;
        hitbox.GetComponent<Renderer>().material.color = Color.blue;
        StartCoroutine(ParryRefresh());
    }

    private IEnumerator ParryRefresh()
    {
        float timer = 0f;
        parryRefreshing = true;

        while (timer < parryRefresh)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        parryRefreshing = false;
    }
}
