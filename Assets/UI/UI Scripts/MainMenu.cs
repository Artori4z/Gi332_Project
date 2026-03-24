using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MainMenu : MonoBehaviour
{
    public GameObject Option, Menu, Muti, Play;

    [Header("Spawn Settings")]
    public Vector3 hostSpawnPos = new Vector3(-3.28f, 0.55f, 10.63f);
    public Vector3 clientSpawnPos = new Vector3(0f, 0.55f, 0f);

    public void Awake() => ShowPanel(Menu);

    private void ShowPanel(GameObject panelToShow)
    {
        Menu.SetActive(panelToShow == Menu);
        Muti.SetActive(panelToShow == Muti);
        Option.SetActive(panelToShow == Option);
        Play.SetActive(panelToShow == Play);
    }

    // --- Network Logic ---

    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            // ลงทะเบียน callback สำหรับตั้งจุดเกิด
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Pending = false;

        // ถ้าเป็นคนเปิดห้อง (Host) จะได้ ID 0 หรือ ID ของตัวเอง
        if (request.ClientNetworkId == NetworkManager.Singleton.LocalClientId)
        {
            response.Position = hostSpawnPos;
        }
        else
        {
            response.Position = clientSpawnPos;
        }
        response.Rotation = Quaternion.identity;
    }

    public void StartHostButton()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            // เปลี่ยนฉากผ่าน NetworkSceneManager เท่านั้น
            NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }
    }

    public void StartClientButton()
    {
        NetworkManager.Singleton.StartClient();
    }

    // --- UI Buttons ---
    public void PlayGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    public void Options() => ShowPanel(Option);
    public void Multiplayer() => ShowPanel(Muti);
    public void MainMenuButton() => ShowPanel(Menu);
    public void PlayButton() => ShowPanel(Play);
    public void QuitGame() { Debug.Log("Quit!"); Application.Quit(); }
}
