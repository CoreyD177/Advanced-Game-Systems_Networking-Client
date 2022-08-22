using RiptideNetworking; //Use the riptide Networking Components
using System.Collections.Generic; //Needed for use of dictionaries
using UnityEngine; //Connect to Unity Engine

public class Player : MonoBehaviour
{
    #region Variables
    //Dictionary to store a list of players connected to the server
    public static Dictionary<ushort, Player> playerList = new Dictionary<ushort, Player>();
    //Property to get and set the ID of the player
    public ushort Id { get; private set; }
    //A bool property to determine whether the object is controlled by the local player or is an online player
    public bool IsLocal { get; private set; }
    [Header("Class References")]
    [Tooltip("Drag the players Animator component to this field to grab its animation manager")]
    [SerializeField] private PlayerAnimationManager _animationManager;
    [Tooltip("Drag the Interpolator component to this field to store it for use")] 
    [SerializeField] private Interpolator _interpolator;
    [Header("Camera")]
    [Tooltip("Add the camera or dummy camera object that is a child of this player object")]
    [SerializeField] private Transform _camTransform;
    //Private variable to store the players username
    private string _username;
    #endregion
    #region Player Management
    private void OnDestroy()
    {
        //When player object is destroyed removes its value from the dictionary
        playerList.Remove(Id);
    }
    private void Move(ushort tick, Vector3 newPosition, Vector3 forward)
    {
        //Run the Interpolators NewUpdate function to handle movement
        _interpolator.NewUpdate(tick, newPosition);
        //If player is not local set direction camera is facing to vector3 setting passed from the message
        if (!IsLocal)
        {
            _camTransform.forward = forward;
            
        }
        //Run the animators speed function to animate the character based off its movement speed
        _animationManager.AnimateBasedOnSpeed();
    }
    public static void Spawn(ushort id, string username, Vector3 position)
    {
        //Create a variable to handle a player instance
        Player player;
        //If player ID matches ID of the local player instantiate the local prefab and set IsLocal to true, otherwise instantiate the online player object. Store the player class on instantiation
        if (id == NetworkManager.NetworkManagerInstance.GameClient.Id)
        {
            player = Instantiate(GameLogic.GameLogicInstance.LocalPlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = true;
        }
        else
        {
            player = Instantiate(GameLogic.GameLogicInstance.PlayerPrefab, position, Quaternion.identity).GetComponent<Player>();
            player.IsLocal = false;
        }
        //Players name will equal the username set by the connected player if it is available or will equal Guest if a username is not available 
        player.name = $"Player {id} ({(string.IsNullOrEmpty(username) ? "Guest" : username)})";
        //Players ID is the ID that was sent by the server
        player.Id = id;
        //Players username is the username sent by the server
        player._username = username;
        //Add the spawned player to the dictionary of players in the scene
        playerList.Add(id, player);
    }
    #endregion
    #region Messages
    //Message handler to run the Spawn function based on the ID, username and position passed in the message
    [MessageHandler((ushort)ServerToClientID.playerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
    //Message handler for player movement checks if we have a player matching the ID and runs their corresponding move function passiing through the Tick count, and Vector3 positions from the message
    [MessageHandler((ushort)ServerToClientID.playerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (playerList.TryGetValue(message.GetUShort(), out Player player))
        {
            player.Move(message.GetUShort(), message.GetVector3(), message.GetVector3());
        }
    }
    #endregion
}
