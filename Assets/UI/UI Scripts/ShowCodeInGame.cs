using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ShowCodeInGame : NetworkBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI codeDisplayPrefab; // ลาก Text ที่จะโชว์รหัสมาใส่
    public GameObject hostOnlyControls;      // ลาก Parent ของปุ่มที่มีเฉพาะ Host มาใส่ (ถ้ามี)

    public override void OnNetworkSpawn()
    {
        // 1. จัดการเรื่องการแสดงรหัส (แสดงให้ทั้ง Host และ Client เห็น)
        if (codeDisplayPrefab != null)
        {
            if (!string.IsNullOrEmpty(MainMenu.JoinCode))
            {
                codeDisplayPrefab.text = "Room Code: " + MainMenu.JoinCode;
            }
            else
            {
                codeDisplayPrefab.text = "Local / LAN Mode";
            }
        }

        // 2. จัดการเรื่องปุ่มเฉพาะ Host
        if (hostOnlyControls != null)
        {
            // ถ้าไม่ใช่ Host (เป็น Client) ให้ปิด Object นี้ทิ้งไปเลย
            hostOnlyControls.SetActive(IsHost);
        }
    }

    // ฟังก์ชันสำหรับปุ่ม Copy (กดได้ทุกคน)
    public void CopyJoinCode()
    {
        string code = MainMenu.JoinCode;
        if (!string.IsNullOrEmpty(code))
        {
            GUIUtility.systemCopyBuffer = code;
            Debug.Log("Copied Code: " + code);
        }
    }
    public void StartGame()
    {
        hostOnlyControls.SetActive(false);
        codeDisplayPrefab.gameObject.SetActive(false);
    }
}
