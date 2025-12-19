using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OrderGenerator : MonoBehaviour
{
    //OrderManager -----------------
    public OrderManager orderManager;

    //Prefab -----------------
    public Canvas FishAndChipsTicket;
    public Canvas SushiTicket;
    public Canvas OnigiriTicket;
    public Canvas BugNuggetsTicket;
    public Canvas MeatMushroomsTicket;
    public Canvas TacoTicket;

    //Listes -----------------
    public List<OrderTicket> listTickets = new List<OrderTicket>();
    List<Plat> listActiveOrders;

    //V�rification de status d'envoi -----------------
    bool orderSent;

    //Effets sonores pour les commandes -----------------
    AudioSource audiosource;
    public AudioClip audio_ReceivedOrder;
    public AudioClip audio_CompletedOrder;

    void Start()
    {
        orderSent = false;
        audiosource = GetComponent<AudioSource>();
    }

    void Update()
    {
        //G�n�rer des commandes aux 30 secondes, pour un maximum de 3 commandes � la fois
        listActiveOrders = orderManager.GetActiveOrders();
        int nbCommandes = listActiveOrders.Count;

        //S'il y a moins de 3 commandes, on d�marre la coroutine pour 
        if (nbCommandes < 3 && !orderSent)
        {
            orderSent = true;
            StartCoroutine(GenererCommande());
        }

        Debug.Log("<color=blue>" + nbCommandes + "</color>");
    }

    //Transformer les types de EMeal en tableau
    static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(Random.Range(0, A.Length));
        return V;
    }

    // Coroutine pour la cr�ation de commandes aux 30 secondes
    private IEnumerator GenererCommande()
    {
        //Cr�ation de la commande al�atoirement
        EMeal commandeAleatoire = GetRandomEnum<EMeal>();
        // orderManager.CreateOrder(commandeAleatoire);

        //Affichage de la commande au comptoir (UI)
        bool commandePasse = false;

        for (int i = 0; i < listTickets.Count && i < listActiveOrders.Count; i++)
        {
            OrderTicket t = listTickets[i];

            if (t.ticket == null && !commandePasse)
            {
                foreach (Ingredient ingredient in (RecipeDatabase.GetRecipe(commandeAleatoire).requiredIngredients))
                {
                    //Liste des ingr�dients requis
                    print(ingredient.state);
                    print(ingredient.baseIngredient);
                }

                t.plat = listActiveOrders[i];

                Canvas commandeGenere = null;

                switch (t.plat.mealType)
                {
                    case EMeal.FishAndChips:
                        commandeGenere = Instantiate(FishAndChipsTicket, t.transform.position, t.transform.rotation);
                        break;

                    case EMeal.Sushi:
                        commandeGenere = Instantiate(SushiTicket, t.transform.position, t.transform.rotation);
                        break;

                    case EMeal.Onigiri:
                        commandeGenere = Instantiate(OnigiriTicket, t.transform.position, t.transform.rotation);
                        break;

                    case EMeal.BugNuggets:
                        commandeGenere = Instantiate(BugNuggetsTicket, t.transform.position, t.transform.rotation);
                        break;

                    case EMeal.MeatMushrooms:
                        commandeGenere = Instantiate(MeatMushroomsTicket, t.transform.position, t.transform.rotation);
                        break;

                    case EMeal.Taco:
                        commandeGenere = Instantiate(TacoTicket, t.transform.position, t.transform.rotation);
                        break;

                    default:
                        Debug.LogError("Meal type non g�r� : " + t.plat.mealType);
                        break;
                }

                // Assignation finale
                if (commandeGenere != null)
                {
                    t.ticket = commandeGenere;
                    commandeGenere.gameObject.SetActive(true);
                }

                commandePasse = true;
            }

            Debug.Log($"Item at index {i}: {t.plat}");
        }

        //GESTION DU SON
        audiosource.PlayOneShot(audio_ReceivedOrder);


        //Puis on attend 30 secondes
        yield return new WaitForSeconds(30f);
        orderSent = false;
    }

    //�a fonctionne, juste d�finir la m�thode de v�rification
    void CompletionCommande(Plat order)
    {
        orderManager.CompleteOrder(order);

        //GESTION DU SON
        audiosource.PlayOneShot(audio_CompletedOrder);

        foreach (OrderTicket t in listTickets)
        {
            if (t.plat.IsCompleted())
            {
                Destroy(t.ticket.gameObject);
                t.ticket = null;
                t.plat = null;
            }
        }
    }
}