using BackEnd;
using BackEnd.Tcp;
using System;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.SceneManagement;
using Protocol;


/* [ partial class 매치 매니저 ]
 * - BackendMatchManager.cs
 * - BackendMatch.cs
 * - BackendInGame.cs
 * 
 * [ BackendMatchManager.cs ]
 * 매치매니저에 필요한 변수 선언
 * 매치메이킹 핸들러 정의
 * 인게임 핸들러 정의
 * 매칭 관련 기능은 BackendMatch.cs에 정의
 * 인게임 관련 기능은 BackendInGame.cs에 정의
 */
public partial class BackendMatchManager : MonoBehaviour
{
    public class ServerInfo
    {
        public string host;
        public ushort port;
        public string roomToken;
    }
    // 콘솔에서 생성한 매칭 카드 정보
    public class MatchInfo
    {
        public string title;                // 매칭 명
        public string inDate;               // 매칭 inDate (UUID)
        public MatchType matchType;         // 매치 타입
        public MatchModeType matchModeType; // 매치 모드 타입
        public string headCount;            // 매칭 인원
        public bool isSandBoxEnable;        // 샌드박스 모드 (AI매칭)
    }
    public List<MatchInfo> matchInfos { get; private set; } = new List<MatchInfo>();  // 콘솔에서 생성한 매칭 카드들의 리스트
    public List<SessionId> sessionIdList { get; private set; }  // 매치에 참가중인 유저들의 세션 목록
    public Dictionary<SessionId, MatchUserGameRecord> gameRecords { get; private set; } = null;  // 매치에 참가중인 유저들의 매칭 기록
    private static BackendMatchManager instance = null;
    private string inGameRoomToken = string.Empty;  // 게임 룸 토큰 (인게임 접속 토큰)
    public SessionId hostSession { get; private set; }  // 호스트 세션
    private ServerInfo roomInfo = null;             // 게임 룸 정보
    [field: SerializeField]
    public bool isConnectMatchServer { get; private set; } = false;
    [SerializeField]
    private bool isConnectInGameServer = false;
    [SerializeField]
    private bool isJoinGameRoom = false;
    public bool isReconnectProcess { get; private set; } = false;
    public bool isSandBoxGame { get; private set; } = false;

    #region Host
    [SerializeField]
    private bool isHost = false;                    // 호스트 여부 (서버에서 설정한 SuperGamer 정보를 가져옴)
    private Queue<KeyMessage> localQueue = null;    // 호스트에서 로컬로 처리하는 패킷을 쌓아두는 큐 (로컬처리하는 데이터는 서버로 발송 안함)
    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public static BackendMatchManager Instance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    private void OnApplicationQuit()
    {
        if (isConnectMatchServer)
        {
            LeaveMatchMakingServer();
            Debug.Log("ApplicationQuit - LeaveMatchServer");
        }
    }

    private void Update()
    {
        if (isConnectInGameServer || isConnectMatchServer)
        {
            Backend.Match.Poll();

            // 호스트의 경우 로컬 큐가 존재
            // 큐에 있는 패킷을 로컬에서 처리
            if (localQueue != null)
            {
                while (localQueue.Count > 0)
                {
                    var msg = localQueue.Dequeue();
                    WorldManager.instance.OnRecieveForLocal(msg);
                }
            }
        }
    }
    private void Start()
    {
        MatchMakingHandler();
        GameHandler();
        ExceptionHandler();
    }
    public bool IsHost()
    {
        return isHost;
    }
    public bool IsMySessionId(SessionId session)
    {
        return Backend.Match.GetMySessionId() == session;
    }
    public string GetNickNameBySessionId(SessionId session)
    {
        //return Backend.Match.GetNickNameBySessionId(session);
        return gameRecords[session].m_nickname;
    }

