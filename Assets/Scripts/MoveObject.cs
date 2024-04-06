using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public Transform moving_object;
    public Transform camPivot;
    Joystick controller;

    public float speed = 2f;

    // Start is called before the first frame update
    void Awake()
    {
        controller = GetComponent<Joystick>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 conDir = controller.Direction;
        if (conDir == Vector2.zero) return;

        float thetaEuler = Mathf.Acos(conDir.y / conDir.magnitude) * (180 / Mathf.PI) * Mathf.Sign(conDir.x);

        Vector3 moveAngle = Vector3.up * (camPivot.transform.rotation.eulerAngles.y + thetaEuler);
        moving_object.rotation = Quaternion.Euler(moveAngle);
        moving_object.Translate(Vector3.forward * Time.fixedDeltaTime * speed);
    }
}
