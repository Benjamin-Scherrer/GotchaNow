using UnityEngine;
using System.Collections;
using Unity.Cinemachine;

public class LockOnCamera : MonoBehaviour
{
    public BattleManager bm;
    public GameObject player;
    public GameObject target;
    private Vector3 targetVector;
    private Vector3 camPos;
    public GameObject freeCam;
    [HideInInspector] public bool isActive = false;

    void FixedUpdate()
    {
        targetVector = target.transform.position - player.transform.position;

        camPos = player.transform.position - new Vector3(targetVector.normalized.x, 0f, targetVector.normalized.z) * 8 + new Vector3(0, 2f, 0);
        transform.position = Vector3.Lerp(transform.position, camPos, 0.1f);

        if (isActive)
        {
            //freeCam.GetComponent<CinemachineOrbitalFollow>().ForceCameraPosition(transform.position,transform.rotation);


        }
    }

    public void SetTarget()
    {
        float closestAngle = 999;

        for (int i = 0; i < bm.activeEnemy.Count; i++)
        {
            Vector3 camRay = freeCam.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f,0.5f,0)).direction;
            Vector3 newTargetCheck = bm.activeEnemy[i].transform.position - player.transform.position;

            float angle = Vector3.Angle(camRay, newTargetCheck);

            if (angle < closestAngle)
            {
                closestAngle = angle;
                target.GetComponent<LockOnTarget>().targetEnemy = bm.activeEnemy[i];
            }
        }
    }

    public void SwitchTargetR()
    {
        float closestAngle = 999;
        Vector3 currentTargetPos = target.GetComponent<LockOnTarget>().targetEnemy.transform.position;

        for (int i = 0; i < bm.activeEnemy.Count; i++)
        {
            if (target.GetComponent<LockOnTarget>().targetEnemy != bm.activeEnemy[i])
            {
                Vector3 newTargetCheck = bm.activeEnemy[i].transform.position - player.transform.position;

                float dirCheck = Vector3.Cross(currentTargetPos-player.transform.position, newTargetCheck).y;
                float angle = Vector3.Angle(currentTargetPos-player.transform.position, newTargetCheck);

                if (dirCheck > 0 && angle < closestAngle)
                {
                    closestAngle = angle;
                    target.GetComponent<LockOnTarget>().targetEnemy = bm.activeEnemy[i];
                }

                Debug.Log(dirCheck + " " + i);
            }
        }
    }

    public void SwitchTargetL()
    {
        float closestAngle = 999;
        Vector3 currentTargetPos = target.GetComponent<LockOnTarget>().targetEnemy.transform.position;

        for (int i = 0; i < bm.activeEnemy.Count; i++)
        {
            if (target.GetComponent<LockOnTarget>().targetEnemy != bm.activeEnemy[i])
            {
                Vector3 newTargetCheck = bm.activeEnemy[i].transform.position - player.transform.position;
                float dirCheck = Vector3.Cross(currentTargetPos-player.transform.position, newTargetCheck).y;
                float angle = Vector3.Angle(currentTargetPos-player.transform.position, newTargetCheck);

                if (dirCheck < 0 && angle < closestAngle)
                {
                    closestAngle = angle;
                    target.GetComponent<LockOnTarget>().targetEnemy = bm.activeEnemy[i];
                }
            }
        }
    }
}
