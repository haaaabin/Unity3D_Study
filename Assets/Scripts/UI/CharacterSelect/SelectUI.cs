using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectUI : MonoBehaviour
{
    private static SelectUI instance;
    public GameObject popup_object, select_object, loading_object, loading;
    public TMP_InputField input_nickname;
    public TMP_Text popup_description;

    void Awake()
    {
        instance = this;
    }

    public static SelectUI Instance()
    {
        if (instance == null)
        {
            Debug.LogError("SelectUI 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    void Start()
    {
        popup_object.SetActive(false);
        loading_object.SetActive(false);
        BackendMatchManager.Instance().GetMatchList();
        BackendMatchManager.Instance().JoinMatchMakingServer();
    }
    public void SelectBoyCharacter()
    {
        PlayerInfo.Instance.PlayerType = PlayerType.Boy;
        popup_object.SetActive(true);
    }
    public void SelectGirlCharacter()
    {
        PlayerInfo.Instance.PlayerType = PlayerType.Girl;
        popup_object.SetActive(true);
    }
    public void SetPopUpDescription(string description)
    {
        popup_description.text = description;
    }
    public void LoadingObject()
    {
        select_object.SetActive(false);
        loading_object.SetActive(true);
        loading.SetActive(true);
    }
    public void ClickGameStart()
    {
        if (string.IsNullOrEmpty(input_nickname.text))
        {
            Debug.Log("닉네임을 입력해주세요.");
            return;
        }
        PlayerInfo.Instance.Nickname = input_nickname.text;
        bool isSuccess = BackendServerManager.Instance().UpdateNickname(PlayerInfo.Instance.Nickname);
        if (isSuccess)
        {
            DoJoinMatchserver();
        }
    }
    public void DoJoinMatchserver()
    {
        LoadingObject();
        BackendMatchManager.Instance().CreateMatchRoom();
        BackendMatchManager.Instance().RequestMatchMaking(0);
    }

    public void DoLeaveMatchserver()
    {
       BackendMatchManager.Instance().LeaveMatchRoom();
       BackendMatchManager.Instance().LeaveInGameRoom();
    }
}