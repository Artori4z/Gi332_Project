using Unity.Netcode;
using UnityEngine;

public class Player : Entity
{
    protected InputSystem_Actions Controls;
    protected Vector2 MoveInput;
    protected float Cooldown = 5f;
    protected float CanCast = 0f;

    protected virtual void Class() { }
    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Controls = new InputSystem_Actions();
        Controls.Enable();
    }
    protected override void Update()
    {
        if (!IsOwner) return;
        MoveInput = Controls.Player.Move.ReadValue<Vector2>();
        Class();
    }
    protected override void FixedUpdate()
    {
        Vector3 movement = new Vector3(MoveInput.x, 0f, MoveInput.y);
        // 1. หา "เป้าหมาย" ของทิศทางที่อยากไป
        Vector3 targetDirection = new Vector3(MoveInput.x, 0, MoveInput.y).normalized;

        // 2. ใช้ Lerp ค่อยๆ ปรับความเร็วปัจจุบัน (currentVelocity) ให้ไปหาเป้าหมาย
        // วิธีนี้จะทำให้เวลาปล่อยปุ่ม ค่าจะไม่กลายเป็น 0 ทันที แต่จะค่อยๆ ลดลงจนหยุด
        currentVelocity = Vector3.Lerp(currentVelocity, targetDirection, SmoothTime * Time.fixedDeltaTime);

        // 3. บวกตำแหน่งด้วยความเร็วที่กำลังไหลอยู่
        transform.position += currentVelocity * Speed * Time.deltaTime;

        if (rb != null)
        {
            rb.mass = Def;
        }

    }
    protected void OnEnable()
    {
        Controls.Player.Enable();
    }
    protected void OnDisable()
    {
        Controls.Player.Disable();
    }
    protected virtual void OnCollisionEnter(Collision collision)
    {
        // เฉพาะ "เจ้าของ" ตัวละครที่วิ่งไปชนเท่านั้นที่มีสิทธิ์สั่ง (ป้องกันการรันซ้ำซ้อน)
        if (!IsOwner) return;
        if (collision.gameObject.CompareTag("Destory"))
        {
            Die();
        }
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {

            var targetNetObj = collision.gameObject.GetComponent<NetworkObject>();
            if (targetNetObj != null)
            {
                // คำนวณทิศทางจากเครื่องเราส่งไปให้ Server
                Vector3 pushDir = (collision.transform.position - transform.position).normalized;
                RequestAtkServerRpc(targetNetObj.NetworkObjectId, pushDir);
            }
        }
    }

    [ServerRpc]
    void RequestAtkServerRpc(ulong targetId, Vector3 direction)
    {
        // ส่งสัญญาณไปหา Client ทุกเครื่อง (รวมถึง Host) ว่าให้จัดการแรงผลักตัวละครตัวนี้
        ApplyAtkEffectClientRpc(targetId, direction);
    }

    [ClientRpc]
    void ApplyAtkEffectClientRpc(ulong targetId, Vector3 direction)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetId, out var targetNetObj))
        {
            Entity targetEntity = targetNetObj.GetComponent<Entity>();
            Rigidbody targetRb = targetNetObj.GetComponent<Rigidbody>();

            if (targetEntity != null)
            {
                // เรียกใช้ฟังก์ชัน public ที่เราสร้างไว้
                // ส่งแค่ค่า AtkPower ไป เดี๋ยว Entity ไปลบ Def เองข้างใน
                targetEntity.TakeDamage(AtkPower);
            }

            if (targetRb != null)
            {
                targetRb.AddForce(direction * AtkPower, ForceMode.Impulse);
                targetRb.linearDamping = 5f;
            }
        }

    }
}
