// PlayerWeaponThrow.cs - Handles the throwing mechanics for the player
// This script is responsible for managing the knife throw mechanics, including depth based on press duration.

using UnityEngine;

public class PlayerWeaponThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private float maxHoldTime = 2f;
    [SerializeField] private float throwCooldown = 1f;
    [SerializeField] private float newWeaponInterval = 1f;
    [SerializeField] private float throwSpeed = 20f;
    [SerializeField] private float minDepth = 0.1f;
    [SerializeField] private float maxDepth = 0.5f;

    private GameObject currentWeapon;
    private float holdStartTime;
    private bool isHolding;
    private bool isThrowing;
    private float lastThrowTime;

    private void Start()
    {
        SpawnNewWeapon();
    }

    private void Update()
    {
        HandleInput();
        if (isHolding) AnimatePullback();
        if (currentWeapon == null && Time.time >= lastThrowTime + newWeaponInterval) SpawnNewWeapon();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            StartHolding();
        }
        else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            if (isHolding) ThrowWeapon();
        }
    }

    private void StartHolding()
    {
        if (Time.time < lastThrowTime + throwCooldown) return;
        isHolding = true;
        holdStartTime = Time.time;
    }

    private void AnimatePullback()
    {
        float holdDuration = Mathf.Clamp(Time.time - holdStartTime, 0, maxHoldTime);
        float pullbackDistance = Mathf.Lerp(0, -40f, holdDuration / maxHoldTime);
        if (currentWeapon != null)
        {
            currentWeapon.transform.position = Vector2.Lerp(currentWeapon.transform.position, (Vector2)transform.position + new Vector2(0, pullbackDistance), Time.deltaTime * 50f);
        }
    }

    private void ThrowWeapon()
    {
        isHolding = false;
        isThrowing = false;
        lastThrowTime = Time.time;

        if (currentWeapon == null) return;

        float holdDuration = Mathf.Clamp(Time.time - holdStartTime, 0, maxHoldTime);
        float penetrationDepth = Mathf.Lerp(minDepth, maxDepth, holdDuration / maxHoldTime) * 5f;

        Rigidbody2D rb = currentWeapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.up * throwSpeed;
        }

        currentWeapon.GetComponent<Weapon>().SetPenetrationDepth(penetrationDepth);
        currentWeapon = null;
    }

    private void SpawnNewWeapon()
    {
        currentWeapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        isThrowing = false;
        Rigidbody2D rb = currentWeapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
}