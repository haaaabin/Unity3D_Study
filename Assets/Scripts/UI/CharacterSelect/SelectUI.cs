using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectUI : MonoBehaviour
{
    public GameObject popup_object;
    public TMP_InputField input_nickname;
    void Start()
    {
        popup_object.SetActive(false);
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
    public void ClickGameStart()
    {
        if (string.IsNullOrEmpty(input_nickname.text))
        {
            Debug.Log("닉네임을 입력해주세요.");
            return;
        }
        PlayerInfo.Instance.Nickname = input_nickname.text;
        bool isSuccess = BackendManager.Instance().UpdateNickname(PlayerInfo.Instance.Nickname);
        if (isSuccess)
        {
            SceneManager.LoadScene("3. InGameJJM");
        }
    }
}
