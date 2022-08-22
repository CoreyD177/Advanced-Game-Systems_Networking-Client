using UnityEngine; //Connect to Unity Engine

public class PlayerAnimationManager : MonoBehaviour
{
    #region Variables
    [Header("Class References")]
    [Tooltip("Drag the Animator component into this box to store it for use")]
    [SerializeField] private Animator _animator;
    [Header("Thresholds")]
    [Tooltip("Set the normal walk speed of the player so we can use it to calculate the sprint threshold")]
    [SerializeField] private float _playerMoveSpeed = 5f;
    //Sprint threshold to determine when we should switch from walk animation to sprint animation and back
    private float _sprintThreshold;
    //Last position variable to allow is to determine the distance the player has moved
    private Vector3 _lastPosition;
    #endregion
    #region Animation
    private void Start()
    {
        //If animator is not connected retrieve the animator from the game object
        if (_animator == null) _animator = GetComponent<Animator>();
        //Make the sprint threshold 1.5 times the walk speed. Multiply by Time.deltaTime to keep it consistent with actual movement.
        _sprintThreshold = _playerMoveSpeed * 1.5f * Time.fixedDeltaTime;
    }
    public void AnimateBasedOnSpeed()
    {
        //Set the y axis of the last position to the current y position so jumping or falling doesn't trigger the animation
        _lastPosition.y = transform.position.y;
        //Determine the distance between the current position and the last recorded position to determine how far the object moved
        float distanceMoved = Vector3.Distance(transform.position, _lastPosition);
        //If we moved set the IsMoving bool to true to trigger the walking animation
        _animator.SetBool("IsMoving", distanceMoved > 0.01f);
        //If we moved further than the sprint threshold set the IsSprinting bool to true to trigger the sprinting animation
        _animator.SetBool("IsSprinting", distanceMoved > _sprintThreshold);
        //Set the last position to the current position ready for the next calculation
        _lastPosition = transform.position;
    }
    #endregion
}
