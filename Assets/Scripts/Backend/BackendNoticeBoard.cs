using BackEnd;
using TMPro;
using UnityEngine;

public class BackendNoticeBoard : MonoBehaviour
{
    public TMP_InputField[] input_post;
    const byte TITLE_INDEX = 0;
    const byte CONTENT_INDEX = 1;
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
        }
        else
        {
            Debug.LogError("게시글 작성 실패");
        }
    }
}
