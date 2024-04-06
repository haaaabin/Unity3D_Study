using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    public Button char1, char2;
    void Start()
    {
        char1.onClick.AddListener(SelectChar1);
        char2.onClick.AddListener(SelectChar2);
    }
    void SelectChar1() {
        Debug.Log("캐릭터 1을 선택합니다.");
        SceneManager.LoadScene("SampleScene");
    }
    void SelectChar2() {
        Debug.Log("캐릭터 2을 선택합니다.");
        SceneManager.LoadScene("SampleScene");
    }
}
