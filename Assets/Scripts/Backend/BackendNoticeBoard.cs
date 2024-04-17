using BackEnd;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class BackendNoticeBoard : MonoBehaviour
{
    public TMP_InputField[] input_post;
    const byte TITLE_INDEX = 0;
    const byte CONTENT_INDEX = 1;

    public GameObject post_Prefab;
    public Transform post_spawnPoint;
    public GameObject post_Content;
    public bool isMyPost = false;

    [System.Serializable]
    public struct PostData
    {
        public string title;
        public string content;
        public string inDate;
        public string id;
    }

    //게시글 정보를 저장할 구조체 리스트
    public List<PostData> postDataList = new List<PostData>();
    
    //게시글 정보를 저장할 딕셔너리
    public Dictionary<string, GameObject> postObjectDic = new Dictionary<string, GameObject>();

    private static BackendNoticeBoard instance;

    void Awake()
    {
        instance = this;
    }

    public static BackendNoticeBoard Instance()
    {
        if (instance == null)
        {
            Debug.LogError("BackendNoticeBoard 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    void Start()
    {
        AddPostList();
    }

    public void WritePost()
    {
        string id = BackendServerManager.Instance().GetId();
        string date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        int like = 0;
        string title = input_post[TITLE_INDEX].text;
        string content = input_post[CONTENT_INDEX].text;

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
        {
            Debug.LogError("제목 또는 내용이 비어있습니다.");
            InGameUI.Instance().ShowPopUp("제목 또는 내용이 비어있습니다.");
            return;
        }
        Param param = new Param
        {
            { "date", date },
            { "like", like },
            { "title", title },
            { "content", content },
            { "id", id }
        };
        var bro = Backend.GameData.Insert("notice_table", param);
        if (bro.IsSuccess())
        {
            Debug.Log("게시글 작성 성공");
            GetPost();
        }
        else
        {
            Debug.LogError("게시글 작성 실패");
        }

        InGameUI.Instance().TogglePanelWrite();
    }


    public void GetPost()
    {
        Debug.Log("게시글 조회 함수를 호출합니다.");

        BackendReturnObject bro;
        if(isMyPost)
        {
            bro = Backend.GameData.GetMyData("notice_table", new Where());
        }
        else
        {
            bro = Backend.GameData.Get("notice_table", new Where());

        }
        if (bro.IsSuccess())
        {
            Debug.Log("게시글 조회에 성공했습니다. : " + bro);

            LitJson.JsonData postDataJson = bro.FlattenRows();

            if (postDataJson.Count <= 0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
                InGameUI.Instance().ShowPopUp("작성한 게시글이 없습니다.");
            }
            else
            {
                // 기존의 게시글 데이터를 모두 삭제
                postDataList.Clear();

                for (int i = 0; i < postDataJson.Count; i++)
                {
                    string title = postDataJson[i]["title"].ToString();
                    string content = postDataJson[i]["content"].ToString();
                    string inDate = postDataJson[i]["inDate"].ToString();
                    string id = postDataJson[i]["id"].ToString();

                    // 게시글 데이터를 저장할 구조체를 생성하여 리스트에 추가
                    PostData postData = new PostData();
                    postData.title = title;
                    postData.content = content;
                    postData.inDate = inDate;
                    postData.id = id;
                    postDataList.Add(postData);

                }
                AddPostList();
            }
        }
        else
        {
            Debug.LogError("게시글 조회 실패 : " + bro.GetErrorCode());
        }

    }

    private void AddPostList()
    {
        //기존의 게시글 UI 요소를 모두 삭제
        foreach (GameObject postObject in postObjectDic.Values)
        {
            Destroy(postObject);
        }

        postObjectDic.Clear();

        //새로운 게시글 데이터를 이용하여 UI 요소 생성
        foreach (PostData postData in postDataList)
        {
            GameObject postObject = Instantiate(post_Prefab, post_spawnPoint);

            //LoginUI에서 사용하는 ID를 가져와서 게시글에 표시
            string loginId = BackendServerManager.Instance().GetId();

            postObject.transform.Find("Text_Info/ID").GetComponent<TextMeshProUGUI>().text = postData.id;
            postObject.transform.Find("Text_Info/Title").GetComponent<TextMeshProUGUI>().text = postData.title;
            postObject.transform.Find("Text_Info/Content").GetComponent<TextMeshProUGUI>().text = postData.content;

            // 중복 키 방지
            if (!postObjectDic.ContainsKey(postData.inDate))
            {
                //딕셔너리에 게시글 데이터와 게시글 UI 요소를 추가
                postObjectDic.Add(postData.inDate, postObject);
            }

            Button button = postObject.GetComponent<Button>();
            if (button != null)
            {
                int index = postDataList.IndexOf(postData);     //해당 게시글의 인덱스를 저장
                button.onClick.AddListener(() => ShowPostPanel(postData.inDate));
            }

            postObject.transform.Find("DeleteBtn").gameObject.SetActive(isMyPost);
            Button delete_btn = postObject.transform.Find("DeleteBtn").GetComponent<Button>();
            if (delete_btn != null)
            {
                delete_btn.onClick.AddListener(() => DeletePost(postData.inDate));
            }

        }

    }

    //게시글 상세보기 
    public void ShowPostPanel(string inDate)
    {
        //인덱스가 유효한지 확인
        if (!postObjectDic.ContainsKey(inDate))
        {
            Debug.LogError("해당 inDate의 게시글이 존재하지 않습니다.");
            return;
        }

        InGameUI.Instance().TogglePost();
        
        foreach (PostData postData in postDataList)
        {
            //해당 인덱스의 게시글 데이터를 이용하여 상세보기 UI에 정보를 표시
            TextMeshProUGUI show_title = post_Content.transform.Find("InputFieldGroup/Title_Panel/Title").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI show_content = post_Content.transform.Find("InputFieldGroup/Content_Panel/Content").GetComponent<TextMeshProUGUI>();

            if (postData.inDate == inDate)
            {
                show_title.text = postData.title;
                show_content.text = postData.content;
            }
        }
    }
    public void DeletePost(string inDate)
    {
        // 게시글 삭제 요청 보내기
        var bro = Backend.GameData.DeleteV2("notice_table", inDate, Backend.UserInDate);
        if (bro.IsSuccess())
        {
            Destroy(postObjectDic[inDate].gameObject);
            postObjectDic.Remove(inDate);

            //리스트에서도 삭제
            postDataList.RemoveAll(post => post.inDate == inDate);
            AddPostList();
            
            Debug.Log("게시글 삭제 성공");
        }
        else
        {
            Debug.LogError("게시글 삭제 실패 : " + bro.GetErrorCode());
        }

    }
}
