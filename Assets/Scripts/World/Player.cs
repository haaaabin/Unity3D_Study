using BackEnd.Tcp;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    private SessionId index = 0;
    private string nickName = string.Empty;
    public PlayerType playerType = PlayerType.Boy;

    [SerializeField]
    private bool isMe = false;
    public GameObject nameObject;
    [field: SerializeField]
    public bool isMove { get; private set; }
    [field: SerializeField]
    public Vector3 moveVector { get; private set; }
    [field: SerializeField]
    public bool isRotate { get; private set; }
    [SerializeField]
    private float rotSpeed = 4.0f;
    [SerializeField]
    private float moveSpeed = 4.0f;

    private GameObject playerModelObject;

    public bool enableMobile = false;
    
    [SerializeField]
    private float smoothRotationTime;   //target 각도로 회전하는데 걸리는 시간
    [SerializeField]
    private float smoothMoveTime;   //target 속도로 바뀌는데 걸리는 시간
    private float rotationVelocity;

    Transform cameraTransform;
    public VariableJoystick joystick;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }
    void Start()
    {
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<VariableJoystick>();
        if (BackendMatchManager.Instance() == null)
        {
            // 매칭 인스턴스가 존재하지 않을 경우 (인게임 테스트 용도)
            Initialize(true, SessionId.None, "testPlayer", 0);
        }
    }

    void Update()
    {
        if (BackendMatchManager.Instance() == null)
        {
            // 매칭 인스턴스가 존재하지 않는 경우 (인게임 테스트 용도)
            Vector3 tmp = new Vector3(joystick.input.x, joystick.input.y, 0 );
            tmp = Vector3.Normalize(tmp);
            SetMoveVector(tmp);
        }
        if (isMove)
        {
            Move();
            anim.SetBool("IsWalk", true);
        }
        else
        {
            anim.SetBool("IsWalk", false);
        }

        if (isRotate)
        {
            Rotate();
        }

    }

    // 플레이어 초기화
    public void Initialize(bool isMe, SessionId index, string nickName, float rot)
    {
        this.isMe = isMe;
        this.index = index;
        this.nickName = nickName;

        playerModelObject = this.gameObject;
        playerModelObject.transform.rotation = Quaternion.Euler(0, rot, 0);

        nameObject = Instantiate(nameObject, Vector3.zero, Quaternion.identity, playerModelObject.transform);
        nameObject.GetComponent<TMP_Text>().text = this.nickName;
        nameObject.transform.position = GetNameUIPos();

        if (this.isMe)
        {
            Camera.main.GetComponentInParent<MainCamera>().target = this.transform;
        }

        this.isMove = false;
        this.moveVector = new Vector3(0, 0, 0);
        this.isRotate = false;

    }
    
    #region 이동관련 함수
    /*
     * 변화량만큼 이동
     * 특정 좌표로 이동
     */
   
    public void SetMoveVector(float move)
    {
        SetMoveVector(this.transform.forward * move);
    }
    public void SetMoveVector(Vector3 vector)
    {
        moveVector = vector;

        if (vector == Vector3.zero)
        {
            isMove = false;
        }
        else
        {
            isMove = true;
        }
    }

    public void Move()
    {
        Move(moveVector);
    }
    public void Move(Vector3 var)
    {
        // 회전
        if (var.Equals(Vector3.zero))
        {
            isRotate = false;
        }
        else
        {
            if (Quaternion.Angle(playerModelObject.transform.rotation, Quaternion.LookRotation(var)) > Quaternion.kEpsilon)
            {
                isRotate = true;
            }
            else
            {
                isRotate = false;
            }
        }

        // 움직임을 멈췄을 때 다시 처음 각도로 돌아가는 걸 막기 위함
        if (var != Vector3.zero)
        {
            float rotation = Mathf.Atan2(var.x, var.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotation, ref rotationVelocity, smoothRotationTime);
        }
        // 이동
        var pos = gameObject.transform.position + playerModelObject.transform.forward * moveSpeed * Time.deltaTime;
        SetPosition(pos);
    }

    public void Rotate()
    {
        if (moveVector.Equals(Vector3.zero))
        {
            isRotate = false;
            return;
        }
        if (Quaternion.Angle(playerModelObject.transform.rotation, Quaternion.LookRotation(moveVector)) < Quaternion.kEpsilon)
        {
            isRotate = false;
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(moveVector, Vector3.up);
        
        playerModelObject.transform.rotation = Quaternion.Slerp(playerModelObject.transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
    }

    public void SetPosition(Vector3 pos)
    {
        gameObject.transform.position = pos;
    }

    // isStatic이 true이면 해당 위치로 바로 이동
    public void SetPosition(float x, float y, float z)
    {
        Vector3 pos = new Vector3(x, y, z);
        SetPosition(pos);
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    public Vector3 GetRotation()
    {
        return gameObject.transform.rotation.eulerAngles;
    }
    #endregion
    Vector3 GetNameUIPos()
    {
        return this.transform.position + (Vector3.up * 2.0f);
    }

}