    public bool IsSessionListNull()
    {
        return sessionIdList == null || sessionIdList.Count == 0;
    }
    private bool SetHostSession()
    {
        // 호스트 세션 정하기
        // 각 클라이언트가 모두 수행 (호스트 세션 정하는 로직은 모두 같으므로 각각의 클라이언트가 모두 로직을 수행하지만 결과값은 같다.)

        Debug.Log("호스트 세션 설정 진입");
        // 호스트 세션 정렬 (각 클라이언트마다 입장 순서가 다를 수 있기 때문에 정렬)
        sessionIdList.Sort();
        isHost = false;
        // 내가 호스트 세션인지
        foreach (var record in gameRecords)
        {
            if (record.Value.m_isSuperGamer == true)
            {
                if (record.Value.m_sessionId.Equals(Backend.Match.GetMySessionId()))
                {
                    isHost = true;
                }
                hostSession = record.Value.m_sessionId;
                break;
            }
        }

        Debug.Log("호스트 여부 : " + isHost);

        // 호스트 세션이면 로컬에서 처리하는 패킷이 있으므로 로컬 큐를 생성해준다
        if (isHost)
        {
            localQueue = new Queue<KeyMessage>();
        }
        else
        {
            localQueue = null;
        }

        // 호스트 설정까지 끝나면 매치서버와 접속 끊음
        LeaveMatchMakingServer();
        return true;
    }
    // 매칭 서버 관련 이벤트 핸들러
    private void MatchMakingHandler()
    {
        // 매칭 서버 접속 이벤트
        Backend.Match.OnJoinMatchMakingServer += (JoinChannelEventArgs args) =>
        {
            Debug.Log("OnJoinMatchMakingServer : " + args.ErrInfo);
            ProcessAccessMatchMakingServer(args.ErrInfo);
        };

        // 매칭 서버 접속 종료 이벤트
        Backend.Match.OnLeaveMatchMakingServer += (args) =>
        {
            Debug.Log("OnLeaveMatchMakingServer : " + args.ErrInfo);
            isConnectMatchServer = false;
        };

        // 대기방 생성 이벤트
        Backend.Match.OnMatchMakingRoomCreate += (MatchMakingInteractionEventArgs args) =>
        {
            Debug.Log("OnMatchMakingRoomCreate : " + args.ErrInfo + " : " + args.Reason);
            if (args.ErrInfo == ErrorCode.Success)
            {
                RequestMatchMaking(0);
            }
            else
            {
                Backend.Match.CreateMatchRoom();
            }
        };

        // 매칭 신청/최초/성사 이벤트
        Backend.Match.OnMatchMakingResponse += (args) =>
        {
            Debug.Log("OnMatchMakingResponse : " + args.ErrInfo + " : " + args.Reason);
            ProcessMatchMakingResponse(args);
        };
        // 대기방에 유저 입장 메시지
        Backend.Match.OnMatchMakingRoomJoin += (args) =>
        {
            Debug.Log(string.Format("OnMatchMakingRoomJoin : {0} : {1}", args.ErrInfo, args.Reason));
        };
    }

