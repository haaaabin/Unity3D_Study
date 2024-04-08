using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public Vector2 TouchDist;   //현재 터치 입력의 거리
    [HideInInspector]
    public Vector2 PointerOld;  //이전 터치 입력 위치
    [HideInInspector]
    protected int PointerId;    //현재 터치 입력에 대한 포인터ID
    [HideInInspector]
    public bool Pressed;    //현재 터치가 눌려있는지 여부

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Pressed)
        {   
            /* PointerId가 유효한 경우(Input.touches 배열 길이 내에 있는 경우)
               실제 터치 위치와 이전 터치 위치의 차이를 계산하여 TouchDist 업데이트 */
            if (PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
            }
            else
            {
                TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - PointerOld;
                PointerOld = Input.mousePosition;
            }
        }
        else
        {
            TouchDist = new Vector2();  // (0,0)으로 설정
        }
    }

    //터치 입력이 감지되면 호출
    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;

        print("PointerID: " + PointerId);
        print("PointerOld: " + PointerOld);

    }

    //터치 입력이 떼어지면 호출
    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}
