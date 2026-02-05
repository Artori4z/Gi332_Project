using UnityEngine;

public class Jumper : Player
{
    private float jumpForce = 10f;
    private bool isGrounded;
    protected override void Start()
    {
        Hp = 100;
        Speed = 10f;
        Def = 1;
        AtkPower = 10;
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

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
