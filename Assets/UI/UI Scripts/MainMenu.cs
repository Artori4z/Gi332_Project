using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject Option, Menu, Muti, Play, Host, Client;
    [Header("Relay UI")]
    public TMP_InputField joinCodeInput;
    [Header("Spawn Settings")]
    public Vector3 hostSpawnPos = new Vector3(-3.28f, 0.55f, 10.63f);
    public Vector3 clientSpawnPos = new Vector3(0f, 0.55f, 0f);
    public static string JoinCode;
    public void Awake() => ShowPanel(Menu);

    async void Start()
    {
        // เริ่มต้นระบบ Unity Services ตั้งแต่เปิดเกม
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (System.Exception e) { Debug.LogError(e); }

        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
    }

    private void ShowPanel(GameObject panelToShow)
    {
        Menu.SetActive(panelToShow == Menu);
        Client.SetActive(panelToShow == Client);
        Host.SetActive(panelToShow == Host);
        Muti.SetActive(panelToShow == Muti);
        Option.SetActive(panelToShow == Option);
        Play.SetActive(panelToShow == Play);
    }
    public async void StartRelayHost()
    {
        try
        {
            // 1. ขอรหัส Relay
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            // 2. ตั้งค่า Transport
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData
            );

            // 3. เริ่ม Host และเปลี่ยนฉากทันที!
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Gameplay", LoadSceneMode.Single);
        }
        catch (System.Exception e) { Debug.LogError(e); }
    }

    // --- ส่วนของ Client (เหมือนเดิม) ---
    public async void StartRelayClient()
    {
        try
        {
            string code = joinCodeInput.text;
            if (string.IsNullOrEmpty(code)) return;

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code);
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4, (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes, joinAllocation.Key,
                joinAllocation.ConnectionData, joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
        }
        catch (System.Exception e) { Debug.LogError($"Relay Client Error: {e.Message}"); }
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Position = (request.ClientNetworkId == NetworkManager.Singleton.LocalClientId) ? hostSpawnPos : clientSpawnPos;
        response.Rotation = Quaternion.identity;
        response.Pending = false;
    }

    // --- UI Buttons ---
    public void ClientButton() => ShowPanel(Client);
    public void HostButton() => ShowPanel(Host);
    public void Options() => ShowPanel(Option);
    public void Multiplayer() => ShowPanel(Muti);
    public void MainMenuButton() => ShowPanel(Menu);
    public void PlayButton() => ShowPanel(Play);
    public void QuitGame() => Application.Quit();
}