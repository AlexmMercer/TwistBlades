// Weapon.cs - Redesigned script for weapon behavior
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float penetrationDepth;

    public void SetPenetrationDepth(float depth)
    {
        penetrationDepth = depth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            StickToTarget(collision.gameObject);
        }
    }

    private void StickToTarget(GameObject target)
    {
        transform.SetParent(target.transform);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        // Adjust position for penetration depth
        Vector2 localPosition = transform.localPosition;
        //localPosition.z = -penetrationDepth;
        transform.localPosition = localPosition;
    }
}
