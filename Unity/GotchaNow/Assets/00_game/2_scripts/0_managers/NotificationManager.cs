using FMODUnity;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using GotchaNow;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;

    [Header("Inputs")]
    public InputActionReference upInput;
    public InputActionReference downInput;
    public InputActionReference menuInput;
    private bool upScrolling;
    private bool downScrolling;

    [Header("Variables")]
    public float maxScrollSpeed = 0.1f;
    public float openTime = 0.3f;
    public bool menuOpen = false;
    private bool menuReady = true;
    private Vector2 closedSize = new (256.1538f, 51.5f); //new Vector2 (320, 48);
    private Vector2 openSize = new (256.1538f, 171f+8f); //new Vector2(320, 240);

    [Header("References")]
    public List<GameObject> request = new ();
    private int selectedRequest = 4;
    public GameObject healRequest;
    public GameObject buffRequest;
    public GameObject meteorRequest;
    public GameObject cancelRequest;
    public GameObject notifMenu;
    public GameObject selector;
    public GameObject buttonSouth;
    [Space(10)]
    public Image notifBarUI;
    public Image quotaBarUI;
    public TextMeshProUGUI notifPercent;
    public TextMeshProUGUI quotaPercent;
    [Space(10)]
    public GameObject Meteor;
    [Header("Variables")]
    public float quotaFillTime = 0.33f;
    private float chargeTimer = 0;
    public float chargePerSecond = 0.05f;
    public float notifFillTime = 1f;
    public float notifCharge = 0;
    public float currentQuota = 0;
    public float maxQuota = 0;
    public float healQuota = 10f;
    public float buffQuota = 15f;
    public float meteorQuota = 40f;

    
    [Header("Events")]
    public UnityEvent BuffEnabled;
    public UnityEvent BuffDisabled;

    private readonly List<float> enableTresholds = new ();

    void Awake()
    {
        instance = this;

        request.Add(healRequest);
        request.Add(buffRequest);
        request.Add(meteorRequest);
        request.Add(cancelRequest);

        BuffEnabled = new UnityEvent();
        BuffDisabled = new UnityEvent();   

        selector.SetActive(false);
        request.ForEach(req => req.SetActive(false));

        float enableTresholdTotal = 44.76923f; //initial offset from closed size to first item
        foreach(GameObject req in request)
        {
            RectTransform rt = req.transform as RectTransform;
            enableTresholdTotal += rt.sizeDelta.y;
            enableTresholds.Add(enableTresholdTotal);
        }
    }

    void Start()
    {
        notifCharge = 0;
        currentQuota = 0;
        
        ChargeNotifBar(0);
        ChargeQuota(0);
    }

    void Update()
    {
        if (menuInput.action.IsPressed() && menuReady)
        {
            if (!menuOpen && notifCharge >= 1)
            {
                StartCoroutine(OpenRequestMenu());
                Debug.Log("opening menu");
            }
            else if (menuOpen && notifCharge >= 1)
            {
                SelectMenuItem();
            }

            menuReady = false;
        }

        if (menuOpen) //scrolling
        {
            if (upInput.action.IsPressed() && !upScrolling)
            {
                StartCoroutine(UpScroll());
            }
            else if (downInput.action.IsPressed() && !downScrolling)
            {
                StartCoroutine(DownScroll());
            }
        }

        if (!menuReady) //re-enable input when button released
        {
            if (!menuInput.action.IsPressed())
            {
                menuReady = true;
            }
        }

        chargeTimer += Time.deltaTime;
        
        if (chargeTimer >= 1)
        {
            if (notifCharge + chargePerSecond > 1)
            {
                ChargeNotifBar(1 - notifCharge);
            }
            else if (notifCharge < 1)
            {
                ChargeNotifBar(chargePerSecond);
            }
            chargeTimer = 0;
        }
    }

    public IEnumerator OpenRequestMenu()
    {
        float timer = 0;
        selectedRequest = 3;

        menuOpen = true;

        RectTransform rt = notifMenu.GetComponent<RectTransform>();
        rt.sizeDelta = closedSize;

        buttonSouth.SetActive(false);

        //stop time, muffle music
        StartCoroutine(BattleManager.instance.SetTimeScale(0f,openTime,1f));
        MusicPlayer.instance.SetLowPassFilter(1f);

        RuntimeManager.PlayOneShot(UiSfxPlayer.instance.openNotifMenu, transform.position); //play sfx
        
        while (timer < openTime)
        {
            timer += Time.unscaledDeltaTime;
            rt.sizeDelta = Vector2.Lerp(closedSize, openSize, timer / openTime);

            // if (!healRequest.activeSelf && rt.sizeDelta.y > 96)
            // {
            //     healRequest.SetActive(true);
            // }
            // if (!buffRequest.activeSelf && rt.sizeDelta.y > 144)
            // {
            //     buffRequest.SetActive(true);
            // }
            // if (!meteorRequest.activeSelf && rt.sizeDelta.y > 192)
            // {
            //     meteorRequest.SetActive(true);
            // }
            for (int i = 0; i < request.Count - 1; i++)
            {
                var requestItem = request[i];
                Debug.Log("OpenRequest - Checking " + requestItem.name + ". SizeDelta.y: " + rt.sizeDelta.y + ", Treshold: " + enableTresholds[i]);
                if(requestItem.activeSelf) continue;
                if(rt.sizeDelta.y <= enableTresholds[i]) continue;
                request[i].SetActive(true);

            }

            yield return null;
        }

        cancelRequest.SetActive(true);
        selector.SetActive(true);
        buttonSouth.SetActive(true);

        HighlightRequest(3);

        rt.sizeDelta = openSize;
    }

    public IEnumerator CloseRequestMenu()
    {
        float timer = 0;
        menuOpen = false;

        RectTransform rt = notifMenu.GetComponent<RectTransform>();
        rt.sizeDelta = openSize;

        selector.SetActive(false);
        buttonSouth.SetActive(false);
        cancelRequest.SetActive(false);

        while (timer < openTime)
        {
            timer += Time.unscaledDeltaTime;
            rt.sizeDelta = Vector2.Lerp(openSize, closedSize, timer / openTime);

            // if (meteorRequest.activeSelf && rt.sizeDelta.y < 192)
            // {
            //     meteorRequest.SetActive(false);
            // }
            // if (buffRequest.activeSelf && rt.sizeDelta.y < 144)
            // {
            //     buffRequest.SetActive(false);
            // }
            // if (healRequest.activeSelf && rt.sizeDelta.y < 96)
            // {
            //     healRequest.SetActive(false);
            // }
            for (int i = request.Count - 1; i >= 0; i--)
            {
                var requestItem = request[i];
                Debug.Log("CloseRequest - Checking " + requestItem.name + ". SizeDelta.y: " + rt.sizeDelta.y + ", Treshold: " + enableTresholds[i]);
                if(!requestItem.activeSelf) continue;
                if(rt.sizeDelta.y >= enableTresholds[i]) continue;
                request[i].SetActive(false);
            }

            yield return null;
        }

        rt.sizeDelta = closedSize;

        buttonSouth.SetActive(true);
        RectTransform buttonRt = buttonSouth.GetComponent<RectTransform>();
        buttonRt.anchoredPosition = new Vector3 (-2.5f, -12);//new Vector3(-132, -24, 0);
    }

    private IEnumerator UpScroll()
    {
        float timer = 0;
        float scrollSpeed = 0.5f;

        upScrolling = true;

        ScrollRequest("up");

        while (upInput.action.IsPressed())
        {
            timer += Time.unscaledDeltaTime;

            if (timer > scrollSpeed)
            {
                ScrollRequest("up");

                scrollSpeed /= 2;
                if (scrollSpeed < maxScrollSpeed) scrollSpeed = maxScrollSpeed;

                timer = 0;
            }

            yield return null;
        }

        upScrolling = false;
    }

    private IEnumerator DownScroll()
    {
        float timer = 0;
        float scrollSpeed = 0.5f;

        downScrolling = true;

        ScrollRequest("down");

        while (downInput.action.IsPressed())
        {
            timer += Time.unscaledDeltaTime;

            if (timer > scrollSpeed)
            {
                ScrollRequest("down");

                scrollSpeed /= 2;
                if (scrollSpeed < maxScrollSpeed) scrollSpeed = maxScrollSpeed;

                timer = 0;
            }

            yield return null;
        }

        downScrolling = false;
    }

    private void ScrollRequest(string dir)
    {
        if (dir == "up")
        {
            if (selectedRequest > 0) selectedRequest -= 1;
            else selectedRequest = 3;

            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.scrollUp, transform.position); //play sfx
        }

        else if (dir == "down")
        {
            if (selectedRequest < 3) selectedRequest += 1;
            else selectedRequest = 0;

            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.scrollDown, transform.position); //play sfx
        }

        HighlightRequest(selectedRequest);
    }
    private void HighlightRequest(int pos)
    {
        selectedRequest = pos;

        RectTransform selectorRt = selector.GetComponent<RectTransform>();
        RectTransform buttonRt = buttonSouth.GetComponent<RectTransform>();

        // selectorRt.anchoredPosition = new Vector3(0, -48 + (selectedRequest * -48), 0);

        Vector3 goalPos = request[pos].GetComponent<RectTransform>().anchoredPosition;
        selectorRt.anchoredPosition = goalPos;

        // buttonRt.anchoredPosition = new Vector3(-132, -72 + (selectedRequest * -48), 0);
        buttonRt.anchoredPosition = new Vector3(buttonRt.anchoredPosition.x, goalPos.y, goalPos.z);
    }

    private void SelectMenuItem()
    {
        if (selectedRequest == 0) //heal
        {
            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.confirm, transform.position); //play sfx
            RequestHeal();            
        }
        if (selectedRequest == 1) //buff
        {
            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.confirm, transform.position); //play sfx
            RequestBuff();          
        }
        if (selectedRequest == 2) //meteor
        {
            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.confirm, transform.position); //play sfx
            RequestMeteor();           
        }
        if (selectedRequest == 3) //cancel, close menu
        {
            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.back, transform.position); //play sfx
            StartCoroutine(CloseRequestMenu());

            StartCoroutine(BattleManager.instance.SetTimeScale(1f,openTime,0f)); //reset timescale
            MusicPlayer.instance.SetLowPassFilter(0f);
        }
    }

    private void RequestHeal()
    {
        ChargeNotifBar(-1);

        PopupManager.instance.ShowHealMePopup();
        RuntimeManager.PlayOneShot(UiSfxPlayer.instance.phoneNotification, transform.position); //play sfx

        StartCoroutine(CloseRequestMenu());
    }

    private void RequestBuff()
    {
        ChargeNotifBar(-1);       

        PopupManager.instance.ShowBuffMePopup();
        RuntimeManager.PlayOneShot(UiSfxPlayer.instance.phoneNotification, transform.position); //play sfx

        StartCoroutine(CloseRequestMenu());
    }

    private void RequestMeteor()
    {
        ChargeNotifBar(-1);        

        PopupManager.instance.ShowMeteoriteNowPopup();
        RuntimeManager.PlayOneShot(UiSfxPlayer.instance.phoneNotification, transform.position); //play sfx

        StartCoroutine(CloseRequestMenu());
    }

    public void AcceptHeal()
    {
        StartCoroutine(BattleManager.instance.SetTimeScale(1f,openTime,0f));
        MusicPlayer.instance.SetLowPassFilter(0f);
        
        PlayerBattle.Instance.Heal(50);

        ChargeQuota(healQuota);
    }

    public void AcceptBuff()
    {
        StartCoroutine(BattleManager.instance.SetTimeScale(1f,openTime,0f));
        MusicPlayer.instance.SetLowPassFilter(0f);

        BuffEnabled.Invoke();

        ChargeQuota(buffQuota);
    }
    public void AcceptMeteor()
    {
        StartCoroutine(BattleManager.instance.SetTimeScale(1f,openTime,0f));
        MusicPlayer.instance.SetLowPassFilter(0f);

        Instantiate(Meteor, new Vector3 (200,90,0), Quaternion.identity);

        ChargeQuota(meteorQuota);
    }

    public void ChargeNotifBar(float amount)
    {
        StartCoroutine(UpdateNotifUI(notifCharge, notifCharge + amount));
        
        notifCharge += amount;
    }
    
    public void ChargeQuota(float amount)
    {
        StartCoroutine(UpdateQuotaUI(currentQuota, currentQuota+amount));
        currentQuota += amount;
    }

    private IEnumerator UpdateNotifUI(float oldCharge, float newCharge)
    {
        float timer = 0;
        int fillPercentage;

        while (timer < notifFillTime)
        {
            timer += Time.unscaledDeltaTime;

            notifBarUI.fillAmount = Mathf.Lerp(oldCharge, newCharge, timer / notifFillTime);

            fillPercentage = (int)(notifBarUI.fillAmount * 100);
            notifPercent.text = fillPercentage.ToString() + "%";

            yield return null;
        }

        if (oldCharge < 1 && newCharge >= 1)
        {
            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.notificationReady, transform.position); //play sfx
        }

        notifBarUI.fillAmount = newCharge;

        fillPercentage = (int)(newCharge * 100);
        notifPercent.text = fillPercentage.ToString() + "%";
    }

    public IEnumerator UpdateQuotaUI(float oldQuota, float newQuota)
    {
        float timer = 0;
        int fillPercentage;

        if (newQuota > oldQuota)
        {
            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.fillQuota, transform.position); //play sfx
        }

        while (timer < quotaFillTime)
        {
            timer += Time.deltaTime;
            quotaBarUI.fillAmount = Mathf.Lerp(oldQuota / maxQuota, newQuota / maxQuota, timer / quotaFillTime);

            fillPercentage = (int)(quotaBarUI.fillAmount * 100);
            quotaPercent.text = fillPercentage.ToString() + "%";

            yield return null;
        }

        quotaBarUI.fillAmount = newQuota / maxQuota;

        if (newQuota >= maxQuota)
        {
            RuntimeManager.PlayOneShot(UiSfxPlayer.instance.quotaFull, transform.position);
        } 

        //Debug.Log("fillAmount: " + quotaBar.fillAmount);
    }

    public void StartBattle()
    {
        notifCharge = 0;

        notifBarUI.fillAmount = 0;
        notifPercent.text = "0%";

        quotaBarUI.fillAmount = currentQuota/maxQuota;
        
        int fillPercentage = (int)(quotaBarUI.fillAmount * 100);
        quotaPercent.text = fillPercentage.ToString() + "%";
    }
    
    public void FullReset()
    {
        notifCharge = 0;
        currentQuota = 0;
        ChargeNotifBar(0);
        ChargeQuota(0);
    }
}
