using TMPro;
using UnityEngine;

public class ShowCodeInGame : MonoBehaviour
{
    private TextMeshProUGUI codeText;
    void Start()
    {
        codeText = GetComponent<TextMeshProUGUI>();

        // ดึงค่ารหัสที่เก็บไว้ใน MainMenu มาแสดง
        if (!string.IsNullOrEmpty(MainMenu.JoinCode))
        {
            codeText.text = "Room Code: " + MainMenu.JoinCode;
        }
        else
        {
            codeText.text = "Local Game"; // กรณีเล่นคนเดียวหรือ LAN
        }
    }
}
