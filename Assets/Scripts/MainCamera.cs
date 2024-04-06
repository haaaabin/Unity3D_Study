using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public float Yaxis;
    public float Xaxis;

    public Transform target;

    public float rotSensitive = 10f;//카메라 회전 감도
    public float RotationMin = -10f;//카메라 회전각도 최소
    public float RotationMax = 80f;//카메라 회전각도 최대
    public float smoothTime = 0.12f;//카메라가 회전하는데 걸리는 시간
 
    private Vector3 targetRotation;
    private Vector3 currentVel;

    public bool enableMobile = false;

    public FixedTouchField touchField;

    public float offsetX;
    public float offsetY;
    public float offsetZ;
    void LateUpdate()
    {
        if (enableMobile)
        {
            Yaxis = Yaxis + touchField.TouchDist.x * rotSensitive;
            Xaxis = Xaxis - touchField.TouchDist.y * rotSensitive;
        }
        else
        {
            Yaxis = Yaxis + Input.GetAxis("Mouse X") * rotSensitive;
            Xaxis = Xaxis - Input.GetAxis("Mouse Y") * rotSensitive;
        }

        //Xaxis는 마우스를 아래로 했을때(음수값이 입력 받아질때) 값이 더해져야 카메라가 아래로 회전한다 

        Xaxis = Mathf.Clamp(Xaxis, RotationMin, RotationMax);
        //X축회전이 한계치를 넘지않게 제한해준다.

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(Xaxis, Yaxis), ref currentVel, smoothTime);
        transform.eulerAngles = targetRotation;
        //SmoothDamp를 통해 부드러운 카메라 회전

        Vector3 FixPedPos = new Vector3(target.transform.position.x + offsetX,
                                        target.transform.position.y + offsetY,
                                        target.transform.position.z + offsetZ);
        transform.position = FixPedPos;
        //transform.position = target.position - transform.forward * dis;
        //카메라의 위치는 플레이어보다 설정한 값만큼 떨어져있게 계속 변경된다.
    }
}
