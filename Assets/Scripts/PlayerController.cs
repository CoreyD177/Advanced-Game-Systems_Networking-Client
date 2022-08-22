using RiptideNetworking; //Use Riptide Networking componenets
using UnityEngine; //Connect to Unity Engine

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Camera")]
    [Tooltip("Add the camera that is a child object of this player object to store its transform")]
    [SerializeField] private Transform _camTransform;
    //An array of bools for inputs to store whether the user is pushing that button
    private bool[] inputs;
    #endregion
    #region Setup
    private void Start()
    {
        //Input array is a new array with 6 values
        inputs = new bool[6];
    }
    #endregion
    #region Movement
    private void Update()
    {
        //If user pushes the set button, set its corresponding bool value to true
        if (Input.GetKey(KeyCode.W))
        {
            inputs[0] = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputs[1] = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputs[2] = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputs[3] = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            inputs[4] = true;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputs[5] = true;
        }
    }
    private void FixedUpdate()
    {
        //Run the SendInput method to send the inputs to the server for calculation
        SendInput();
        //Reset array values to false after sending of values
        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = false;
        }
    }
    #endregion
    #region Messages
    private void SendInput()
    {
        //Create a message to send the inputs
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ClientToServerID.input);
        //Add the bool array to the message without adding the length of the array because we know it is 6
        message.AddBools(inputs, false);
        //Add the direction the camera is facing to the message
        message.AddVector3(_camTransform.forward);
        //Use the network manager to send the message to the server
        NetworkManager.NetworkManagerInstance.GameClient.Send(message);
    }
    #endregion
}
