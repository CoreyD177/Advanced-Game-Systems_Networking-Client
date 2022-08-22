using RiptideNetworking; //Allows us to use networking functions from the Riptide Networking function
using UnityEngine; //Connection to Unity engine

public static class MessageExtensions
{
    //Extends the Message class so we can pass along Vector2, Vector3 and Quaternion structs
    #region Vector2
    //New overload for the Add function so we can use it to add Vector2 values. Calls the AddVector2 method and passes along message and Vector2 value.
    public static Message Add(this Message message, Vector2 value) => AddVector2(message, value);
    //AddVector2 method recieves the Vector2 information and the message and adds the Vector2 as a series of 2 floats    
    public static Message AddVector2(this Message message, Vector2 value)
    {
        return message.AddFloat(value.x).AddFloat(value.y);
    }
    //GetVector2 retrieves the 2 sequential floats from the message and passes them back as a Vector2    
    public static Vector2 GetVector2(this Message message)
    {
        return new Vector2(message.GetFloat(), message.GetFloat());
    }
    #endregion
    #region Vector3
    //New overload for the Add function to call the AddVector3 function and pass along the message and 
    public static Message Add(this Message message, Vector3 value) => AddVector3(message, value);
    //AddVector3 function takes the message and Vector3 information and adds the Vector3 to the message as a sequence of 3 floats    
    public static Message AddVector3(this Message message, Vector3 value)
    {
        return message.AddFloat(value.x).AddFloat(value.y).AddFloat(value.z);
    }
    //GetVector3 function retrieves 3 sequential floats from the message and returns it as a Vector3    
    public static Vector3 GetVector3(this Message message)
    {
        return new Vector3(message.GetFloat(), message.GetFloat(), message.GetFloat());
    }
    #endregion
    #region Quaternion
    //New overload for the Add function to take in the message and Quaternion data and pass it on to the AddQuaternion function
    public static Message Add(this Message message, Quaternion value) => AddQuaternion(message, value);
    //AddQuaternion function takes the Quaternion data and adds it to the message as 4 sequential floats   
    public static Message AddQuaternion(this Message message, Quaternion value)
    {
        return message.AddFloat(value.x).AddFloat(value.y).AddFloat(value.z).AddFloat(value.w);
    }
    //GetQuaternion function retrieves 4 sequential floats from the message and returns them as a Quaternion    
    public static Quaternion GetQuaternion(this Message message)
    {
        return new Quaternion(message.GetFloat(), message.GetFloat(), message.GetFloat(), message.GetFloat());
    }
    #endregion
}
