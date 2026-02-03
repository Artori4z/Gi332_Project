using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1f;
    private Rigidbody rb;
    private GameObject[] player;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        // 1. หา Player ทุกตัวที่มี Tag "Player" มาเก็บไว้ใน Array
         player = GameObject.FindGameObjectsWithTag("Player");

        GameObject closestPlayer = null;
        float shortestDistance = Mathf.Infinity; // ตั้งค่าเริ่มต้นให้ไกลเป็นอนันต์
        Vector3 currentPosition = transform.position;

        // 2. วนลูปเช็ก Player ทีละตัวว่าใครใกล้ที่สุด
        foreach (GameObject p in player)
        {
            float distanceToPlayer = Vector3.Distance(p.transform.position, currentPosition);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                closestPlayer = p;
            }
        }

        // 3. ถ้าเจอคนที่ใกล้ที่สุด ก็สั่งให้วิ่งไปหา
        if (closestPlayer != null)
        {
            Vector3 d = closestPlayer.transform.position - currentPosition;
            Vector3 dir = d.normalized;
            rb.AddForce(dir * speed);
        }
    }
}
