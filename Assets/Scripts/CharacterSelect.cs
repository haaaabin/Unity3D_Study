using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using BackEnd;

public class CharacterSelect : MonoBehaviour
{
    public Button btn_character_boy, btn_character_girl, btn_gamestart;
    public GameObject popup_gamestart;
    public TMP_InputField input_nickname;
    void Start()
    {
        btn_character_boy.onClick.AddListener(SelectBoyCharacter);
        btn_character_girl.onClick.AddListener(SelectGirlCharacter);
        btn_gamestart.onClick.AddListener(ClickGameStart);
    }
    void SelectBoyCharacter() {
        PlayerInfo.Instance.PlayerType = PlayerType.Boy;
        popup_gamestart.SetActive(true);
    }
    void SelectGirlCharacter() {
        PlayerInfo.Instance.PlayerType = PlayerType.Girl;
        popup_gamestart.SetActive(true);
    }
    void ClickGameStart()
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
