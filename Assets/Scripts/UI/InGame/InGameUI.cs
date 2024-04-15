using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    private static InGameUI instance;

    public GameObject popup_btn_noticeboard, panel_noticeboard;
    public GameObject panel_write, panel_openPost;

    public Button btn_write, btn_allPost, btn_myPost, btn_close;

    public TMP_InputField[] input_post;
    private const byte TITLE_INDEX = 0, CONTENT_INDEX = 1;

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

    void Start()
    {
        popup_btn_noticeboard.SetActive(false);
        panel_noticeboard.SetActive(false);
        panel_write.SetActive(false);
        panel_openPost.SetActive(false);
    }

    // 버튼 사용 가능 여부 설정
    public void SetButtonInteractable(bool interactable)
    {
        btn_write.interactable = interactable;
        btn_allPost.interactable = interactable;
        btn_myPost.interactable = interactable;
        btn_close.interactable = interactable;
    }

    // 게시판 열기 버튼 On/Off
    public void ToggleNoticeBoardButton()
    {
        popup_btn_noticeboard.SetActive(!popup_btn_noticeboard.activeSelf);
    }
    

    // 게시판 On/Off
    public void ToggleNoticeBoard()
    {
        panel_noticeboard.SetActive(!panel_noticeboard.activeSelf);
        if (panel_noticeboard.activeSelf)
        {
            AllPost();
        }
    }

    // 쓰기 패널 On/Off
    public void TogglePanelWrite()
    {
        input_post[TITLE_INDEX].text = "";
        input_post[CONTENT_INDEX].text = "";
        panel_write.SetActive(!panel_write.activeSelf);
        SetButtonInteractable(!panel_write.activeSelf);
    }
    // 게시글 On/Off
    public void TogglePost()
    {
        panel_openPost.SetActive(!panel_openPost.activeSelf);
        SetButtonInteractable(!panel_openPost.activeSelf);
    }

    // 게시글 쓰기
    public void Write()
    {
        BackendNoticeBoard.Instance().WritePost();
        TogglePanelWrite();
    }

    // 모든 게시글 불러오기
    public void AllPost()
    {
        BackendNoticeBoard.Instance().isMyPost = false;
        BackendNoticeBoard.Instance().GetPost();
    }

    // 내 게시글 불러오기
    public void MyPost()
    {
        BackendNoticeBoard.Instance().isMyPost = true;
        BackendNoticeBoard.Instance().GetPost();
    }
}