// Weapon.cs - Redesigned script for weapon behavior
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private float penetrationDepth;
    [SerializeField] bool isAttackWeapon;

    public void SetPenetrationDepth(float depth)
    {
        //Debug.Log("Weapon: SetPenetrationDepth called with depth " + depth);
        penetrationDepth = depth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Weapon: OnCollisionEnter2D called.");
        if (collision.gameObject.CompareTag("Target") && isAttackWeapon == true)
        {
            //Debug.Log("Weapon: Collision with target detected.");
            StickToTarget(collision.gameObject);
            LevelManager.Instance.OnKnifeHitTarget();
        }
        else if (collision.gameObject.CompareTag("Weapon"))
        {
           // Debug.Log("Weapon: Collision with another weapon detected. Level failed.");
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
            //Debug.Log("Weapon: Rigidbody set to kinematic and velocity set to zero.");
        }

        // Adjust position for penetration depth
        Vector2 localPosition = transform.localPosition;
        localPosition.y += 1 / penetrationDepth;
        transform.localPosition = localPosition;
        transform.SetParent(target.transform);
        //Debug.Log("Weapon: Weapon stuck to target with adjusted position.");
    }
}