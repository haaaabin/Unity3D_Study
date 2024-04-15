using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    private static InGameUI instance;

    public GameObject btn_noticeBoard_object;
    public GameObject noticeBoard_object;
    public GameObject noticeBoard_write_object;
    public GameObject ShowPost_Panel;

    public Button writebtn;
    public Button allpostbtn;
    public Button mypostbtn;
    public Button closebtn;

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
        IntializeNoticeBoardUI();
    }

    // 게시판 UI 초기화
    public void IntializeNoticeBoardUI()
    {
        SetActiveNoticeBoardUI(false);
        SetActiveShowPostPanel(false);
    }

    // 게시판 UI 활성화
    public void SetActiveNoticeBoardUI(bool active)
    {
        btn_noticeBoard_object.SetActive(active);
        noticeBoard_object.SetActive(active);
        noticeBoard_write_object.SetActive(active);
    }

    // 게시판 게시글 보기 패널 활성화
    public void SetActiveShowPostPanel(bool active)
    {
        ShowPost_Panel.SetActive(active);
    }

    // 버튼 사용 가능 여부 설정
    public void SetButtonInteractable(bool interactable)
    {
        writebtn.interactable = interactable;
        allpostbtn.interactable = interactable;
        mypostbtn.interactable = interactable;
        closebtn.interactable = interactable;
    }

    //게시판 토글 버튼
    public void ToggleNoticeBoardButton()
    {
        btn_noticeBoard_object.SetActive(!btn_noticeBoard_object.activeSelf);
    }
    
    //게시판 열기
    public void ToggleNoticeBoard()
    {
        noticeBoard_object.SetActive(true);
        BackendNoticeBoard.Instance().isMyPost = false;
        BackendNoticeBoard.Instance().GetPost();
    }

    //게시판 쓰기
    public void NoticeBoardWrite()
    {
        noticeBoard_write_object.SetActive(!noticeBoard_write_object.activeSelf);
        SetButtonInteractable(!noticeBoard_write_object.activeSelf);

    }

    //게시판 내 게시글 보기
    public void NoticeBoardMyPostButton()
    {
        BackendNoticeBoard.Instance().isMyPost = true;
        BackendNoticeBoard.Instance().GetPost();
    }

    //게시판 상세 보기
    public void ShowPost()
    {
        SetActiveShowPostPanel(true);
        SetButtonInteractable(false);
    }

    public void ClosePost()
    {
        SetActiveShowPostPanel(false);
        SetButtonInteractable(true);
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
