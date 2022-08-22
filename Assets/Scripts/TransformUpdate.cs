using UnityEngine; //Connect to Unity Engine

public class TransformUpdate
{
    //Tick stores the current iteration of the server/client tick count
    public ushort Tick { get; private set; }
    //A Vector3 to store the position of the object sent by the server
    public Vector3 Position { get; private set; }

    public TransformUpdate(ushort tick, Vector3 position)
    {
        //Set the Tick count and the position to match the values passed to this function
        Tick = tick;
        Position = position;
    }
}
