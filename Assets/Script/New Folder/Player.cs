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
    }
    protected override void Update() 
    {
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
        currentVelocity = Vector3.Lerp(currentVelocity, targetDirection, SmoothTime * Time.deltaTime);

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
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            Atk(collision.gameObject);
        }
    }
    
}
