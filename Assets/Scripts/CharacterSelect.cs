using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public Button char1, char2;
    void Start()
    {
        char1.onClick.AddListener(SelectBoyCharacter);
        char2.onClick.AddListener(SelectGirlCharacter);
    }
    void SelectBoyCharacter() {
        Debug.Log("캐릭터 1을 선택합니다.");
        PlayerInfo.Instance.SetPlayerType(PlayerType.Boy);
        SceneManager.LoadScene("IngameSceneJJM");
    }
    void SelectGirlCharacter() {
        Debug.Log("캐릭터 2을 선택합니다.");
        PlayerInfo.Instance.SetPlayerType(PlayerType.Girl);
        SceneManager.LoadScene("IngameSceneJJM");
    }
}
