using BackEnd;
using BackEnd.Tcp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendMatch : MonoBehaviour
{
    /* 1. 매칭 서버 접속
     * 2. 대기방 생성
     * 3. 매칭 신청
     * 4. 매칭 성사
     */
    void JoinMatchServer()
    {
        ErrorInfo errorInfo;
        bool isSuccess = Backend.Match.JoinMatchMakingServer(out errorInfo);
        //if (errorInfo != null)
        //{
        //    Debug.LogError("매칭 서버 접속 실패");
        //    return;
        //}
        if (isSuccess)
        {
            Debug.Log("매칭 서버 접속 성공");
        }
        else
        {
            Debug.LogError("매칭 서버 접속 실패");
        }
        Backend.Match.OnJoinMatchMakingServer = (JoinChannelEventArgs args) =>
        {
            // TODO
        };
    }

    void LeaveMatchserver()
    {
        Backend.Match.LeaveMatchMakingServer();
        Backend.Match.OnLeaveMatchMakingServer = (LeaveChannelEventArgs args) =>
        {
            Debug.Log("매칭 서버 접속 해제");
        };
    }
}
