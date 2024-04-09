using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameManager : MonoBehaviour
{
    public GameObject boy, girl, noticeboard;
    public Transform followcam_boy, followcam_girl;
    public MainCamera mainCamera;
    public Button btn_back, btn_noticeboard, btn_noticeboard_close;
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
        btn_back.onClick.AddListener(LoadSelectScene);
        btn_noticeboard.onClick.AddListener(ToggleNoticeBoard);
        btn_noticeboard_close.onClick.AddListener(ToggleNoticeBoard);
    }
    void LoadSelectScene()
    {
        SceneManager.LoadScene("SelectScene");
    }
    void ToggleNoticeBoard()
    {
        noticeboard.SetActive(!noticeboard.activeSelf);
    }
}
