using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkServer : NetworkBehaviour
{
  public static NetworkServer Singleton;
  private static readonly NetworkMatchmaking matchmaking = new();
  private static bool isMatchmakingRunning = false;
  private NetworkManager networkManager;

  [Header("NetworkPrefabs")]
  [SerializeField] private GameObject m_playerPrefab;
  [SerializeField] private GameObject m_playerHandPrefab;
  [SerializeField] private GameObject m_prefabMosquito;
  [SerializeField] private GameObject m_prefabFish;

  private Dictionary<ulong, GameObject[]> m_connectedPlayers;
  public static Action OnGameStarted;

#if UNITY_EDITOR
  [Header("Debug")]
  [SerializeField] private bool _DebugMode;
  [SerializeField] private GameObject _KnifePrefab;
  [SerializeField] private Button _MatchmakingStartButton;
#endif

  private void Awake()
  {
    if (Singleton == null)
    {
      Singleton = this;
      DontDestroyOnLoad(this);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    StartCoroutine(WaitForNetworkManagerStart());
  }

  private IEnumerator WaitForNetworkManagerStart()
  {
    while (NetworkManager.Singleton == null)
      yield return null;

    networkManager = NetworkManager.Singleton;
#if UNITY_EDITOR
    _MatchmakingStartButton.onClick.AddListener(StartMatchmaking);
#endif
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    base.OnNetworkPostSpawn();
    Debug.Log($"<color=green>[--- NetworkServer ---] Connected!</color>");

    if (IsServer)
    {
      networkManager.OnClientConnectedCallback += OnClientConnected;
      m_connectedPlayers = new Dictionary<ulong, GameObject[]>();
    }
  }

  private void OnClientConnected(ulong id)
  {
    Debug.Log($"<color=green>[--- NetworkServer ---] A client is connected! ID = {id}</color>");
    Debug.Log($"<color=green>[--- NetworkServer ---] Client count: {networkManager.ConnectedClients.Count}</color>");

    if (IsServer)
    {
      m_connectedPlayers[id] = new GameObject[3];

      GameObject _go_instance = Instantiate(m_playerPrefab);
      GameObject _leftHand = Instantiate(m_playerHandPrefab);
      GameObject _rightHand = Instantiate(m_playerHandPrefab);

      m_connectedPlayers[id][0] = _go_instance;
      m_connectedPlayers[id][1] = _leftHand;
      m_connectedPlayers[id][2] = _rightHand;

      _go_instance.GetComponent<NetworkPlayer>().Initialize(_go_instance, _leftHand, _rightHand);
    }

    if (networkManager.ConnectedClients.Count >= 2)
    {
      Debug.Log($"<color=green>[--- NetworkServer ---] Enough players connected starting game");
      if (IsServer)
      {
        Invoke(nameof(GameStart), 3.0f);
      }
    }
  }
  
  /*
    * Performs necessary actions when all clients 
    * are connected and ready to start.
  */
  private void GameStart()
  {
    if (!IsServer) return;

    /*
      Spawn all network object for physical player data
    */
    foreach (var pair in m_connectedPlayers)
    {
      foreach (var obj in pair.Value)
        obj.GetComponent<NetworkObject>().Spawn();
    }

    for (int i = 0; i < 5; i++)
    {
      GameObject _mosquitoInstance = Instantiate(m_prefabMosquito);
      GameObject _fishInstance = Instantiate(m_prefabFish);

      _mosquitoInstance.GetComponent<NetworkObject>().Spawn();
      _fishInstance.GetComponent<NetworkObject>().Spawn();
    }

    OnGameStartedRpc();
  }

  /*
    * Starts matchmaking to try to find a game
    * If no games can be found start a game as a host.
    * If one game is found connect as client.
  */
  public async void StartMatchmaking()
  {
    if (isMatchmakingRunning) return;
    isMatchmakingRunning = true;
    try
    {
      Debug.Log($"[--- NetworkServer ---] Running matchming");
      await matchmaking.RunMatchmaking();
    }
    catch (Exception ex)
    {
      Debug.LogError($"[--- NetworkServer ---] {ex.Message}");
      throw;
    }
    finally
    {
      isMatchmakingRunning = false;
    }
  }


  /* >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    >>> RPC 
  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< */

  /*
    * Update clients Meta controller data to server
  */
  [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
  public void OnClientUpdateServerRpc(NetworkPlayerDataUpdate data, RpcParams serverParams = default)
  {
    ulong senderID = serverParams.Receive.SenderClientId;
    NetworkPlayer _target = m_connectedPlayers[senderID][0].GetComponent<NetworkPlayer>();
    
    _target.RootUpdate(data.rootPosition, data.rootRotation);
    _target.LeftHandUpdate(data.leftHandPosition, data.leftHandRotation);
    _target.RightHandUpdate(data.rightHandPosition, data.rightHandRotation);
  }

  /*
    * Player wants to perform grab interaction
  */
  [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
  public void OnClientGrabActionServerRpc(int hand, RpcParams serverRpcParams = default)
  {
    ulong senderID = serverRpcParams.Receive.SenderClientId;
    NetworkPlayer _target = m_connectedPlayers[senderID][0].GetComponent<NetworkPlayer>();
    _target.OnGrabAction(hand);
  }

  /*
    * Player wants to perform ungrab interaction
  */
  [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
  public void OnClientUngrabActionServerRpc(int hand, RpcParams serverRpcParams = default)
  {
    ulong senderID = serverRpcParams.Receive.SenderClientId;
    NetworkPlayer _target = m_connectedPlayers[senderID][0].GetComponent<NetworkPlayer>();
    _target.OnUngrabAction(hand);
  }

  /*
    * Updates all clients that the game has started
  */
  [Rpc(SendTo.ClientsAndHost)]
  public void OnGameStartedRpc()
  {
    Debug.Log("<color=green>[----- NetworkManager -----] GAME HAS STARTED");
    OnGameStarted?.Invoke();
  }

  public void SpawnGameObjectInNetwork(GameObject _gameObject)
  {
    if (_gameObject.TryGetComponent(out NetworkObject networkObject))
    {
      networkObject.Spawn(true);
    }
  }

  public void DespawnGameObjectInNetwork(GameObject _gameObject)
  {
    if (!IsServer) return;

    if (_gameObject.TryGetComponent(out NetworkObject networkObject))
    {
      networkObject.Despawn(true);
    }
  }

  public void DeactivateGameObjectInNetwork(GameObject _gameObject)
  {
    if (_gameObject.TryGetComponent(out NetworkObject networkObject))
      Singleton.DeactivateObjRpc(networkObject.NetworkObjectId);
#if UNITY_EDITOR
    else
      Debug.LogWarning($"The object {_gameObject} is not a NetworkObject");
#endif
  }

  [Rpc(SendTo.ClientsAndHost)]
  private void DeactivateObjRpc(ulong id)
  {
    NetworkObject _target = NetworkManager.Singleton.SpawnManager.SpawnedObjects[id];
    _target.gameObject.SetActive(false);
  }
}
