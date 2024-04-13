using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameUI : MonoBehaviour
{
    private static InGameUI instance;
    void Awake()
    {
        instance = this;
    }

    public static InGameUI Instance()
    {
        if (instance == null)
        {
            Debug.LogError("InGameUI 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    public GameObject btn_noticeBoard_object;
    public GameObject noticeBoard_object;
    public GameObject noticeBoard_write_object;
    public GameObject ShowPost_Panel;
    public GameObject btn_delete;

    public TMP_InputField[] input_post;
    private const byte TITLE_INDEX = 0, CONTENT_INDEX = 1;

    void Start()
    {
        btn_noticeBoard_object.SetActive(false);
        noticeBoard_write_object.SetActive(false);
        noticeBoard_object.SetActive(false);
        ShowPost_Panel.SetActive(false);
    }
    public void ToggleNoticeBoardButton()
    {
        btn_noticeBoard_object.SetActive(!btn_noticeBoard_object.activeSelf);
    }
    public void ToggleNoticeBoard()
    {
        noticeBoard_object.SetActive(true);
        BackendNoticeBoard.Instance().isMyPost = false;
        BackendNoticeBoard.Instance().GetPost();
    }
    public void ToggleNoticeBoardWrite()
    {
        noticeBoard_write_object.SetActive(!noticeBoard_write_object.activeSelf);
    }
    public void ToggleNoticeBoardMyPostButton()
    {
        BackendNoticeBoard.Instance().isMyPost = true;
        BackendNoticeBoard.Instance().GetPost();
    }
    public void ShowPost()
    {
        ShowPost_Panel.SetActive(true);
    }
    public void ClosePost()
    {
        ShowPost_Panel.SetActive(false);
    }

    public void CloseNoticeBoard()
    {
        noticeBoard_object.SetActive(false);
    }

    public void ClearPostingText()
    {
        input_post[TITLE_INDEX].text = "";
        input_post[CONTENT_INDEX].text = "";
    }

    public void LoadSelectScene()
    {
        SceneManager.LoadScene("1. Select");
    }
}
