using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class ReturnToMenu : MonoBehaviour
{
    public void BackToMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            if (NetworkManager.Singleton.IsHost ||
                NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        SceneManager.LoadScene("MainMenu");
    }
}