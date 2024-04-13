using BackEnd;
using UnityEngine;

/* [ BackendInGame.cs ]
 * 인게임에 필요한 변수들
 * 인게임서버 게임룸 접속하기 (인게임 서버 접속은 BackendMatch.cs에 정의)
 * 인게임 서버 접속 종료
 * 게임 시작
 */
public partial class BackendMatchManager : MonoBehaviour
{
    // 게임 로그
    private string FAIL_ACCESS_INGAME = "인게임 접속 실패 : {0} - {1}";
    private string SUCCESS_ACCESS_INGAME = "유저 인게임 접속 성공 : {0}";
    private string NUM_INGAME_SESSION = "인게임 내 세션 갯수 : {0}";

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
}
