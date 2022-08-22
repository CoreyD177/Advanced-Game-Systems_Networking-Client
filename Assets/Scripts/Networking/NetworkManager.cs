using UnityEngine; //Connection to Unity engine
using RiptideNetworking; //Allows use of Riptide Networking components
using RiptideNetworking.Utils; //Allows use of RiptideLogger to log networking error messages
using System; //Allows use of EventArgs for server connection event functions
//Enum to store the values for Server Tick, player location and movement that will be passed from the server to the client
public enum ServerToClientID
{
    sync = 1,
    playerSpawned,
    playerMovement,
}
//Enum to store the player name and input values to send to the server
public enum ClientToServerID
{
    name = 1,
    input,
}
public class NetworkManager : MonoBehaviour
{
    #region Variables
    //There can be only one!!! We will have a private instance and a public property to control the instaqnce
    private static NetworkManager _networkManagerInstance;
    public static NetworkManager NetworkManagerInstance
    {
        //Property read is public by default and reads the set instance
        get => _networkManagerInstance;
        private set
        {
            //Property private write sets instance to the value if the instance is null
            if (_networkManagerInstance == null) _networkManagerInstance = value;
            //Otherwise send a warning and destroy the new instance
            else if (_networkManagerInstance != value)
            {
                Debug.LogWarning($"{nameof(NetworkManager)} instance already exists, destroy duplicate! THERE CAN BE ONLY ONE!");
                Destroy(value);
            }
        }
    }
    //A property to Instantiate and control a game client to connect to the server
    public Client GameClient { get; private set; }
    //private ushort to store the Server Tick increment
    private ushort _serverTick;
    //ServerTick property to get and set the current incrementation of the server and the interpolation tick
    public ushort ServerTick
    {
        get => _serverTick;
        private set
        {
            _serverTick = value;
            InterpolationTick = (ushort)(value - TicksBetweenPositionUpdates);
        }
    }
    //Property to get and set the Interpolation tick. Set to equal server tick minus the amount of ticks between position updates
    public ushort InterpolationTick { get; private set; }
    //Amount of ticks to allow between updates of the position
    private ushort _ticksBetweenPositionUpdates = 2;
    //Property to get and set the Ticks between position updates and update the Interpolation tick
    public ushort TicksBetweenPositionUpdates
    {
        get => _ticksBetweenPositionUpdates;
        private set
        {
            _ticksBetweenPositionUpdates = value;
            InterpolationTick = (ushort)(ServerTick - value);
        }
    }
    [Header("Server Location")]
    [Tooltip("Set the port to use to connect to server")]
    [SerializeField] private ushort s_port;
    [Tooltip("Set the IP of the hosting server")]
    [SerializeField] private string s_ip;
    //Adds a set amount of pixels of space to the inspector before the following property
    [Space(10)]
    [Tooltip("Set the tolerance for difference between server side tick count and client side tick count")]
    [SerializeField] private ushort _tickDivergenceTolerance = 1;
    #endregion
    #region Setup
    private void Awake()
    {
        //When the object that this script is on activates, set the instance to this
        NetworkManagerInstance = this;
    }

    private void Start()
    {
        //Logs what the network is doing with messages for debug, info, warning and error and select whether to include timestamps
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
        //Create a new client
        GameClient = new Client();
        //Add the successful connection function to the client reference to be run upon connecting to server
        GameClient.Connected += DidConnect;
        //Add the Connection failed funtion to the client to be run when connection fails
        GameClient.ConnectionFailed += FailedToConnect;
        //Add the player left function to the client to destroy player on leaving
        GameClient.ClientDisconnected += PlayerLeft;
        //Add the Disconnected function to the client to handle disconnection procedures on exiting from server
        GameClient.Disconnected += DidDisconnect;
        //Set server tick to 2 to avoid it being set witha negative number
        ServerTick = TicksBetweenPositionUpdates;
    }
    #endregion
    #region Server Functions
    private void FixedUpdate()
    {
        //Run the Tick function to handle network events and messages
        GameClient.Tick();
        //Increment to server tick variable to try to keep it up to date between messages
        ServerTick++;
    }
    private void OnApplicationQuit()
    {
        //Run the disconnect method when user quits game
        GameClient.Disconnect();
    }
    #endregion
    #region Events
    public void Connect()
    {
        //Connect to the server using the set port and ip address
        GameClient.Connect($"{s_ip}:{s_port}");
    }
    void DidConnect(object sender, EventArgs e)
    {
        //On server connection call the SendName function from the UI Manager to send the user's name to the server
        UIManager.UIManagerInstance.SendName();
    }
    private void FailedToConnect(object sender, EventArgs e)
    {
        //If failed to connect use the UI Managers BackToMain function to reactivate the connection menu
        UIManager.UIManagerInstance.BackToMain();
    }
    void DidDisconnect(object sender, EventArgs e)
    {
        //On disconnetion from server reactivate the connection menu and destroy all players in the scene
        UIManager.UIManagerInstance.BackToMain();
        foreach (Player player in Player.playerList.Values)
        {
            Destroy(player.gameObject);
        }
    }
    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        //If player list includes value for player matching the leaving players ID, destroy it and remove it from list.
        if (Player.playerList.TryGetValue(e.Id, out Player player)) Destroy(player.gameObject);
        
    }
    private void SetTick(ushort serverTick)
    {
        //Check if we are exceeding the tolerance for differences between player and server tick and set server tick to match player tick if we are
        if (Mathf.Abs(ServerTick - serverTick) > _tickDivergenceTolerance) ServerTick = serverTick;
    }
    //Retrieve the Tick count from the server message and call the SetTick method to compare it with the clients version and adjust if necessary
    [MessageHandler((ushort)ServerToClientID.sync)]
    public static void Sync(Message message)
    {
        NetworkManagerInstance.SetTick(message.GetUShort());
    }
    #endregion
}
