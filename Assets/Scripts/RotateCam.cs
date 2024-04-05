using UnityEngine;
using UnityEngine.EventSystems;

public class RotateCam : MonoBehaviour, IBeginDragHandler, IDragHandler
{

    public Transform camPivot;
    public float rotationSpeed = 0.4f;

    Vector3 beginPos;
    Vector3 draggingPos;
    float xAngle;
    float yAngle;
    float xAngleTemp;
    float yAngleTemp;

    void Start()
    {
        xAngle = camPivot.rotation.eulerAngles.x;
        yAngle = camPivot.rotation.eulerAngles.y;
    }

    public void OnBeginDrag(PointerEventData beginPoint)
    {
        beginPos = beginPoint.position;

        xAngleTemp = xAngle;
        yAngleTemp = yAngle;
    }

    public void OnDrag(PointerEventData draggingPoint)
    {
        draggingPos = draggingPoint.position;

        yAngle = yAngleTemp + (draggingPos.x - beginPos.x) * Screen.height / 1080 * rotationSpeed * Time.deltaTime;
        xAngle = xAngleTemp - (draggingPos.y - beginPos.y) * Screen.width / 2340 * rotationSpeed * Time.deltaTime;

        if (xAngle > 30) xAngle = 30;
        if (xAngle < -60) xAngle = -60;

        camPivot.rotation = Quaternion.Euler(xAngle, yAngle, 0.0f);
    }
}