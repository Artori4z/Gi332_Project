using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Entity : NetworkBehaviour
{
    [SerializeField] protected int Hp;
    protected float Speed;
    protected int Def;
    protected int AtkPower;
    protected Rigidbody rb;
    protected StopHelper helper;
    protected Vector3 currentVelocity;
    protected float SmoothTime;
    protected virtual void Awake()
    { 
        rb = GetComponent<Rigidbody>();
    }
    protected virtual void Start() { }
    protected virtual void Update() { }
    protected virtual void FixedUpdate() { }

    protected void Atk(GameObject target)
    {
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb == null) return;

        // ผลัก
        Vector3 pushDir = (target.transform.position - transform.position).normalized;
        targetRb.AddForce(pushDir * AtkPower * 2, ForceMode.Impulse);

        // ทำให้หนืดเพื่อจะหยุด
        targetRb.linearDamping = 5f;

        // สร้างคลาสเล็กๆ หรือเก็บ Component ไว้เพื่อ Reset 
        // แต่ถ้าเอาแบบง่ายที่สุดสำหรับมือใหม่ ให้สร้างสคริปต์จิ๋วไปแปะที่ศัตรูครับ
        helper = target.GetComponent<StopHelper>();
        if (helper == null) helper = target.AddComponent<StopHelper>();
        helper.ResetAfter(1f);
    }
    protected virtual void Move() { }
    public void TakeDamage(int damage)
    {
        // คำนวณพลังป้องกันตรงนี้เลย
        int finalDamage = damage - Def;
        if (finalDamage < 0) finalDamage = 0;

        Hp -= finalDamage;

        // เช็คตายที่ Server
        if (IsServer && Hp <= 0)
        {
            Die();
        }
    }
    protected void Die()
    {
        // 1. ต้องให้ Server เป็นคนตัดสินเท่านั้น เพื่อป้องกัน Client คำนวณพลาด
        if (!IsServer) return;

        // 2. เช็คว่า "เจ้าของ" ของตัวละครที่กำลังตายนี้คือใคร
        // ถ้า OwnerClientId == 0 โดยปกติคือ Host
        if (OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            // ถ้าคนตายคือคนที่มี ID เดียวกับ Server/Host
            AnnounceWinnerClientRpc("Player 2 Wins!");
        }
        else
        {
            // ถ้าคนตายไม่ใช่ Host (ก็คือ Client)
            AnnounceWinnerClientRpc("Player 1 Wins!");
        }

        // 3. ลบตัวละครออกจากระบบ Network
        GetComponent<NetworkObject>().Despawn();
    }

    [ClientRpc]
    private void AnnounceWinnerClientRpc(string message)
    {
        // แสดงผลบนหน้าจอของทุกคน
        Debug.Log(message);
        // ตัวอย่าง: WinText.text = message;
    }
}
