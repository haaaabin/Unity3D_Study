using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool enableMobile = false;

    [SerializeField]
    private float smoothRotationTime;
    [SerializeField]
    private float smoothMoveTime;
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

        Vector3 input = Vector3.zero;

        if (enableMobile)
        {
            input = new Vector3(joystick.input.x, joystick.input.y,0);
        }
        else
        {
            input = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        }

        anim.SetBool("IsWalk", input != Vector3.zero);

        Vector2 inputDir = input.normalized;

        //캐릭터 회전
        if (inputDir != Vector2.zero)
        {
            float rotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref rotationVelocity, smoothRotationTime);
        }

        targetSpeed = moveSpeed * inputDir.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothMoveTime);

        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);
        
    }
    void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "NoticeBoard") {
            Debug.Log("NoticeBoard");
        }
    }
}
