using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : NetworkBehaviour
{
  public static NetworkServer Singleton;
  private static readonly NetworkMatchmaking matchmaking = new();
  private static bool isMatchmakingRunning = false;
  private NetworkManager networkManager;

  [Header("NetworkPrefabs")]
  [SerializeField] private GameObject m_playerPrefab;
  [SerializeField] private GameObject m_playerHandPrefab;

  private Dictionary<ulong, GameObject[]> m_connectedPlayers;

  public static Action OnGameStarted;

#if UNITY_EDITOR
  [Header("Debug")]
  [SerializeField] private bool _DebugMode;
  [SerializeField] private GameObject _KnifePrefab;
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
    Debug.Log($"[--- NetworkServer ---] Waiting for NetworkManager to be ready...");

    while (NetworkManager.Singleton == null) yield return null;

    networkManager = NetworkManager.Singleton;
    Debug.Log($"[--- NetworkServer ---] NetworkManager is ready.");
    Invoke(nameof(StartMatchmaking), 5.0f);
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

      _go_instance.GetComponent<NetworkPlayer>().Initialize(_leftHand, _rightHand);
    }

    if (networkManager.ConnectedClients.Count >= 2)
    {
      Debug.Log($"<color=green>[--- NetworkServer ---] Enough players connected starting game");
      if (IsServer)
      {
        Invoke(nameof(SpawnObject), 3.0f);
      }
    }
  }

  [ServerRpc]
  public void OnClientUpdateServerRpc(NetworkPlayerDataUpdate data, ServerRpcParams serverParams = default)
  {
    ulong senderID = serverParams.Receive.SenderClientId;
    NetworkPlayer _target = m_connectedPlayers[senderID][0].GetComponent<NetworkPlayer>();

    _target.LeftHandUpdate(data.leftHandPosition, data.leftHandRotation);
    _target.RightHandUpdate(data.rightHandPosition, data.rightHandRotation);
  }

  [ServerRpc]
  public void OnClientGrabActionServerRpc(int hand, ServerRpcParams serverRpcParams = default)
  {
    ulong senderID = serverRpcParams.Receive.SenderClientId;
    NetworkPlayer _target = m_connectedPlayers[senderID][0].GetComponent<NetworkPlayer>();
    _target.OnGrabAction(hand);
  }

  [ServerRpc]
  public void OnClientUngrabActionServerRpc(int hand, ServerRpcParams serverRpcParams = default)
  {
    ulong senderID = serverRpcParams.Receive.SenderClientId;
    NetworkPlayer _target = m_connectedPlayers[senderID][0].GetComponent<NetworkPlayer>();
    _target.OnUngrabAction(hand);
  }

  [Rpc(SendTo.ClientsAndHost)]
  public void OnGameStartedRpc()
  {
    OnGameStarted?.Invoke();
  }

#if UNITY_EDITOR
  private void SpawnObject()
  {
    if (!IsServer)
      return;

    foreach (var pair in m_connectedPlayers)
    {
      foreach (var obj in pair.Value)
        obj.GetComponent<NetworkObject>().Spawn();
    }

    OnGameStartedRpc();
#endif
  }

  private async void StartMatchmaking()
  {
    if (isMatchmakingRunning)
      return;

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
}
