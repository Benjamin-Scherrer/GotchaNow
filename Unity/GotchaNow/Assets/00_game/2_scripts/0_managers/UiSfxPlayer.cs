using FMODUnity;
using UnityEngine;

public class UiSfxPlayer : MonoBehaviour
{
    public static UiSfxPlayer instance;
    public EventReference notificationReady;
    public EventReference openNotifMenu;
    public EventReference closeNotifMenu;
    public EventReference confirm;
    public EventReference back;
    public EventReference scrollUp;
    public EventReference scrollDown;
    public EventReference fillQuota;
    public EventReference quotaFull;
    public EventReference phoneNotification;
    public EventReference phoneConfirm;
    public EventReference phoneMessage;
    public EventReference heal;
    public EventReference buffActivate;
    public EventReference buffDeactivate;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
