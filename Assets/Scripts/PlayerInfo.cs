using UnityEngine;

public enum PlayerType {
    None,
    Boy,
    Girl
}

public sealed class PlayerInfo : SingletonBase<PlayerInfo>
{
    [SerializeField]
    private PlayerType playerType = PlayerType.Boy;

    public void SetPlayerType(PlayerType type) {
        playerType = type;
    }
    public PlayerType GetPlayerType() {
        return playerType;
    }

}
