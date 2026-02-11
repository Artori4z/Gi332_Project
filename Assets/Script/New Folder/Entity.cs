using System.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected int Hp;
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
        Hp -= AtkPower - Def;
        Rigidbody targetRb = target.GetComponent<Rigidbody>();
        if (targetRb == null) return;

        // ผลัก
        Vector3 pushDir = (target.transform.position - transform.position).normalized;
        targetRb.AddForce(pushDir * AtkPower, ForceMode.Impulse);

        // ทำให้หนืดเพื่อจะหยุด
        targetRb.linearDamping = 5f;

        // สร้างคลาสเล็กๆ หรือเก็บ Component ไว้เพื่อ Reset 
        // แต่ถ้าเอาแบบง่ายที่สุดสำหรับมือใหม่ ให้สร้างสคริปต์จิ๋วไปแปะที่ศัตรูครับ
        helper = target.GetComponent<StopHelper>();
        if (helper == null) helper = target.AddComponent<StopHelper>();
        helper.ResetAfter(1f);
    }
    protected virtual void Move() { }
    protected virtual void Die() { }
    // Coroutine สำหรับค่อยๆ ทำให้ศัตรูหยุด
}
