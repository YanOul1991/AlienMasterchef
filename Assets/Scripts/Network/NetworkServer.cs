using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : NetworkBehaviour
{
  public static NetworkServer Singleton;
  private static readonly NetworkMatchmaking matchmaking = new();
  private static bool isMatchmakingRunning = false;
  private NetworkManager networkManager;

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

    while (NetworkManager.Singleton == null)
      yield return null;

    // yield return new WaitUntil(() => NetworkManager.Singleton != null && NetworkManager.didStart);
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
    }
  }

  private void OnClientConnected(ulong id)
  {
    Debug.Log($"<color=green>[--- NetworkServer ---] A client is connected!</color>");
    Debug.Log($"<color=green>[--- NetworkServer ---] Client count: {networkManager.ConnectedClients.Count}</color>");

    if (!IsServer)
      return;

    if (networkManager.ConnectedClients.Count >= 2)
    {
      Debug.Log($"<color=green>[--- NetworkServer ---] Enough players connected starting game");
      Invoke(nameof(SpawnObject), 3.0f);
    }
  }

#if UNITY_EDITOR
  private void SpawnObject()
  {
    if(!IsServer)
      return;

    GameObject _newKnife = Instantiate(_KnifePrefab);
    _newKnife.GetComponent<NetworkObject>().Spawn(true);
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