    // 인게임 서버 관련 이벤트 핸들러
    private void GameHandler()
    {
        // 인게임 서버접속 이벤트
        Backend.Match.OnSessionJoinInServer += (args) =>
        {
            Debug.Log("OnSessionJoinInServer : " + args.ErrInfo);
            if (args.ErrInfo != ErrorInfo.Success)
            {
                if (isReconnectProcess)
                {
                    if (args.ErrInfo.Reason.Equals("Reconnect Success"))
                    {
                        //재접속 성공
                        Debug.Log("재접속 성공");
                    }
                    else if (args.ErrInfo.Reason.Equals("Fail To Reconnect"))
                    {
                        Debug.Log("재접속 실패");
                        JoinMatchMakingServer();
                        isConnectInGameServer = false;
                    }
                }
                return;
            }
            if (isJoinGameRoom)
            {
                return;
            }
            if (inGameRoomToken == string.Empty)
            {
                Debug.LogError("인게임 서버 접속 성공했으나 룸 토큰이 없습니다.");
                return;
            }
            Debug.Log("인게임 서버 접속 성공");
            isJoinGameRoom = true;
            AccessInGameRoom(inGameRoomToken);
        };

        // 클라이언트가 게임방 접속에 성공했을 때 입장한 클라이언트에게만 호출
        Backend.Match.OnSessionListInServer += (args) =>
        {
            // 세션 리스트 호출 후 조인 채널이 호출됨
            // 현재 같은 게임(방)에 참가중인 플레이어들 중 나보다 먼저 이 방에 들어와 있는 플레이어들과 나의 정보가 들어있다.
            // 나보다 늦게 들어온 플레이어들의 정보는 OnMatchInGameAccess 에서 수신됨
            Debug.Log("OnSessionListInServer : " + args.ErrInfo);
            ProcessMatchInGameSessionList(args);
            SceneManager.LoadScene("2. InGameJJM");
        };

        // 클라이언트가 게임방 접속에 성공했을 때 모든 유저에게 호출
        Backend.Match.OnMatchInGameAccess += (args) =>
        {
            Debug.Log("OnMatchInGameAccess : " + args.ErrInfo);
            // 세션(클라이언트)이 인게임 룸에 접속할 때마다 호출
            ProcessMatchInGameAccess(args);
        };

        // 서버에서 게임 시작 패킷을 보내면 호출
        Backend.Match.OnMatchInGameStart += () =>
        {
            GameSetup();
        };
        
        // 게임 결과 이벤트
        Backend.Match.OnMatchResult += (args) =>
        {
            Debug.Log("게임 결과값 업로드 결과 : " + string.Format("{0} : {1}", args.ErrInfo, args.Reason));
            // 서버에서 게임 결과 패킷을 보내면 호출
            // 내가(클라이언트가) 서버로 보낸 결과값이 정상적으로 업데이트 되었는지 확인
        };

        Backend.Match.OnMatchRelay += (args) =>
        {
            // 각 클라이언트들이 서버를 통해 주고받은 패킷들
            // 서버는 단순 브로드캐스팅만 지원 (서버에서 어떠한 연산도 수행하지 않음)
            if (WorldManager.instance == null)
            {
                // 월드 매니저가 존재하지 않으면 바로 리턴
                return;
            }

            WorldManager.instance.OnRecieve(args);
        };

        Backend.Match.OnMatchChat += (args) =>
        {
            // 채팅기능은 튜토리얼에 구현되지 않았습니다.
        };

        // 인게임 서버 접속 종료 이벤트
        Backend.Match.OnLeaveInGameServer += (args) =>
        {
            Debug.Log("OnLeaveInGameServer : " + args.ErrInfo + " : " + args.Reason);
            if (args.Reason.Equals("Fail To Reconnect"))
            {
                JoinMatchMakingServer();
            }
            isConnectInGameServer = false;
        };

        // 다른 유저가 재접속 했을 때 호출
        Backend.Match.OnSessionOnline += (args) =>
        {
            var nickName = Backend.Match.GetNickNameBySessionId(args.GameRecord.m_sessionId);
            Debug.Log(string.Format("[{0}] 온라인되었습니다. - {1} : {2}", nickName, args.ErrInfo, args.Reason));
        };

        // 다른 유저 혹은 자기자신이 접속이 끊어졌을 때 모든 클라이언트에게 호출
        Backend.Match.OnSessionOffline += (args) =>
        {
            Debug.Log(string.Format("[{0}] 오프라인되었습니다. - {1} : {2}", args.GameRecord.m_nickname, args.ErrInfo, args.Reason));
        };

        // 슈퍼게이머 유저가 변경되었을 때 호출
        Backend.Match.OnChangeSuperGamer += (args) =>
        {
            Debug.Log(string.Format("이전 방장 : {0} / 새 방장 : {1}", args.OldSuperUserRecord.m_nickname, args.NewSuperUserRecord.m_nickname));
        };
    }
    private void ExceptionHandler()
    {
        // 예외가 발생했을 때 호출
        Backend.Match.OnException += (e) =>
        {
            Debug.Log(e);
        };
    }
    public void GetMatchList()
    {
        // 매칭 카드 정보 초기화
        matchInfos.Clear();
        var callback = Backend.Match.GetMatchList();
        if (callback.IsSuccess() == false)
        {
            Debug.Log("매칭카드 리스트 불러오기 실패\n" + callback);
            return;
        }

        foreach (JsonData row in callback.Rows())
        {
            MatchInfo matchInfo = new MatchInfo();
            matchInfo.title = row["matchTitle"]["S"].ToString();
            matchInfo.inDate = row["inDate"]["S"].ToString();
            matchInfo.headCount = row["matchHeadCount"]["N"].ToString();
            matchInfo.isSandBoxEnable = row["enable_sandbox"]["BOOL"].ToString().Equals("True") ? true : false;

            foreach (MatchType type in Enum.GetValues(typeof(MatchType)))
            {
                if (type.ToString().ToLower().Equals(row["matchType"]["S"].ToString().ToLower()))
                {
                    matchInfo.matchType = type;
                }
            }

            foreach (MatchModeType type in Enum.GetValues(typeof(MatchModeType)))
            {
                if (type.ToString().ToLower().Equals(row["matchModeType"]["S"].ToString().ToLower()))
                {
                    matchInfo.matchModeType = type;
                }
            }

            matchInfos.Add(matchInfo);
        }
        Debug.Log("매칭카드 리스트 불러오기 성공 : " + matchInfos.Count);
    }
    public void SetHostSession(SessionId host)
    {
        hostSession = host;
    }
    public void AddMsgToLocalQueue(KeyMessage message)
    {
        // 로컬 큐에 메시지 추가
        if (isHost == false || localQueue == null)
        {
            return;
        }

        localQueue.Enqueue(message);
    }
}
