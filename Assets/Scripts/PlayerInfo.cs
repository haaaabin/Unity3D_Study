using UnityEngine;

public enum PlayerType {
    Boy,
    Girl
}

public sealed class PlayerInfo : SingletonBase<PlayerInfo>
{
    [SerializeField]
    private PlayerType playerType = PlayerType.Boy;
    public PlayerType PlayerType
    {
        get { return playerType; }
        set { playerType = value; }
    }

    [SerializeField]
    private string nickname = "Unknown";
    public string Nickname
    {
        get { return nickname; }
        set 
        { 
            if (string.IsNullOrEmpty(value))
            {
                Debug.Log("PlayerInfo::Nickname setter - IsNullOrEmpty");
                return;
            }
            nickname = value; 
        }
    }
}