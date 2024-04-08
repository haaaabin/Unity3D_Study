using UnityEngine;

public enum PlayerType {
    None,
    Boy,
    Girl
}

public class PlayerInfo : MonoBehaviour
{
    private static PlayerInfo _instance = null;

    public static PlayerInfo Instance {
        get {
            if (_instance == null) {
                _instance = new PlayerInfo();
            }

            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [SerializeField]
    private PlayerType playerType = PlayerType.Boy;

    public void SetPlayerType(PlayerType type) {
        playerType = type;
    }
    public PlayerType GetPlayerType() {
        return playerType;
    }

}
