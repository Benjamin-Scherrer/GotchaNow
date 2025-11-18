using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using System.Threading;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Splines.Interpolators;
using TMPro;
using GotchaNow;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager instance;
    public InputActionReference upInput;
    public InputActionReference downInput;
    public InputActionReference menuInput;
    private bool upScrolling;
    private bool downScrolling;
    public float maxScrollSpeed = 0.1f;
    public List<GameObject> request = new List<GameObject>();
    private int selectedRequest = 4;
    public GameObject healRequest;
    public GameObject buffRequest;
    public GameObject meteorRequest;
    public GameObject cancelRequest;
    public GameObject notifMenu;
    public GameObject selector;
    public GameObject buttonSouth;
    public float openTime = 0.3f;
    public bool menuOpen = false;
    private bool menuReady = true;
    private Vector2 closedSize = new Vector2 (320, 48);
    private Vector2 openSize = new Vector2(320, 240);
    public Image notifBarUI;
    public Image quotaBarUI;
    public TextMeshProUGUI notifPercent;
    public TextMeshProUGUI quotaPercent;
    public float quotaFillTime = 0.33f;
    private float chargeTimer = 0;
    public float chargePerSecond = 0.05f;
    public float notifFillTime = 1f;
    public float notifCharge = 0;
    public float currentQuota = 0;
    public float maxQuota = 0;
    public GameObject Meteor;
    public UnityEvent BuffEnabled;
    public UnityEvent BuffDisabled;

    void Awake()
    {
        instance = this;

        request.Add(healRequest);
        request.Add(buffRequest);
        request.Add(meteorRequest);
        request.Add(cancelRequest);

        BuffEnabled = new UnityEvent();
        BuffDisabled = new UnityEvent();   
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
        
        while (timer < openTime)
        {
            timer += Time.unscaledDeltaTime;
            rt.sizeDelta = Vector2.Lerp(closedSize, openSize, timer / openTime);

            if (!healRequest.activeSelf && rt.sizeDelta.y > 96)
            {
                healRequest.SetActive(true);
            }
            if (!buffRequest.activeSelf && rt.sizeDelta.y > 144)
            {
                buffRequest.SetActive(true);
            }
            if (!meteorRequest.activeSelf && rt.sizeDelta.y > 192)
            {
                meteorRequest.SetActive(true);
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

            if (meteorRequest.activeSelf && rt.sizeDelta.y < 192)
            {
                meteorRequest.SetActive(false);
            }
            if (buffRequest.activeSelf && rt.sizeDelta.y < 144)
            {
                buffRequest.SetActive(false);
            }
            if (healRequest.activeSelf && rt.sizeDelta.y < 96)
            {
                healRequest.SetActive(false);
            }

            yield return null;
        }

        rt.sizeDelta = closedSize;

        buttonSouth.SetActive(true);
        RectTransform buttonRt = buttonSouth.GetComponent<RectTransform>();
        buttonRt.anchoredPosition= new Vector3(-132, -24, 0);
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

                scrollSpeed = scrollSpeed / 2;
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

                scrollSpeed = scrollSpeed / 2;
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
        }

        else if (dir == "down")
        {
            if (selectedRequest < 3) selectedRequest += 1;
            else selectedRequest = 0;
        }

        HighlightRequest(selectedRequest);
    }
    private void HighlightRequest(int pos)
    {
        selectedRequest = pos;

        RectTransform selectorRt = selector.GetComponent<RectTransform>();
        RectTransform buttonRt = buttonSouth.GetComponent<RectTransform>();

        selectorRt.anchoredPosition = new Vector3(0, -48 + (selectedRequest * -48), 0);
        buttonRt.anchoredPosition = new Vector3(-132, -72 + (selectedRequest * -48), 0);
    }

    private void SelectMenuItem()
    {
        if (selectedRequest == 0) //heal
        {
            RequestHeal();            
        }
        if (selectedRequest == 1) //buff
        {
            RequestBuff();          
        }
        if (selectedRequest == 2) //meteor
        {
            RequestMeteor();           
        }
        if (selectedRequest == 3) //cancel, close menu
        {
            StartCoroutine(CloseRequestMenu());
            Debug.Log("closing menu");
        }
    }

    private void RequestHeal()
    {
        ChargeNotifBar(-1);
        ChargeQuota(30);

        PopupManager.instance.ShowHealMePopup();

        StartCoroutine(CloseRequestMenu());
    }

    private void RequestBuff()
    {
        ChargeNotifBar(-1);
        ChargeQuota(30);

        PopupManager.instance.ShowBuffMePopup();
        // AcceptBuff(); //wip

        StartCoroutine(CloseRequestMenu());
    }

    private void RequestMeteor()
    {
        ChargeNotifBar(-1);
        ChargeQuota(50);

        PopupManager.instance.ShowMeteoriteNowPopup();
        // AcceptMeteor(); //wip

        StartCoroutine(CloseRequestMenu());
    }

    public void AcceptHeal()
    {
        PlayerBattle.Instance.Heal(50);
    }

    public void AcceptBuff()
    {
        //buff effect
        Debug.Log("Buff accepted");

        BuffEnabled.Invoke();
    }
    public void AcceptMeteor()
    {
        //spawn meteor
        Debug.Log("Meteor accepted");

        Instantiate(Meteor, new Vector3 (200,90,0), Quaternion.identity);
    }

    public void ChargeNotifBar(float amount)
    {
        StartCoroutine(UpdateNotifUI(notifCharge, notifCharge + amount));
        notifCharge = notifCharge + amount;
    }
    
    public void ChargeQuota(float amount)
    {
        StartCoroutine(UpdateQuotaUI(currentQuota, currentQuota+amount));
        currentQuota = currentQuota + amount;
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

        notifBarUI.fillAmount = newCharge;

        fillPercentage = (int)(newCharge * 100);
        notifPercent.text = fillPercentage.ToString() + "%";
    }

    public IEnumerator UpdateQuotaUI(float oldQuota, float newQuota)
    {
        float timer = 0;
        int fillPercentage;

        while (timer < quotaFillTime)
        {
            timer += Time.deltaTime;
            quotaBarUI.fillAmount = Mathf.Lerp(oldQuota / maxQuota, newQuota / maxQuota, timer / quotaFillTime);

            fillPercentage = (int)(quotaBarUI.fillAmount * 100);
            quotaPercent.text = fillPercentage.ToString() + "%";

            yield return null;
        }

        quotaBarUI.fillAmount = newQuota / maxQuota;

        //Debug.Log("fillAmount: " + quotaBar.fillAmount);
    }

    public void StartBattle()
    {
        notifCharge = 0;
        ChargeNotifBar(0);
    }
    
    public void FullReset()
    {
        notifCharge = 0;
        currentQuota = 0;
        ChargeNotifBar(0);
        ChargeQuota(0);
    }
}
