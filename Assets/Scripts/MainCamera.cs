using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public Transform target;
    public float followspeed = 15f; //따라가는 속도
    public float sensitive = 7f;  //감도
    public float clampAngle = 50f;  //카메라 회전 각도의 제한

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;   //방향벡터
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

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);  //각도의 범위
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);   //카메라의 회전
        transform.rotation = rot;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, followspeed * Time.deltaTime);

        //transform.TransformPoint() 로컬 좌표계에서 지정된 지점을 월드 좌표계로 변환
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;
        
        //카메라의 현재 위치와 최종 위치 사이의 장애물이 있는지 검사
        if(Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }

        //Lerp 함수를 사용하여 실제 카메라의 로컬 위치를 방향 벡터와 최종 거리에 맞게 부드럽게 이동
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
