using BackEnd.Tcp;
using Protocol;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    static public WorldManager instance;
    private SessionId myPlayerIndex = SessionId.None;

    #region 플레이어
    public GameObject playerPool;
    public GameObject playerPrefab;
    private Dictionary<SessionId, Player> players;

    public GameObject startPointObject;
    private Vector3 startingPoint;

    #endregion

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitializeGame();
    }
    public bool InitializeGame()
    {
        if (!playerPool)
        {
            Debug.Log("Player Pool Not Exist!");
            return false;
        }
        Debug.Log("게임 초기화 진행");
        myPlayerIndex = SessionId.None;
        // 시작점
        startingPoint = startPointObject.transform.position;
        SetPlayerInfo();
        return true;
    }

    public void SetPlayerInfo()
    {
        if (BackendMatchManager.Instance().sessionIdList == null)
        {
            // 현재 세션ID 리스트가 존재하지 않으면, 0.5초 후 다시 실행
            Invoke("SetPlayerInfo", 0.5f);
            return;
        }
        var gamers = BackendMatchManager.Instance().sessionIdList;
        int size = gamers.Count;
        if (size <= 0)
        {
            Debug.Log("No Player Exist!");
            return;
        }

        players = new Dictionary<SessionId, Player>();

        int index = 0;
        foreach (var sessionId in gamers)
        {
            Debug.LogFormat("플레이어 생성 : {0}", sessionId);
            GameObject player = Instantiate(playerPrefab, new Vector3(startingPoint.x, startingPoint.y, startingPoint.z), Quaternion.identity, playerPool.transform);
            players.Add(sessionId, player.GetComponent<Player>());

            if (BackendMatchManager.Instance().IsMySessionId(sessionId))
            {
                myPlayerIndex = sessionId;
                players[sessionId].Initialize(true, myPlayerIndex, BackendMatchManager.Instance().GetNickNameBySessionId(sessionId), 0);
            }
            else
            {
                players[sessionId].Initialize(false, sessionId, BackendMatchManager.Instance().GetNickNameBySessionId(sessionId), 0);
            }
            index += 1;
        }
        Debug.Log("Num Of Current Player : " + size);
        if (BackendMatchManager.Instance().IsHost())
        {
            // 게임 시작 메시지를 전송
            GameStartMessage gameStartMessage = new GameStartMessage();
            BackendMatchManager.Instance().SendDataToInGame<GameStartMessage>(gameStartMessage);
        }

    }
    public void OnRecieve(MatchRelayEventArgs args)
    {
        if (args.BinaryUserData == null)
        {
            Debug.LogWarning(string.Format("빈 데이터가 브로드캐스팅 되었습니다.\n{0} - {1}", args.From, args.ErrInfo));
            // 데이터가 없으면 그냥 리턴
            return;
        }
        Message msg = DataParser.ReadJsonData<Message>(args.BinaryUserData);
        if (msg == null)
        {
            return;
        }
        if (BackendMatchManager.Instance().IsHost() != true && args.From.SessionId == myPlayerIndex)
        {
            return;
        }
        if (players == null)
        {
            Debug.LogError("Players 정보가 존재하지 않습니다.");
            return;
        }
        switch (msg.type)
        {
            case Protocol.Type.GameStart:
                GameManager.Instance().ChangeState(GameManager.GameState.InGame);
                break;
            case Protocol.Type.Key:
                KeyMessage keyMessage = DataParser.ReadJsonData<KeyMessage>(args.BinaryUserData);
                ProcessKeyEvent(args.From.SessionId, keyMessage);
                break;
            case Protocol.Type.PlayerMove:
                PlayerMoveMessage moveMessage = DataParser.ReadJsonData<PlayerMoveMessage>(args.BinaryUserData);
                ProcessPlayerData(moveMessage);
                break;
            case Protocol.Type.GameSync:
                GameSyncMessage syncMessage = DataParser.ReadJsonData<GameSyncMessage>(args.BinaryUserData);
                ProcessSyncData(syncMessage);
                break;
            default:
                Debug.Log("Unknown protocol type");
                return;
        }
    }
    public void OnRecieveForLocal(KeyMessage keyMessage)
    {
        ProcessKeyEvent(myPlayerIndex, keyMessage);
    }
    private void ProcessKeyEvent(SessionId index, KeyMessage keyMessage)
    {
        if (BackendMatchManager.Instance().IsHost() == false)
        {
            //호스트만 수행
            return;
        }
        bool isMove = false;

        int keyData = keyMessage.keyData;

        Vector3 moveVector = Vector3.zero;
        Vector3 playerPos = players[index].GetPosition();
        if ((keyData & KeyEventCode.MOVE) == KeyEventCode.MOVE)
        {
            moveVector = new Vector3(keyMessage.x, keyMessage.y, keyMessage.z);
            moveVector = Vector3.Normalize(moveVector);
            isMove = true;
        }

        if (isMove)
        {
            players[index].SetMoveVector(moveVector);
            PlayerMoveMessage msg = new PlayerMoveMessage(index, playerPos, moveVector);
            BackendMatchManager.Instance().SendDataToInGame<PlayerMoveMessage>(msg);
        }
    }
    private void ProcessPlayerData(PlayerMoveMessage data)
    {
        if (BackendMatchManager.Instance().IsHost() == true)
        {
            //호스트면 리턴
            return;
        }
        Vector3 moveVecotr = new Vector3(data.xDir, data.yDir, data.zDir);
        // moveVector가 같으면 방향 & 이동량 같으므로 적용 굳이 안함
        if (!moveVecotr.Equals(players[data.playerSession].moveVector))
        {
            players[data.playerSession].SetPosition(data.xPos, data.yPos, data.zPos);
            players[data.playerSession].SetMoveVector(moveVecotr);
        }
    }

    private void ProcessSyncData(GameSyncMessage syncMessage)
    {
        // 플레이어 데이터 동기화
        int index = 0;
        if (players == null)
        {
            Debug.LogError("Player Poll is null!");
            return;
        }
        foreach (var player in players)
        {
            var y = player.Value.GetPosition().y;
            player.Value.SetPosition(new Vector3(syncMessage.xPos[index], y, syncMessage.zPos[index]));
            index++;
        }
        BackendMatchManager.Instance().SetHostSession(syncMessage.host);
    }
    public bool IsMyPlayerMove()
    {
        return players[myPlayerIndex].isMove;
    }

    public bool IsMyPlayerRotate()
    {
        return players[myPlayerIndex].isRotate;
    }
    public GameSyncMessage GetNowGameState(SessionId hostSession)
    {
        int numOfClient = players.Count;

        float[] xPos = new float[numOfClient];
        float[] zPos = new float[numOfClient];
        bool[] online = new bool[numOfClient];
        int index = 0;
        foreach (var player in players)
        {
            xPos[index] = player.Value.GetPosition().x;
            zPos[index] = player.Value.GetPosition().z;
            index++;
        }
        return new GameSyncMessage(hostSession, numOfClient, xPos, zPos, online);
    }
}
