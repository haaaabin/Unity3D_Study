using BackEnd;
using BackEnd.Tcp;
using UnityEngine;

/* [ BackendMatch.cs ]
 * 매칭 관련 기능 정의
 * 매칭 서버 접속
 * 매칭 서버 나가기
 * 매칭 신청하기
 * 매칭 신청 취소하기
 */
public partial class BackendMatchManager : MonoBehaviour
{
    // 디버그 로그
    private string NOTCONNECT_MATCHSERVER = "매치 서버에 연결되어 있지 않습니다.";
    private string RECONNECT_MATCHSERVER = "매치 서버에 접속을 시도합니다.";
    private string FAIL_CONNECT_MATCHSERVER = "매치 서버 접속 실패 : {0}";
    private string SUCCESS_CONNECT_MATCHSERVER = "매치 서버 접속 성공";
    private string SUCCESS_MATCHMAKE = "매칭 성공 : {0}";
    private string SUCCESS_REGIST_MATCHMAKE = "매칭 대기열에 등록되었습니다.";
    private string FAIL_REGIST_MATCHMAKE = "매칭 실패 : {0}";
    private string CANCEL_MATCHMAKE = "매칭 신청 취소 : {0}";
    private string INVAILD_MATCHTYPE = "잘못된 매치 타입입니다.";
    private string INVALID_MODETYPE = "잘못된 모드 타입입니다.";
    private string INVALID_OPERATION = "잘못된 요청입니다\n{0}";
    private string EXCEPTION_OCCUR = "예외 발생 : {0}\n다시 매칭을 시도합니다.";


    
    // 매칭 서버 접속 요청
    public void JoinMatchMakingServer()
    {
        if (isConnectMatchServer)
        {
            return;
        }

        ErrorInfo errorInfo;
        isConnectMatchServer = true;
        if (!Backend.Match.JoinMatchMakingServer(out errorInfo))
        {
            Debug.LogError("매칭 서버 접속 실패");
        }
    }

    // 매칭 서버 접속 종료
    public void LeaveMatchMakingServer()
    {
        isConnectMatchServer = false;
        Backend.Match.LeaveMatchMakingServer();
    }

    // 매칭 대기방 생성 및 입장
    public bool CreateMatchRoom()
    {
        SelectUI.Instance().SetPopUpDescription("매칭 대기방 생성 중");

        if (!isConnectMatchServer)
        {
            Debug.LogError(NOTCONNECT_MATCHSERVER);
            Debug.Log(RECONNECT_MATCHSERVER);
            JoinMatchMakingServer();
            return false;
        }
        Backend.Match.CreateMatchRoom();
        return true;
    }

    // 매칭 대기 방 나가기
    public void LeaveMatchRoom()
    {
        Backend.Match.LeaveMatchRoom();
    }
    // 매칭 신청하기
    public void RequestMatchMaking(int index)
    {
        SelectUI.Instance().SetPopUpDescription("매칭 신청중");

        // 매청 서버에 연결되어 있지 않으면 매칭 서버 접속
        if (!isConnectMatchServer)
        {
            Debug.Log(NOTCONNECT_MATCHSERVER);
            Debug.Log(RECONNECT_MATCHSERVER);
            JoinMatchMakingServer();
            return;
        }
        // 변수 초기화
        isConnectInGameServer = false;

        Backend.Match.RequestMatchMaking(matchInfos[index].matchType, matchInfos[index].matchModeType, matchInfos[index].inDate);
        if (isConnectInGameServer)
        {
            Backend.Match.LeaveGameServer(); //인게임 서버 접속되어 있을 경우를 대비해 인게임 서버 리브 호출
        }

        //nowMatchType = matchInfos[index].matchType;
        //nowModeType = matchInfos[index].matchModeType;
        //numOfClient = int.Parse(matchInfos[index].headCount);
    }

    // 매칭 서버 접속에 대한 리턴값
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // 접속 실패
            isConnectMatchServer = false;
        }

        if (!isConnectMatchServer)
        {
            var errorLog = string.Format(FAIL_CONNECT_MATCHSERVER, errInfo.ToString());
            // 접속 실패
            Debug.Log(errorLog);
        }
        else
        {
            //접속 성공
            Debug.Log(SUCCESS_CONNECT_MATCHSERVER);
        }
    }

    /*
     * 매칭 신청에 대한 리턴값 (호출되는 종류)
     * 매칭 신청 성공했을 때
     * 매칭 성공했을 때
     * 매칭 신청 실패했을 때
     */
    private void ProcessMatchMakingResponse(MatchMakingResponseEventArgs args)
    {
        string debugLog = string.Empty;
        bool isError = false;
        switch (args.ErrInfo)
        {
            case ErrorCode.Success:
                // 매칭 성공했을 때
                debugLog = string.Format(SUCCESS_MATCHMAKE, args.Reason);
                SelectUI.Instance().SetPopUpDescription("인게임 서버 접속 중");
                ProcessMatchSuccess(args);
                break;

            case ErrorCode.Match_InProgress:
                // 매칭 신청 성공했을 때 or 매칭 중일 때 매칭 신청을 시도했을 때

                // 매칭 신청 성공했을 때
                if (args.Reason == string.Empty)
                {
                    debugLog = SUCCESS_REGIST_MATCHMAKE;
                }
                break;

            case ErrorCode.Match_MatchMakingCanceled:
                // 매칭 신청이 취소되었을 때
                debugLog = string.Format(CANCEL_MATCHMAKE, args.Reason);
                break;

            case ErrorCode.Match_InvalidMatchType:
                isError = true;
                // 매치 타입을 잘못 전송했을 때
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVAILD_MATCHTYPE);
                break;

            case ErrorCode.Match_InvalidModeType:
                isError = true;
                // 매치 모드를 잘못 전송했을 때
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVALID_MODETYPE);
                break;

            case ErrorCode.InvalidOperation:
                isError = true;
                // 잘못된 요청을 전송했을 때
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                break;

            case ErrorCode.Match_Making_InvalidRoom:
                isError = true;
                // 잘못된 요청을 전송했을 때
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                break;

            case ErrorCode.Exception:
                isError = true;
                // 매칭 되고, 서버에서 방 생성할 때 에러 발생 시 exception이 리턴됨
                // 이 경우 다시 매칭 신청해야 됨
                debugLog = string.Format(EXCEPTION_OCCUR, args.Reason);
                break;
        }

        if (!debugLog.Equals(string.Empty))
        {
            Debug.Log(debugLog);
            if (isError == true)
            {
                //LobbyUI.GetInstance().SetErrorObject(debugLog);
            }
        }
    }

    // 매칭 성공 시 인게임 서버 접속
    private void ProcessMatchSuccess(MatchMakingResponseEventArgs args)
    {

        ErrorInfo errorInfo;
        if (!Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address, args.RoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo))
        {
            var debugLog = string.Format(FAIL_ACCESS_INGAME, errorInfo.ToString(), string.Empty);
            Debug.Log(debugLog);
        }
        // 인자값에서 인게임 룸토큰을 저장해두어야 한다.
        // 인게임 서버에서 룸에 접속할 때 필요
        // 1분 내에 모든 유저가 룸에 접속하지 않으면 해당 룸은 파기된다.
        isConnectInGameServer = true;
        isJoinGameRoom = false;
        isReconnectProcess = false;
        inGameRoomToken = args.RoomInfo.m_inGameRoomToken;
        isSandBoxGame = args.RoomInfo.m_enableSandbox;
    }
}
