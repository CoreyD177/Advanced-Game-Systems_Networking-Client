using UnityEngine; //Connect to Unity Engine
using UnityEngine.UI; //Access the UI elements of the Unity Engine
using RiptideNetworking; //Use the Riptide Networking components


public class UIManager : MonoBehaviour
{
    #region Variables
    //There can be only one!!! We will have a private instance and a public property to control the instaqnce
    private static UIManager _uIManagerInstance;
    public static UIManager UIManagerInstance
    {
        //Property read is public by default and reads the set instance
        get => _uIManagerInstance;
        private set
        {
            //Property private write sets the loaded instance to be the instance if one doesn't exist
            if (_uIManagerInstance == null)
            {
                _uIManagerInstance = value;
            }
            //If an instance already exist log a warning and destroy the new instance
            else if (_uIManagerInstance != value)
            {
                Debug.LogWarning($"{nameof(NetworkManager)} instance already exists, destroy duplicate! THERE CAN BE ONLY ONE!");
                Destroy(value);
            }
        }
    }
    [Header("Connect")]
    [Tooltip("Drag the UI panel from the game scene in here so we can turn it on and off")]
    [SerializeField] private GameObject _connectUI;
    [Tooltip("Drag the text box from the UI interface so we can get the username input from it")]
    [SerializeField] private InputField _usernameField;
    #endregion
    #region UI Functions
    private void Awake()
    {
        //Access the UIManagerInstance property to set this class as the instance
        UIManagerInstance = this;
    }
    public void ConnectClicker() //Activated via UI Button
    {
        //Set the username field to be inactive
        _usernameField.interactable = false;
        //Disable the UI panel
        _connectUI.SetActive(false);
        //Run the server connection function
        NetworkManager.NetworkManagerInstance.Connect();
    }
    public void BackToMain()
    {
        //Reenable the username text field
        _usernameField.interactable = true;
        //Reactivate the UI panel
        _connectUI.SetActive(true);
    }
    public void SendName()
    {
        //Create a new message to send the name to the server
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerID.name);
        //Add the username to the message
        message.AddString(_usernameField.text);
        //Use the network manager to send the message to the server
        NetworkManager.NetworkManagerInstance.GameClient.Send(message);
    }
    #endregion
}
