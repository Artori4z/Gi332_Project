using UnityEngine;

public class Jumper : Player
{
    private float jumpForce = 10f;
    private bool isGrounded;
    protected override void Start()
    {
        MaxHp = 100;
        Speed = 10f;
        Def = 1;
        AtkPower = 20;
        SmoothTime = 3f;
        Cooldown = 2f;
    }
    protected override void Class()
    {
        if (Controls.Player.Jump.WasPressedThisFrame() && isGrounded && Time.time >= CanCast)
        {
            Jump();
            CanCast = Time.time + Cooldown;
        }
    }
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (NetworkHp.Value <= 0)
        {
            Die();
        }
    }
    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
