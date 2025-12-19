using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

public class OrderGenerator : NetworkBehaviour
{
  public static OrderGenerator Singleton { get; set; }

  //Prefab -----------------
  public Canvas FishAndChipsTicket;
  public Canvas SushiTicket;
  public Canvas OnigiriTicket;
  public Canvas BugNuggetsTicket;
  public Canvas MeatMushroomsTicket;
  public Canvas TacoTicket;

  private Dictionary<EMeal, Canvas> m_ticketsCanvas;

  //Listes -----------------
  public List<OrderTicket> listTickets = new();
  // List<Plat> listActiveOrders;

  //V�rification de status d'envoi -----------------
  // bool orderSent;

  //Effets sonores pour les commandes -----------------
  AudioSource m_audiosource;
  public AudioClip audio_ReceivedOrder;
  public AudioClip audio_CompletedOrder;

  private void Awake()
  {
    if (Singleton == null) Singleton = this;
    else Destroy(gameObject);

    if (TryGetComponent(out AudioSource audioSourceComponent)) m_audiosource = audioSourceComponent;
    else m_audiosource = gameObject.AddComponent<AudioSource>();

    // orderSent = false;

    m_ticketsCanvas = new Dictionary<EMeal, Canvas>()
    {
      { EMeal.FishAndChips, FishAndChipsTicket},
      { EMeal.Sushi, SushiTicket},
      { EMeal.Onigiri, OnigiriTicket},
      { EMeal.BugNuggets, BugNuggetsTicket},
      { EMeal.MeatMushrooms, MeatMushroomsTicket},
      { EMeal.Taco, TacoTicket }
    };

    foreach (Canvas canvas in m_ticketsCanvas.Values)
      canvas.gameObject.SetActive(false);
  }

  [Rpc(SendTo.ClientsAndHost)]
  public void GenerateTicketRpc(EMeal _mealType)
  {
    Debug.Log($"[----- OrderGenerator -----] NEW ORDER TICKET ADDED of type {_mealType}");
    m_ticketsCanvas[_mealType].gameObject.SetActive(true);
    m_audiosource.PlayOneShot(audio_ReceivedOrder);
  }

  [Rpc(SendTo.ClientsAndHost)]
  public void RemoveTicketRpc(EMeal _mealType)
  {
#if UNITY_EDITOR
    Debug.Log($"[---- OrderGenerator ----] An order has been completed removing ticket!");
#endif
    m_ticketsCanvas[_mealType].gameObject.SetActive(false);
    m_audiosource.PlayOneShot(audio_CompletedOrder);
  }
}