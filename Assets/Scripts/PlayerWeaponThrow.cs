// PlayerWeaponThrow.cs - Handles throwing of the weapon by the player
// This script is responsible for managing the weapon throw mechanics, including depth based on press duration.

using UnityEngine;

public class PlayerWeaponThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    [Tooltip("Minimum depth of weapon when thrown.")]
    [SerializeField] private float minDepth = 0.1f;
    [Tooltip("Maximum depth of weapon when thrown.")]
    [SerializeField] private float maxDepth = 0.5f;
    [Tooltip("Maximum hold time for full throw strength.")]
    [SerializeField] private float maxHoldTime = 2f;
    [Tooltip("Prefab of the weapon to be thrown.")]
    [SerializeField] private GameObject weaponPrefab;
    [Tooltip("Cooldown time between throws.")]
    [SerializeField] private float throwCooldown = 1f;
    [Tooltip("Time interval for spawning a new weapon.")]
    [SerializeField] private float newWeaponInterval = 3f;

    [Header("Throw Animation Settings")]
    [Tooltip("Maximum offset distance for preparing throw.")]
    [SerializeField] private float maxPrepareOffsetY = -2f;
    [Tooltip("Speed at which the weapon moves during preparation.")]
    [SerializeField] private float prepareSpeed = 5f;
    [Tooltip("Speed at which the weapon is thrown.")]
    [SerializeField] private float throwSpeed = 10f;

    private float holdStartTime;
    private bool isHolding = false;
    private GameObject currentWeapon;
    private bool isThrowing = false;
    private float lastThrowTime = -Mathf.Infinity;
    private float lastWeaponSpawnTime = -Mathf.Infinity;

    private void Start()
    {
        // Instantiate the first weapon at the start
        SpawnNewWeapon();
    }

    private void Update()
    {
        // Detect when player starts holding (mouse or touch)
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) && Time.time >= lastThrowTime + throwCooldown)
        {
            StartHolding();
        }

        // Detect when player releases (mouse or touch)
        if ((Input.GetMouseButtonUp(0) && isHolding) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && isHolding))
        {
            ThrowWeapon();
        }

        // Animate weapon during holding for throw preparation
        if (isHolding && currentWeapon != null)
        {
            AnimatePrepareThrow();
        }

        // Animate the weapon flying towards the target
        if (isThrowing && currentWeapon != null)
        {
            AnimateThrow();
        }

        // Spawn a new weapon at regular intervals if no weapon is available
        if (currentWeapon == null && Time.time >= lastWeaponSpawnTime + newWeaponInterval)
        {
            SpawnNewWeapon();
        }
    }

    // Start holding to determine throw strength
    private void StartHolding()
    {
        isHolding = true;
        holdStartTime = Time.time;
    }

    // Animate the weapon during preparation for throw
    private void AnimatePrepareThrow()
    {
        // Calculate the offset based on hold duration
        float holdDuration = Mathf.Clamp(Time.time - holdStartTime, 0, maxHoldTime);
        float currentOffsetY = Mathf.Lerp(0, maxPrepareOffsetY, holdDuration / maxHoldTime);

        // Smoothly move the weapon downward to simulate tension
        Vector3 targetPosition = transform.position + new Vector3(0, currentOffsetY, 0);
        currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, targetPosition, Time.deltaTime * prepareSpeed);
    }

    // Throw the weapon based on hold duration
    private void ThrowWeapon()
    {
        isHolding = false;
        isThrowing = true;
        lastThrowTime = Time.time;

        if (currentWeapon == null) return;

        float holdDuration = Mathf.Clamp(Time.time - holdStartTime, 0, maxHoldTime);
        float depth = Mathf.Lerp(minDepth, maxDepth, holdDuration / maxHoldTime);

        // Reset position to simulate release
        currentWeapon.transform.position = transform.position;
        currentWeapon.GetComponent<Rigidbody2D>().isKinematic = false;
        currentWeapon.GetComponent<Rigidbody2D>().velocity = Vector2.up * throwSpeed;
        currentWeapon.GetComponent<Rigidbody2D>().gravityScale = 0; // Disable gravity for more accurate movement

        // Store depth for later use
        currentWeapon.GetComponent<Weapon>().penetrationDepth = depth;

        // Set currentWeapon to null after throwing, to allow a new one to be spawned
        currentWeapon = null;
    }

    // Animate the weapon flying towards the target
    private void AnimateThrow()
    {
        // Check for collision with the target's Circle Collider
        Collider2D targetCollider = GameObject.FindGameObjectWithTag("Target").GetComponent<Collider2D>();
        if (targetCollider != null && currentWeapon != null && targetCollider.OverlapPoint(currentWeapon.transform.position))
        {
            StickWeaponToTarget(currentWeapon);
            isThrowing = false;
        }
    }

    // Instantiate the weapon to be thrown
    private GameObject InstantiateWeapon()
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("Weapon prefab is not assigned in the inspector.");
            return null;
        }
        return Instantiate(weaponPrefab, transform.position, Quaternion.identity);
    }

    // Spawn a new weapon
    private void SpawnNewWeapon()
    {
        currentWeapon = InstantiateWeapon();
        lastWeaponSpawnTime = Time.time;
        currentWeapon.GetComponent<Rigidbody2D>().isKinematic = true;
    }

    // Stick the weapon to the target
    private void StickWeaponToTarget(GameObject weapon)
    {
        if (weapon == null) return;

        GameObject target = GameObject.FindGameObjectWithTag("Target");
        if (target == null)
        {
            Debug.LogError("Target with tag 'Target' not found.");
            return;
        }

        float depth = weapon.GetComponent<Weapon>().penetrationDepth;

        weapon.transform.position = target.transform.position + (Vector3.forward * depth);
        weapon.transform.SetParent(target.transform);
        Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }
    }
}