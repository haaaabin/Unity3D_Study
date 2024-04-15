using Protocol;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public VariableJoystick joystick;

    void Start()
    {
        GameManager.InGame += MobileInput;        
    }

    void MobileInput()
    {
        if (!joystick)
        {
            return;
        }

        int keyCode = 0;

        keyCode |= KeyEventCode.MOVE;
        Vector3 moveVector = new Vector3(joystick.input.x, 0, joystick.input.y);
        moveVector = Vector3.Normalize(moveVector);


        if (keyCode <= 0)
        {
            return;
        }

        KeyMessage msg = new KeyMessage(keyCode, moveVector);
        if (BackendMatchManager.Instance().IsHost())
        {
            BackendMatchManager.Instance().AddMsgToLocalQueue(msg);
        }
        else
        {
            BackendMatchManager.Instance().SendDataToInGame<KeyMessage>(msg);
        }
    }
}
