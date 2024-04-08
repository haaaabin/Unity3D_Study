using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IngameManager : MonoBehaviour
{
    public GameObject boy, girl;
    public MainCamera mainCamera;
    public Button btn_back;

    void Start()
    {
        if (PlayerInfo.Instance.GetPlayerType() == PlayerType.Boy)
        {
            boy.SetActive(true);
            mainCamera.target = boy.transform;
        }
        else if (PlayerInfo.Instance.GetPlayerType() == PlayerType.Girl)
        {
            girl.SetActive(true);
            mainCamera.target = girl.transform;
        }
        btn_back.onClick.AddListener(LoadSelectScene);
    }
    void LoadSelectScene()
    {
        SceneManager.LoadScene("SelectScene");
    }
}
