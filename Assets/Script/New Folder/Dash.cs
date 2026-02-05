using UnityEngine;

public class Dash : Player
{
    private float dashForce = 20f;
    protected override void Start()
    {
        Hp = 100;
        Speed = 10f;
        Def = 1;
        AtkPower = 10;
    }

    protected override void Class()
    {
        // เช็กปุ่ม Jump (หรือเปลี่ยนเป็นปุ่มอื่นใน Action Asset)
        if (Controls.Player.Jump.WasPressedThisFrame() && Time.time >= CanCast)
        {
            DoDash();
            CanCast = Time.time + Cooldown; // ตั้งเวลา Cooldown
        }
    }
    private void DoDash()
    {
        Vector3 dashDir = new Vector3(MoveInput.x, 0f, MoveInput.y).normalized;
        if (dashDir == Vector3.zero)
        {
            dashDir = transform.forward;
        }
        rb.AddForce(dashDir * dashForce, ForceMode.Impulse);
    }
}
