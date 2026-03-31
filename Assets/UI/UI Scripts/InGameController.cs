using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameController : NetworkBehaviour
{
    [Header("Spawn Points")]
    public Transform spawnPointHost;   // ลากจุดวาง Host มาใส่
    public Transform spawnPointClient; // ลากจุดวาง Client มาใส่
    [Header("Host UI (Left)")]
    public TextMeshProUGUI hpTextHost;
    public TextMeshProUGUI cdTextHost;

    [Header("Client UI (Right)")]
    public TextMeshProUGUI hpTextClient;
    public TextMeshProUGUI cdTextClient;
    // ฟังก์ชันสำหรับผูกกับปุ่ม OnClick
    void Update()
    {
        // วนลูปหาผู้เล่นทุกคนที่ออนไลน์อยู่
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null)
            {
                Player playerScript = client.PlayerObject.GetComponent<Player>();
                if (playerScript != null)
                {
                    // เช็คว่าเป็น Host (ID 0) หรือ Client (ID อื่นๆ)
                    bool isHost = client.ClientId == 0;
                    UpdatePlayerDisplay(isHost, playerScript.NetworkHp.Value, playerScript.NetworkCD.Value);
                }
            }
        }
    }

    void UpdatePlayerDisplay(bool isHost, int hp, float cd)
    {
        if (isHost)
        {
            if (hpTextHost) hpTextHost.text = "HP: " + hp;
            if (cdTextHost) cdTextHost.text = "CD: " + cd.ToString("F1"); // ทศนิยม 1 ตำแหน่ง
        }
        else
        {
            if (hpTextClient) hpTextClient.text = "HP: " + hp;
            if (cdTextClient) cdTextClient.text = "CD: " + cd.ToString("F1");
        }
    }
    public void TeleportPlayersButton()
    {
        // เช็คความปลอดภัย: เฉพาะ Host เท่านั้นที่มีสิทธิ์กดปุ่มนี้แล้วเกิดผล
        if (!IsHost) return;

        // เรียกฟังก์ชันเพื่อย้ายตำแหน่งทุกคน
        ExecuteTeleport();
    }
    private void ExecuteTeleport()
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                // กำหนดตำแหน่งเป้าหมายแยกตาม ID (Host=0, Client=อื่นๆ)
                Vector3 targetPos = (client.ClientId == 0)
                                    ? spawnPointHost.position
                                    : spawnPointClient.position;

                // ส่งคำสั่งวาร์ปไปยังเครื่องที่เป็นเจ้าของตัวละครนั้นๆ
                MovePlayerClientRpc(playerObject.NetworkObjectId, targetPos);
            }
        }
    }
    // --- ฟังก์ชันผูกกับปุ่ม "สร้างห้องใหม่" ---
    public void BackToMenuButton()
    {
        // 1. ถ้าคนกดเป็น Host ให้บอกทุกคนในห้องว่า "เลิกเล่นแล้วนะ"
        if (IsServer)
        {
            BackToMenuClientRpc();
        }
        else
        {
            // 2. ถ้า Client กดเอง ให้ตัวเองหลุดคนเดียว
            LeaveGame();
        }
    }

    [ClientRpc]
    private void BackToMenuClientRpc()
    {
        // คำสั่งนี้จะทำงานที่เครื่องทุกคน (รวมถึง Host ด้วย)
        LeaveGame();
    }

    private void LeaveGame()
    {
        // ตัดการเชื่อมต่อจาก Network
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // โหลดฉากเมนูหลัก (เปลี่ยนชื่อ "MainMenu" ให้ตรงกับชื่อ Scene ของคุณ)
        SceneManager.LoadScene("MainMenu");

        Debug.Log("Left the game session.");
    }
    [ClientRpc]
    private void MovePlayerClientRpc(ulong networkObjectId, Vector3 targetPosition)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var targetNetObj))
        {
            // ปิดตัวขัดขวางฟิสิกส์ (CharacterController)
            var cc = targetNetObj.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // ถ้ามี NetworkTransform ให้ใช้คำสั่ง Teleport ของมัน
            var nt = targetNetObj.GetComponent<NetworkTransform>();
            if (nt != null)
            {
                nt.Teleport(targetPosition, targetNetObj.transform.rotation, targetNetObj.transform.localScale);
            }
            else
            {
                // ถ้าไม่มีให้ย้ายตรงๆ
                targetNetObj.transform.position = targetPosition;
            }

            // เปิด CharacterController กลับมา
            if (cc != null) cc.enabled = true;
        }
    }
}
