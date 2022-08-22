using System.Collections.Generic; //Allow use of lists
using UnityEngine; //Connect to Unity Engine

public class Interpolator : MonoBehaviour
{
    #region Variables
    [Header("Interpolation Inputs")]
    [Tooltip("Start to elapsed time since last update at 0")]
    [SerializeField] private float _timeElapsed = 0f;
    [Tooltip("Set how long you want to take to reach the target to adjust how much smoothing to apply")]
    [SerializeField] private float _timeToReachTarget = 0.05f;
    [Tooltip("Set the distance an object has to have moved to count as movement that needs interpolation")]
    [SerializeField] private float _movementThreshold = 0.05f;
    //List of future transforms that haven't been measured yet
    private readonly List<TransformUpdate> _futureTransformUpdates = new List<TransformUpdate>();
    //A float to store the squared movement threshold for use in calculations
    private float _squareMovementThreshold;
    //A Transform update for the position to move to 
    private TransformUpdate _to;
    //A transform update to store the current position
    private TransformUpdate _from;
    //A transform update to store the location of the previous calculated destination
    private TransformUpdate _previous;
    #endregion
    #region Setup
    private void Start()
    {
        //Set the squared movement threshold by multiplying the movement threshold by itself because, yay maths!
        _squareMovementThreshold = _movementThreshold * _movementThreshold;
        //Set the transform update to move to as the current transform position also storing the server tick count 
        _to = new TransformUpdate(NetworkManager.NetworkManagerInstance.ServerTick, transform.position);
        //Set the transform update to move from as the current position also storing the Interpolation tick
        _from = new TransformUpdate(NetworkManager.NetworkManagerInstance.InterpolationTick, transform.position);
        //Set the previous to be the same as the from
        _previous = _from; //new TransformUpdate(NetworkManager.NetworkManagerInstance.InterpolationTick, transform.position);
    }
    #endregion
    #region Interpolation
    private void Update()
    {
        //For every transform update stored in the list
        for (int i = 0; i < _futureTransformUpdates.Count; i++)
        {
            //If server tick is greater or equal to the tick of our current transform update we are looking at
            if (NetworkManager.NetworkManagerInstance.ServerTick >= _futureTransformUpdates[i].Tick)
            {
                //Set the previous transform to the to transform as we have calculated that on the previous pass
                _previous = _to;
                //Add the current entry in the list as the to location
                _to = _futureTransformUpdates[i];
                //Set a new transform update using the interpolation tick and current transform position
                _from = new TransformUpdate(NetworkManager.NetworkManagerInstance.InterpolationTick, transform.position);
                //Remove the current entry from the list as we have now used it
                _futureTransformUpdates.RemoveAt(i);
                //Decrease the increment count to stop us from increasing past 0 as we want to keep working with the first or 0 entry in the list
                i--;
                //Reset the time elapsed since the last calculation
                _timeElapsed = 0f;
                //Time to reach target is the difference between the tick counts of the from and to locations multiplied by fixed deltatime
                _timeToReachTarget = (_to.Tick - _from.Tick) * Time.fixedDeltaTime;
            }           
        }
        //After the calculations have finished start incrementing the elapsed time by the deltatime
        _timeElapsed += Time.deltaTime;
        //Run the Interpolation function passing the time elapsed divided by the time to reach target as a lerp amount
        InterpolatePosition(_timeElapsed / _timeToReachTarget);
    }
    private void InterpolatePosition(float lerpAmount)
    {
        //If the distance between the to and from position is less than the movement threshold
        if ((_to.Position - _previous.Position).sqrMagnitude < _squareMovementThreshold)
        {
            //If both positions are not equal use vector3.Lerp to smoothly move to to position and then return out of function
            if (_to.Position != _from.Position)
            {
                transform.position = Vector3.Lerp(_from.Position, _to.Position, lerpAmount);
                return;
            }
        }
        
        //If the previous wasn't true, use the LerpUnclamped function to move smoothly to the to position
        //Changed to normal Lerp function as unclamped was giving jittery motions and errors
        transform.position = Vector3.Lerp(_from.Position, _to.Position, lerpAmount);
    }
    public void NewUpdate(ushort tick, Vector3 position)
    {
        //If the tick count is less than or equal to the servers interpolation tick return out of the function
        if (tick <= NetworkManager.NetworkManagerInstance.InterpolationTick) return;
        //For each item in the future transforms list
        for (int i = 0; i < _futureTransformUpdates.Count; i++)
        {
            //If tick count is less than the tick count of the current list item insert a new entry with the tick and position passed to this function and return out
            if (tick < _futureTransformUpdates[i].Tick)
            {
                _futureTransformUpdates.Insert(i, new TransformUpdate(tick, position));
                return;
            }
        }
        //Else add it to the end of the list
        _futureTransformUpdates.Add(new TransformUpdate(tick, position));
    }
    #endregion
}
