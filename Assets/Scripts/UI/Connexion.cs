using UnityEngine;
using UnityEngine.UI;

public class Connexion : MonoBehaviour
{
    public Button bouton;
    void Start()
    {
        bouton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        NetworkServer.Singleton.StartMatchmaking();
        Invoke("DesactiveBouton", 1f);
        
    }

    void DesactiveBouton()
    {
        bouton.gameObject.SetActive(false);
    }
}
