using UnityEngine;

public class Destory : MonoBehaviour
{
    public int Hp;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            Hp = 0;
        }
    }
}
