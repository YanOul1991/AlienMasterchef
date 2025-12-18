using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderGenerator : MonoBehaviour
{
    public OrderManager orderManager;
    public Canvas prefabOrderTicket;
    public List<OrderTicket> listTickets = new List<OrderTicket>();
    List<Plat> listActiveOrders;
    bool orderSent;

    void Start()
    {
        orderSent = false;
    }

    void Update()
    {
        //Générer des commandes aux 30 secondes, pour un maximum de 3 commandes à la fois
        listActiveOrders = orderManager.GetActiveOrders();
        int nbCommandes = listActiveOrders.Count;

        //S'il y a moins de 3 commandes, on démarre la coroutine pour 
        if (nbCommandes < 3 && !orderSent)
        {
            orderSent = true;
            StartCoroutine(GenererCommande());
        }

        //CompletionCommande();
        //foreach(Ingredient i in (RecipeDatabase.GetRecipe(listActiveOrders[0].mealType)).requiredIngredients)
        //{
        //    //Liste des ingrédients requis
        //    print(i.state);
        //    print(i.baseIngredient);
        //}
        //print((RecipeDatabase.GetRecipe(listActiveOrders[0].mealType)).requiredIngredients);

        Debug.Log("<color=blue>" + nbCommandes + "</color>");
    }

    //Transformer les types de EMeal en tableau
    static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(Random.Range(0, A.Length));
        return V;
    }

    // Coroutine pour la création de commandes aux 30 secondes
    private IEnumerator GenererCommande()
    {
        //Création de la commande aléatoirement
        EMeal commandeAleatoire = GetRandomEnum<EMeal>();
        orderManager.CreateOrder(commandeAleatoire);

        //Affichage de la commande au comptoir (UI)

        bool commandePasse = false;

        for (int i = 0; i < listTickets.Count && i < listActiveOrders.Count; i++)
        {
            OrderTicket t = listTickets[i];

            if (t.ticket == null && !commandePasse)
            {
                Canvas commandeGeneree = Instantiate(prefabOrderTicket, t.transform.position, t.transform.rotation);
                commandeGeneree.gameObject.SetActive(true);
                t.plat = listActiveOrders[i];
                t.ticket = commandeGeneree;
                commandePasse = true;
            }
            Debug.Log($"Item at index {i}: {t.plat}");
        }

        //Puis on attend 30 secondes
        yield return new WaitForSeconds(30f);
        orderSent = false;
    }

    //Ça fonctionne, juste définir la méthode de vérification
    void CompletionCommande(Plat order)
    {
        orderManager.CompleteOrder(order);
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