using BackEnd;
using BackEnd.Tcp;
using Protocol;
using System.Collections.Generic;
using UnityEngine;

/* [ BackendInGame.cs ]
 * 인게임에 필요한 변수들
 * 인게임서버 게임룸 접속하기 (인게임 서버 접속은 BackendMatch.cs에 정의)
 * 인게임 서버 접속 종료
 * 게임 시작
 */
public partial class BackendMatchManager : MonoBehaviour
{
    private bool isSetHost = false;                 // 호스트 세션 결정했는지 여부
    // public Dictionary<SessionId, PlayerType> playerTypeList; // 세션별 플레이어 타입

    // 게임 로그
    private string FAIL_ACCESS_INGAME = "인게임 접속 실패 : {0} - {1}";
    private string SUCCESS_ACCESS_INGAME = "유저 인게임 접속 성공 : {0}";
    private string NUM_INGAME_SESSION = "인게임 내 세션 갯수 : {0}";
    // 게임 레디 상태일 때 호출됨
    public void OnGameReady()
    {
        if (isSetHost == false)
        {
            // 호스트가 설정되지 않은 상태이면 호스트 설정
            isSetHost = SetHostSession();
        }
        Debug.Log("호스트 설정 완료");
    }
    // 서버에서 게임 시작 패킷을 보냈을 때 호출
    // 모든 세션이 게임 룸에 참여 후 "콘솔에서 설정한 시간" 후에 게임 시작 패킷이 서버에서 온다
    private void GameSetup()
    {
        Debug.Log("게임 시작 메시지 수신. 게임 설정 시작");
        // 게임 시작 메시지가 오면 게임을 레디 상태로 변경
        isHost = false;
        isSetHost = false;
        OnGameReady();
    }

    // 현재 룸에 접속한 세션들의 정보
    // 클라이언트가 게임방 접속에 성공했을 때 입장한 클라이언트에게만 호출 (재접속 O)
    private void ProcessMatchInGameSessionList(MatchInGameSessionListEventArgs args)
    {
        sessionIdList = new List<SessionId>();
        gameRecords = new Dictionary<SessionId, MatchUserGameRecord>();

        foreach (var record in args.GameRecords)
        {
            Debug.LogFormat("기존 인게임 유저 정보 [{0}] : {1}", record.m_sessionId, record.m_nickname);
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);
        }
        sessionIdList.Sort();
    }

    // 클라이언트가 게임방 접속에 성공했을 때 모든 유저에게 호출 (재접속 시 X)
    private void ProcessMatchInGameAccess(MatchInGameSessionEventArgs args)
    {
        if (isReconnectProcess)
        {
            // 재접속 프로세스 인 경우
            // 이 메시지는 수신되지 않고, 만약 수신되어도 무시함
            Debug.Log("재접속 프로세스 진행중... 재접속 프로세스에서는 ProcessMatchInGameAccess 메시지는 수신되지 않습니다.\n" + args.ErrInfo);
            return;
        }

        Debug.Log(string.Format(SUCCESS_ACCESS_INGAME, args.ErrInfo));

        if (args.ErrInfo != ErrorCode.Success)
        {
            // 게임 룸 접속 실패
            var errorLog = string.Format(FAIL_ACCESS_INGAME, args.ErrInfo, args.Reason);
            Debug.Log(errorLog);
            LeaveInGameServer();
            return;
        }

        // 게임 룸 접속 성공
        // 인자값에 방금 접속한 클라이언트(세션)의 세션ID와 매칭 기록이 들어있다.
        // 세션 정보는 누적되어 들어있기 때문에 이미 저장한 세션이면 건너뛴다.

        var record = args.GameRecord;
        Debug.Log(string.Format(string.Format("새로운 인게임 유저 정보 [{0}] : {1}", args.GameRecord.m_sessionId, args.GameRecord.m_nickname)));
        if (!sessionIdList.Contains(args.GameRecord.m_sessionId))
        {
            // 세션 정보, 게임 기록 등을 저장
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);

            Debug.Log(string.Format(NUM_INGAME_SESSION, sessionIdList.Count));
        }               
    }

    // 인게임 룸 접속
    private void AccessInGameRoom(string roomToken)
    {
        SelectUI.Instance().SetProgressText("게임 방 접속 중");

        Backend.Match.JoinGameRoom(roomToken);
    }
    // 인게임 서버 접속 종료
    public void LeaveInGameServer()
    {
        isConnectInGameServer = false;
        Backend.Match.LeaveGameServer();
    }

    // 호스트에서 보낸 세션리스트로 갱신
    public void SetPlayerSessionList(List<SessionId> sessions)
    {
        sessionIdList = sessions;
    }
    // 서버로 데이터 패킷 전송
    // 서버에서는 이 패킷을 받아 모든 클라이언트(패킷 보낸 클라이언트 포함)로 브로드캐스팅 해준다.
    public void SendDataToInGame<T>(T msg)
    {
        var byteArray = DataParser.DataToJsonData<T>(msg);
        Backend.Match.SendDataToInGameRoom(byteArray);
    }
        private void ProcessSessionOnline(SessionId sessionId, string nickName)
    {
        // 호스트가 아니면 아무 작업 안함 (호스트가 해줌)
        if (isHost)
        {
            // 재접속 한 클라이언트가 인게임 씬에 접속하기 전 게임 정보값을 전송 시 nullptr 예외가 발생하므로 조금
            // 2초정도 기다린 후 게임 정보 메시지를 보냄
            Invoke("SendGameSyncMessage", 2.0f);
        }
    }

    // Invoke로 실행됨
    private void SendGameSyncMessage()
    {
        // 현재 게임 상황 (위치, hp 등등...)
        var message = WorldManager.instance.GetNowGameState(hostSession);
        SendDataToInGame(message);
    }
}