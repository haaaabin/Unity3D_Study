using UnityEngine;
using System.Threading.Tasks; // [변경] async 기능을 이용하기 위해서는 해당 namepsace가 필요합니다.  

// 뒤끝 SDK namespace 추가
using BackEnd;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackendManager : MonoBehaviour {

    public TMP_InputField id_inputfield, pw_inputfield;
    public Button login_button, load_signin_button, signin_button, load_login_button;

    void Start() {
        var bro = Backend.Initialize(true); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if(bro.IsSuccess()) {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        } else {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }
        if (login_button != null) {
            login_button.onClick.AddListener(Login);
        }
        if (signin_button != null) {
            signin_button.onClick.AddListener(Signin);
        }
        if (load_login_button != null) {
            load_login_button.onClick.AddListener(LoadLoginScene);
        }
        if (load_signin_button != null) {
            load_signin_button.onClick.AddListener(LoadSigninScene);
        }
    }
    void LoadLoginScene() {
        SceneManager.LoadScene("LoginScene");
    }
    void LoadSigninScene() {
        SceneManager.LoadScene("SigninScene");
    }

    // =======================================================
    // [추가] 동기 함수를 비동기에서 호출하게 해주는 함수(유니티 UI 접근 불가)
    // =======================================================
    async void Login() {
        await Task.Run(() => {
            BackendLogin.Instance.CustomLogin(id_inputfield.text, pw_inputfield.text); // [추가] 뒤끝 로그인
            id_inputfield.text = "";
            pw_inputfield.text = "";
        });
    }

    async void Signin() {
        await Task.Run(() => {
            BackendLogin.Instance.CustomSignUp(id_inputfield.text, pw_inputfield.text); // [추가] 뒤끝 회원가입 함수
            id_inputfield.text = "";
            pw_inputfield.text = "";
        });
    }
}