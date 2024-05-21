using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public enum CurrentNetworkState
    {
        Disconnected = -1,
        InServer = 0,
        InLobby = 1,
        InRoom = 2
    }

    static NetworkManager instance;
    public static NetworkManager Instance => instance;
    public string roomName = "TestRoom";

    CurrentNetworkState myState = CurrentNetworkState.Disconnected;
    public CurrentNetworkState MyState => myState;

    public UnityAction OnJoinedRoomEvents;

    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        DebugLogger.Instance.Log("Connecting Server");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        myState = CurrentNetworkState.InServer;
        DebugLogger.Instance.Log("Server Connected");
        DebugLogger.Instance.Log("Joining Lobby");
        PhotonNetwork.JoinLobby();
    }

    public override void OnConnected()
    {
        base.OnConnected();
        DebugLogger.Instance.Log("Raw Connection Established");
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        myState = CurrentNetworkState.InLobby;
        DebugLogger.Instance.Log("Lobby Joined");
        DebugLogger.Instance.Log("Creating Room or Joining Existing of name " + roomName);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = false;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, null);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        DebugLogger.Instance.LogError(string.Format("Room creation failed with error code {0} and error message {1}", returnCode, message));
    }

    public override void OnJoinedRoom()
    {
        myState = CurrentNetworkState.InRoom;
        DebugLogger.Instance.Log("Room Joined");
        OnJoinedRoomEvents?.Invoke();
    }
}