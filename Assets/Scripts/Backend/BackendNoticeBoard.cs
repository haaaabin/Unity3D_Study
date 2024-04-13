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

    public bool isMyPost;

    [System.Serializable]
    public struct PostData
    {
        public string nickname;
        public string title;
        public string content;
        public string inDate;
    }

    //게시글 정보를 저장할 구조체 리스트
    public List<PostData> postDataList = new List<PostData>();
    
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
            //{ "gamer_id", id },
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
            }
            else
            {
                //새로운 게시글 데이터를 임시 리스트에 추가
                List<PostData> tempPostDataList = new List<PostData>();

                for (int i = 0; i < postDataJson.Count; i++)
                {
                    string nickname = postDataJson[i]["nickname"].ToString();
                    string title = postDataJson[i]["title"].ToString();
                    string content = postDataJson[i]["content"].ToString();
                    string inDate = postDataJson[i]["inDate"].ToString();
                    //string date = postDataJson[i]["date"].ToString(); 

                    // 새로운 게시글 데이터를 리스트에 추가
                    PostData postData = new PostData();
                    postData.nickname = nickname;
                    postData.title = title;
                    postData.content = content;
                    postData.inDate = inDate;
                    postDataList.Add(postData);
                    tempPostDataList.Add(postData);

                }
                //기존의 게시글 데이터를 지우지 않고 임시 리스트에 추가한 후에 한꺼번에 갱신
                postDataList.Clear();
                postDataList.AddRange(tempPostDataList);
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
        //// 기존에 생성된 게시글 UI 요소들을 제거한다.
        foreach (Transform child in post_spawnPoint)
        {
            Destroy(child.gameObject);
        }

        //새로운 게시글 데이터를 이용하여 UI 요소 생성
        foreach (PostData postData in postDataList)
        {
            GameObject postObject = Instantiate(post_Prefab, post_spawnPoint);

            postObject.transform.Find("Text_Info/NickName").GetComponent<TextMeshProUGUI>().text = postData.nickname;
            postObject.transform.Find("Text_Info/Title").GetComponent<TextMeshProUGUI>().text = postData.title;
            postObject.transform.Find("Text_Info/Info").GetComponent<TextMeshProUGUI>().text = postData.content;

            if (!postObjectDic.ContainsKey(postData.inDate))
            {
                //딕셔너리에 게시글 데이터와 게시글 UI 요소를 추가
                postObjectDic.Add(postData.inDate, postObject);              
            }

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

        Button button = InGameUI.Instance().btn_delete.GetComponent<Button>();
        if(button != null)
        {
            button.onClick.AddListener(() => deletePost(postDataList[index].inDate));
        }
    }
    public void deletePost(string inDate)
    {
        // 게시글 삭제 요청 보내기
        var bro = Backend.GameData.DeleteV2("notice_table", inDate, Backend.UserInDate);
        if (bro.IsSuccess())
        {
            Debug.Log("게시글 삭제 성공");

            if(postObjectDic.ContainsKey(inDate))
            {
                //딕셔너리에서 게시글 데이터와 게시글 UI 요소를 제거
                Destroy(postObjectDic[inDate]);
                postObjectDic.Remove(inDate);

                postDataList.RemoveAll(post => post.inDate == inDate);
                ////게시글 리스트 갱신
                GetPost();
                //게시글 패널 닫기
                InGameUI.Instance().ClosePost();
                //게시글 삭제 성공 메시지 출력
                Debug.Log("게시글 삭제 성공");
               
            }
        }
        else
        {
            Debug.LogError("게시글 삭제 실패 : " + bro.GetErrorCode());
        }

    }
}
