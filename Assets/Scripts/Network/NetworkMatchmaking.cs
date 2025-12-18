using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using MMPlayer = Unity.Services.Matchmaker.Models.Player;
using LobbyPlayer = Unity.Services.Lobbies.Models.Player;
using System.Collections;
using TMPro;


[Serializable]
public class NetworkMatchmaking
{
  [SerializeField] private string queueName = "QuickMatch";
  private LobbyBridge _lobbyBridge;
  // private TextMeshProUGUI textStatus;

  public async Task RunMatchmaking()
  {
    try
    {
      // Initialize unity Services if not already initialized
      if (UnityServices.State != ServicesInitializationState.Initialized)
      {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();
      }

      // textStatus.text = "Initialization...";
      Debug.Log("[--- NetworkMatchmaking ---] Initalization...");

      // Create a ticket 
      await CreateMatchmakingTicketAsync(queueName, 1.0f);

      // Create a new lobby bridge and keep a reference to it
      LobbyBridge lobbyBridge = new();
      _lobbyBridge = lobbyBridge;

      Debug.Log("[--- NetworkMatchmaking ---] Searching for lobby...");
      // textStatus.text = "Rercherche de lobby...";

      if (await lobbyBridge.ShouldHost())
      {
        Debug.Log("[--- NetworkMatchmaking ---] Creating new lobby...");
        // textStatus.text = "Creation d'un nouveau lobby...";
        // Create a join code using Unity Relay Service
        (string joinCode, _) = await RelayCreateHostAsync();
        Debug.Log($"MATCHMAKING_STATUS | Relay Join Code created: {joinCode}");

        // Update the Lobby's data to set the joinCode 
        await lobbyBridge.SetRelayJoinCode(joinCode);

        // Finally if not a client or server then start as host
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
          NetworkManager.Singleton.StartHost();

        NetworkManager network = NetworkManager.Singleton;
        Debug.Log($"MATCHMAKING_STATUS | Game Started as host\nIsServer: {network.IsServer}\nIsClient: {network.IsClient}\nIsListening: {network.IsListening}");
      }
      else
      {
        Debug.Log("[--- NetworkMatchmaking ---] Existing lobby found, connecting...");
        // textStatus.text = "Lobby trouve, connexion en cours...";
        // If found a lobby get its join code
        string joinCode = await lobbyBridge.GetRelayJoinCodeAsync();
        Debug.Log($"MATCHMAKING_STATUS | Found existing lobby join code: {joinCode}");

        await RelayJoinAsClientAsync(joinCode);

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
          Debug.Log("Starting As client");
          NetworkManager.Singleton.StartClient();
        }
      }
    }
    catch (Exception ex)
    {
      // textStatus.text = "Une erreur est survenu, veuillez ressayer.";
      Debug.LogError($"MATCHMAKING_STATUS | {ex.Message}");
      throw;
    }
  }

  private async Task<TicketStatusResponse> CreateMatchmakingTicketAsync(string queue, float pollSeconds)
  {
    List<MMPlayer> players = new()
    {
      new(AuthenticationService.Instance.PlayerId)
    };

    CreateTicketOptions createTicketOptions = new()
    {
      QueueName = queue,
      Attributes = new Dictionary<string, object>
      {
        {"region", "na-east"},
        {"canHost", true }
      }
    };

    CreateTicketResponse createTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, createTicketOptions);

    /* Poll Ticket */

    while (true)
    {
      // Rate limit 
      await Task.Delay(TimeSpan.FromSeconds(1.0f));

      // Poll ticket
      TicketStatusResponse ticketStatusResponse = await MatchmakerService.Instance.GetTicketAsync(createTicketResponse.Id);

      if (ticketStatusResponse == null) continue;

      if (ticketStatusResponse.Type == typeof(MultiplayAssignment))
      {
        MultiplayAssignment assignment = ticketStatusResponse.Value as MultiplayAssignment;
        switch (assignment?.Status)
        {
          case MultiplayAssignment.StatusOptions.Found: return ticketStatusResponse;
          case MultiplayAssignment.StatusOptions.Failed: throw new Exception($"[--- NetworkMaychmaking ---] Error: {assignment.Message}");
          case MultiplayAssignment.StatusOptions.Timeout: throw new Exception($"[--- NetworkMaychmaking ---] Error: {assignment.Message}");
          default: break;
        }
        ;
      }
      
      if (ticketStatusResponse.Type == typeof(MatchIdAssignment))
      {
        MatchIdAssignment assignment = ticketStatusResponse.Value as MatchIdAssignment;
        switch (assignment?.Status)
        {
          case MatchIdAssignment.StatusOptions.Found: return ticketStatusResponse;
          case MatchIdAssignment.StatusOptions.Failed: throw new Exception($"[--- NetworkMaychmaking ---] Error: {assignment.Message}");
          case MatchIdAssignment.StatusOptions.Timeout: throw new Exception($"[--- NetworkMaychmaking ---] Error: {assignment.Message}");
          default : break;
        };
      }
    }
  }

  private async Task<(string code, Allocation alloc)> RelayCreateHostAsync()
  {
    Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

    UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

    transport.SetRelayServerData(
      allocation.RelayServer.IpV4,
      (ushort)allocation.RelayServer.Port,
      allocation.AllocationIdBytes,
      allocation.Key,
      allocation.ConnectionData,
      allocation.ConnectionData,
      false
    );

    string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
    return (joinCode, allocation);
  }

  // Join Allocation
  private async Task RelayJoinAsClientAsync(string joinCode, int retryCount = 5, int delayMs = 500)
  {
    UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
    string lastTried = null;

    for (int attempts = 0; attempts < retryCount; attempts++)
    {
      string code = joinCode;
      if (attempts > 1 && _lobbyBridge != null && !string.IsNullOrEmpty(_lobbyBridge.Lobby?.Id))
      {
        try
        {
          // Get the lobby's join code.
          Lobby l = await LobbyService.Instance.GetLobbyAsync(_lobbyBridge.Lobby.Id);
          if (l.Data != null && l.Data.TryGetValue("relayCode", out DataObject dataObject) && !string.IsNullOrEmpty(dataObject.Value))
            code = dataObject.Value;
        }
        catch { }
      }

      // If the current code is the same as last time, retry.
      if (string.IsNullOrEmpty(code) || code == lastTried)
      {
        await Task.Delay(delayMs);
        continue;
      }

      lastTried = code;
      Debug.Log($"MATCHMAKING_STATUS | Trying JoinAllocation code: {code} - Attempt {attempts}/{retryCount}");

      try
      {
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);

        unityTransport.SetRelayServerData(
          joinAllocation.RelayServer.IpV4,
          (ushort)joinAllocation.RelayServer.Port,
          joinAllocation.AllocationIdBytes,
          joinAllocation.Key,
          joinAllocation.ConnectionData,
          joinAllocation.HostConnectionData,
          false
        );

        Debug.Log($"MATCHMAKING_STATUS | Success Allocation With code: {code}");
        return;
      }
      catch (Exception ex)
      {
        // If JoinAllocationAsync Failed and still have retries left, retry.
        string msg = ex.Message ?? string.Empty;
        if (msg.IndexOf("not found", StringComparison.OrdinalIgnoreCase) >= 0 && attempts < retryCount)
        {
          Debug.LogWarning($"MATCHMAKING_STATUS | CLIENT | Join Failed. Retrying...");
          await Task.Delay(delayMs);
          continue;
        }

        // If all retry attempts have been used then thrown a NetworkException
        throw new Exception("[--- NetworkMaychmaking ---] Client has failed to join");
      }
    }

    throw new Exception("[--- NetworkMaychmaking ---]Join failed. Could not find relay code");
  }

  private class LobbyBridge
  {
    public Lobby Lobby { get; private set; }

    public async Task<bool> ShouldHost()
    {
      // Try to quick join an Existing Lobby
      int attempts = 0;
      while (attempts < 5)
      {
        try
        {
          // If sucessfull, update lobby data, and do not become a host
          Lobby = await LobbyService.Instance.QuickJoinLobbyAsync();
          return false;
        }
        catch
        {
          attempts++;
          await Task.Delay(500);
        }
      }

      // If cannot find a Lobby create a new one and become a host (Lobby max player count = 4)
      Lobby = await LobbyService.Instance.CreateLobbyAsync("MyLobby", 4);
      return true;
    }

    // Update the Lobby's Relay Code
    public async Task SetRelayJoinCode(string joinCode)
    {
      Lobby = await LobbyService.Instance.UpdateLobbyAsync(Lobby.Id, new UpdateLobbyOptions
      {
        Data = new Dictionary<string, DataObject>
        {
          {"relayCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
        }
      });

      Debug.Log($"MATCHMAKING_STATUS_LOBBY_BRIDGE | Lobby's data with join code has been updated");
    }

    public async Task<string> GetRelayJoinCodeAsync()
    {
      while (true)
      {
        Lobby l = await LobbyService.Instance.GetLobbyAsync(Lobby.Id);

        if (l.Data != null && l.Data.TryGetValue("relayCode", out DataObject dataObject) && !string.IsNullOrEmpty(dataObject.Value))
          return dataObject.Value;

        await Task.Delay(400);
      }
    }
  }
}
