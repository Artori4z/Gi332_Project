using UnityEngine;

public class Entity : MonoBehaviour
{
    protected int Hp;
    protected float Speed;
    protected int Def;
    protected int AtkPower;
    protected Rigidbody rb;
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

        if (targetRb != null)
        {
            Vector3 pushDir = target.transform.position - transform.position;
            pushDir = new Vector3(pushDir.x, 0, pushDir.z).normalized;
            targetRb.AddForce(pushDir * AtkPower, ForceMode.Impulse);
        }
    }
    protected virtual void Move() { }
    protected virtual void Die() { }
}
