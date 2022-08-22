using UnityEngine; //Connect to Unity Engine

public class GameLogic : MonoBehaviour
{
    #region Variables
    //There can be only one!!! We will have a private instance and a public property to control the instance
    private static GameLogic _gameLogicInstance;
    public static GameLogic GameLogicInstance
    {
        //Property read is public by default and reads the set instance
        get => _gameLogicInstance;
        private set
        {
            //Property private write sets instance to the value if the instance is null
            if (_gameLogicInstance == null)
            {
                _gameLogicInstance = value;
            }
            //If we already have an instance log an error and kill the new instance
            else if (_gameLogicInstance != value)
            {
                Debug.LogWarning($"{nameof(NetworkManager)} instance already exists, destroy duplicate! THERE CAN BE ONLY ONE!");
                Destroy(value);
            }
        }
    }
    [Header("Prefabs")]
    [Tooltip("Add the prefab to use to represent the online players")]
    [SerializeField] private GameObject _playerPrefab;
    [Tooltip("Add the prefab to use to represent the locally controlled player")]
    [SerializeField] private GameObject _localPlayerPrefab;
    //Public properties to allow other classes to retrieve but not chamge the prefabs
    public GameObject PlayerPrefab => _playerPrefab;
    public GameObject LocalPlayerPrefab => _localPlayerPrefab;
    #endregion
    #region Setup
    private void Awake()
    {
        //Sets the singleton instance to this
        GameLogicInstance = this;
    }
    #endregion
}
