using System.Runtime.CompilerServices;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class InGameController : NetworkBehaviour
{
    [Header("Spawn Points")]
    public Transform spawnPointHost;   // ลากจุดวาง Host มาใส่
    public Transform spawnPointClient; // ลากจุดวาง Client มาใส่

    // ฟังก์ชันสำหรับผูกกับปุ่ม OnClick
    public void TeleportPlayers()
    {
        // ต้องเป็น Host เท่านั้นที่สั่งย้ายที่ได้ เพื่อให้ตำแหน่งซิงค์กันทุกเครื่อง
        if (!IsHost) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            if (playerObject != null)
            {
                // ตรวจสอบว่าเป็นเครื่อง Host หรือเครื่อง Client
                Vector3 targetPos = (client.ClientId == NetworkManager.Singleton.LocalClientId)
                                    ? spawnPointHost.position
                                    : spawnPointClient.position;

                // สั่งวาร์ป
                TeleportPlayer(playerObject, targetPos);
            }
        }
    }

    private void TeleportPlayer(NetworkObject player, Vector3 targetPosition)
    {
        // ปิด CharacterController ชั่วคราว (ถ้ามี) เพื่อป้องกันฟิสิกส์ขัดขวางการวาร์ป
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        player.transform.position = targetPosition;

        if (cc != null) cc.enabled = true;
    }
}
