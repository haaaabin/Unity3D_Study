using TMPro;
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public GameObject boy, girl, noticeboard;
    public Transform followcam_boy, followcam_girl;
    public MainCamera mainCamera;
    public TMP_Text nickname_boy, nickname_girl;

    void Start()
    {
        if (PlayerInfo.Instance.PlayerType == PlayerType.Boy)
        {
            boy.SetActive(true);
            mainCamera.target = followcam_boy;
            nickname_boy.text = PlayerInfo.Instance.Nickname;
            
        }
        else if (PlayerInfo.Instance.PlayerType == PlayerType.Girl)
        {
            girl.SetActive(true);
            mainCamera.target = followcam_girl;
            nickname_girl.text = PlayerInfo.Instance.Nickname;
        }
    }
}
