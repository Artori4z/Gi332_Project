using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkSceneLoader : NetworkBehaviour
{
    public void LoadGame()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
        }
    }
}