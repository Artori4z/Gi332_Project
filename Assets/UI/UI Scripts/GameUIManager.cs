using Unity.Netcode;
using UnityEngine;

public class GameUIManager : NetworkBehaviour
{
    public static GameUIManager Instance;

    [Header("UI Panels")]
    public GameObject lobbyUi;
    public GameObject gameplayUi;
    public GameObject p1WinUi; // ฝั่ง Host ชนะ
    public GameObject p2WinUi; // ฝั่ง Client ชนะ

    private void Awake()
    {
        // สร้าง Singleton เพื่อให้เรียกใช้ง่ายๆ เช่น GameUIManager.Instance.ShowWinner(...)
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ฟังก์ชันปิด UI ทุกอย่าง
    public void HideAllUi()
    {
        if (lobbyUi) lobbyUi.SetActive(false);
        if (gameplayUi) gameplayUi.SetActive(false);
        if (p1WinUi) p1WinUi.SetActive(false);
        if (p2WinUi) p2WinUi.SetActive(false);
    }

    // ฟังก์ชันสั่งเปิดหน้าจอผู้ชนะ (ส่งคำสั่งไปทุกเครื่อง)
    [ClientRpc]
    public void ShowWinnerClientRpc(bool isP1Winner)
    {
        HideAllUi();

        if (isP1Winner)
        {
            if (p1WinUi) p1WinUi.SetActive(true);
        }
        else
        {
            if (p2WinUi) p2WinUi.SetActive(true);
        }
    }
}
