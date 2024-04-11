using UnityEngine;
using System.Threading.Tasks; // async  
using BackEnd; // 뒤끝 SDK namespace
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackendManager : MonoBehaviour {

    public TMP_InputField ID, PW;
    public Button btn_login, btn_load_signin, btn_signin, btn_load_login;
    bool isLogin = false;
    bool isSignUp = false;

    void Start() {
        var bro = Backend.Initialize(true); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if(bro.IsSuccess()) {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        } else {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }
        if (btn_login != null) {
            btn_login.onClick.AddListener(DoLogin);
            btn_login.onClick.AddListener(CleanIDPW);
        }
        if (btn_signin != null) {
            btn_signin.onClick.AddListener(DoSignUp);
            btn_signin.onClick.AddListener(CleanIDPW);
        }
        if (btn_load_login != null) {
            btn_load_login.onClick.AddListener(LoadLoginScene);
        }
        if (btn_load_signin != null) {
            btn_load_signin.onClick.AddListener(LoadSigninScene);
        }
    }
    void Update() {
        if (isLogin) {
            isLogin = false;
            LoadSelectScene();
        }
        if (isSignUp) {
            isSignUp = false;
            LoadLoginScene();
        }
    }
    void LoadSelectScene() {
        SceneManager.LoadScene("2. Select");
    }
    void LoadLoginScene() {
        SceneManager.LoadScene("0. Login");
    }
    void LoadSigninScene() {
        SceneManager.LoadScene("1. Signin");
    }
    void CleanIDPW() {
        ID.text = "";
        PW.text = "";
    }

    // 동기 함수를 비동기에서 호출하게 해주는 함수(유니티 UI 접근 불가)
    async void DoLogin() {
        await Task.Run(() => {
            isLogin = BackendLogin.Instance.CustomLogin(ID.text, PW.text);
        });
    }

    async void DoSignUp() {
        await Task.Run(() => {
            isSignUp = BackendLogin.Instance.CustomSignUp(ID.text, PW.text);
        });
    }

    async void DoUpdateNickname() {
        await Task.Run(() => {
            BackendLogin.Instance.UpdateNickname("원하는 이름");
        });
    }
}