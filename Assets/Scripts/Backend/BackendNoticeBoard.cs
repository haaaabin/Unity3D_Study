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
    public Transform myPost_spwanPoint;
    public Transform post_spawnPoint;
    public GameObject post_Content;


    [System.Serializable]
    public struct PostData
    {
        public string nickname;
        public string title;
        public string content;
    }

    //게시글 정보를 저장할 구조체 리스트
    public List<PostData> postDataList = new List<PostData>();

    private static BackendNoticeBoard instance;

    public object QueryOperator { get; private set; }

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

    }

    public void WritePost()
    {
        string nickname = Backend.UserNickName;
        string date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        int like = 0;
        string title = input_post[TITLE_INDEX].text;
        string content = input_post[CONTENT_INDEX].text;


        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content))
        {
            Debug.LogError("제목 또는 내용이 비어있습니다.");
            return;
        }
        Param param = new Param
        {
            { "nickname", nickname },
            { "date", date },
            { "like", like },
            { "title", title },
            { "content", content },
        };
        var bro = Backend.GameData.Insert("notice_table", param);
        if (bro.IsSuccess())
        {
            Debug.Log("게시글 작성 성공");
            PostGet();
        }
        else
        {
            Debug.LogError("게시글 작성 실패");
        }
    }

    public void PostGet()
    {
        Debug.Log("게시글 조회 함수를 호출합니다.");

        var bro = Backend.GameData.GetMyData("notice_table", new Where());
        if(bro.IsSuccess())
        {
            Debug.Log("게시글 조회에 성공했습니다. : " + bro);

            LitJson.JsonData postDataJson = bro.FlattenRows();

            if(postDataJson.Count <= 0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
            }
            else
            {
                //기존에 불러온 게시글 데이터를 비운다.
                postDataList.Clear();

                for(int i = 0; i< postDataJson.Count; i++)
                {
                    string nickname = postDataJson[i]["nickname"].ToString();
                    string title = postDataJson[i]["title"].ToString();
                    string content = postDataJson[i]["content"].ToString();
                    //string date = postDataJson[i]["date"].ToString(); 

                    // 새로운 게시글 데이터를 리스트에 추가
                    PostData postData = new PostData();
                    postData.nickname = nickname;
                    postData.title = title;
                    postData.content = content;
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
        // 기존에 생성된 게시글 UI 요소들을 제거한다.
        foreach (Transform child in post_spawnPoint)
        {
            Destroy(child.gameObject);
        }

        //새로운 게시글 데이터를 이용하여 UI 요소 생성
        foreach(PostData postData in postDataList)
        {
            GameObject postObject = Instantiate(post_Prefab, post_spawnPoint);

            postObject.transform.Find("Text_Info/NickName").GetComponent<TextMeshProUGUI>().text = postData.nickname;
            postObject.transform.Find("Text_Info/Title").GetComponent<TextMeshProUGUI>().text = postData.title;
            postObject.transform.Find("Text_Info/Info").GetComponent<TextMeshProUGUI>().text = postData.content;

            Button button = postObject.GetComponent<Button>();

            if (button != null)
            {
                int index = postDataList.IndexOf(postData);     //해당 게시글의 인덱스를 저장
                button.onClick.AddListener(() => ShowPost_Panel(index));
            }
        }
    }

    //게시글 상세보기 
    public void ShowPost_Panel(int index)
    {
        InGameUI.Instance().ShowPost();

        TextMeshProUGUI show_title = post_Content.transform.Find("InputFieldGroup/Title_Panel/Title").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI show_content = post_Content.transform.Find("InputFieldGroup/Content_Panel/Content").GetComponent<TextMeshProUGUI>();

        show_title.text = postDataList[index].title;
        show_content.text = postDataList[index].content;

    }

    //public void MyPostShow()
    //{
       
    //}

    //public void DeletePost()
    //{

    //    //Backend.PlayerData.DeleteMyData("notice_table", )
    //}
}
