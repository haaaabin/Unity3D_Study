using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;
    public float followspeed = 15f;
    public float sensitive = 7f;  //감도
    public float clampAngle = 50f;  

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;   //방향
    public Vector3 finalDir;    //최종 방향
    public float minDistance;   //최소 거리
    public float maxDistance;   //최대 거리
    public float finalDistance; //최종 거리
    public float smoothness = 10f;

    public FixedTouchField touchField;
    public bool enableMobile = false;

    void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;
    }

    void Update()
    {
        if (enableMobile)
        {
            rotY += touchField.TouchDist.x * sensitive * Time.deltaTime;
            rotX -= touchField.TouchDist.y * sensitive * Time.deltaTime;
        }
        else
        {
            rotY += Input.GetAxis("Mouse X") * sensitive * Time.deltaTime;
            rotX -= Input.GetAxis("Mouse Y") * sensitive * Time.deltaTime;
        }

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, followspeed * Time.deltaTime);

        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;
        
        if(Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }

        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
