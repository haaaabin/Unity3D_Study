using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool enableMobile = false;
    
    [SerializeField]
    private float smoothRotationTime;   //target 각도로 회전하는데 걸리는 시간
    [SerializeField]
    private float smoothMoveTime;   //target 속도로 바뀌는데 걸리는 시간
    [SerializeField]
    private float moveSpeed;
    private float rotationVelocity;
    private float speedVelocity;
    private float currentSpeed;
    private float targetSpeed;

    Transform cameraTransform;
    public VariableJoystick joystick;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        if (enableMobile)
        {
            input = new Vector2(joystick.input.x, joystick.input.y);
        }
        else
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }

        anim.SetBool("IsWalk", input != Vector2.zero);

        Vector2 inputDir = input.normalized;

        //움직임을 멈췄을 때 다시 처음 각도로 돌아가는 걸 막기 위함
        if (inputDir != Vector2.zero)
        {
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref rotationVelocity, smoothRotationTime);
        }
        
        //입력 방향에 따른 목표 속도
        targetSpeed = moveSpeed * inputDir.magnitude;
        //현재 속도를 목표 속도로 부드럽게 조절
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);
        //현재 스피드에서 타겟 스피드까지 smoothMoveTime 동안 변한다.
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        
    }
}
