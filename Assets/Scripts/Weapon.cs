// Weapon.cs - Redesigned script for weapon behavior
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float penetrationDepth;
    [SerializeField] bool isAttackWeapon;

    public void SetPenetrationDepth(float depth)
    {
        penetrationDepth = depth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Target") && isAttackWeapon == true)
        {
            StickToTarget(collision.gameObject);
            LevelManager.Instance.OnKnifeHitTarget();
        }
        else if (collision.gameObject.CompareTag("Weapon"))
        {
            LevelManager.Instance.TriggerLevelFailed();
        }
    }

    private void StickToTarget(GameObject target)
    {
        Debug.Log("Weapon: StickToTarget called.");
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        Vector2 localPosition = transform.localPosition;
        localPosition.y += 1 / penetrationDepth;
        transform.localPosition = localPosition;
        transform.SetParent(target.transform);
    }
}