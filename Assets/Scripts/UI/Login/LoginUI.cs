using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* LoginUI.cs
 * 로그인 UI 관리
 * 로그인, 회원가입 버튼 오브젝트
 * 로그인, 회원가입 입력 필드
 */
public class LoginUI : MonoBehaviour
{
    private static LoginUI instance;
    void Awake()
    {
        instance = this;
    }

    public static LoginUI Instance()
    {
        if (instance == null)
        {
            Debug.LogError("LoginUI 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }
    void Start()
    {
        login_object.SetActive(true);
        signup_object.SetActive(false);
    }
    public GameObject login_object, signup_object, popup_object;
    public TMP_InputField[] inputs;
    public TMP_Text popup_text;
    private const byte ID_INDEX = 0, PW_INDEX = 1;
    void CleanIDPW()
    {
        inputs[ID_INDEX].text = "";
        inputs[PW_INDEX].text = "";
    }
    public void ClickLogin()
    {
        bool isSuccess = BackendServerManager.Instance().CustomLogin(inputs[ID_INDEX].text, inputs[PW_INDEX].text);
        if (isSuccess)
        {
            SceneManager.LoadScene("2. Select");
        }
        else
        {
            ShowPopUp("로그인 실패");
        }
    }
    public void ClickSignUp()
    {
        bool isSuccess = BackendServerManager.Instance().CustomSignUp(inputs[ID_INDEX].text, inputs[PW_INDEX].text);
        if (isSuccess)
        {
            ShowPopUp("회원가입 성공");
            ToggleLoginSignin();
        }
        else
        {
            ShowPopUp("회원가입 실패");
        }
    }
    public void ToggleLoginSignin()
    {
        CleanIDPW();
        login_object.SetActive(!login_object.activeSelf);
        signup_object.SetActive(!signup_object.activeSelf);
    }
    public void ShowPopUp(string text)
    {
        popup_object.SetActive(true);
        popup_text.text = text;
    }
    public void ClosePopUp()
    {
        popup_text.text = "";
        popup_object.SetActive(false);
    }
}
