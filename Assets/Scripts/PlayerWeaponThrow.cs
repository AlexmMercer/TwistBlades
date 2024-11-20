// PlayerWeaponThrow.cs - Handles the throwing mechanics for the player
// This script is responsible for managing the knife throw mechanics, including depth based on press duration.

using UnityEngine;
using System;

public class PlayerWeaponThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private float maxHoldTime = 2f;
    [SerializeField] private float throwCooldown = 1f;
    [SerializeField] private float throwSpeed = 20f;
    [SerializeField] private float minDepth = 0.1f;
    [SerializeField] private float maxDepth = 0.5f;

    private GameObject currentWeapon;
    private float holdStartTime;
    private bool isHolding;
    private bool isThrowing;
    private float lastThrowTime;

    private LevelManager levelManager;

    private void Start()
    {
        levelManager = LevelManager.Instance;
        if (levelManager != null)
        {
            levelManager.OnLevelStart += SpawnNewWeapon;
        }
    }



    private void Update()
    {
        //Debug.Log("PlayerWeaponThrow: Update method called.");
        HandleInput();
        if (isHolding) AnimatePullback();
    }

    private void HandleInput()
    {
        //Debug.Log("PlayerWeaponThrow: HandleInput method called.");
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            //Debug.Log("PlayerWeaponThrow: Start holding detected.");
            StartHolding();
        }
        else if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
        {
            //Debug.Log("PlayerWeaponThrow: Release detected.");
            if (isHolding) ThrowWeapon();
        }
    }

    private void StartHolding()
    {
        //Debug.Log("PlayerWeaponThrow: StartHolding method called.");
        if (Time.time < lastThrowTime + throwCooldown)
        {
            //Debug.Log("PlayerWeaponThrow: Throw on cooldown.");
            return;
        }
        isHolding = true;
        holdStartTime = Time.time;
    }

    private void AnimatePullback()
    {
        //Debug.Log("PlayerWeaponThrow: AnimatePullback method called.");
        float holdDuration = Mathf.Clamp(Time.time - holdStartTime, 0, maxHoldTime);
        float pullbackDistance = Mathf.Lerp(0, -40f, holdDuration / maxHoldTime);
        if (currentWeapon != null)
        {
            currentWeapon.transform.position = Vector2.Lerp(currentWeapon.transform.position, (Vector2)transform.position + new Vector2(0, pullbackDistance), Time.deltaTime * 50f);
            //Debug.Log("PlayerWeaponThrow: Pullback animation applied.");
        }
    }

    private void ThrowWeapon()
    {
        //Debug.Log("PlayerWeaponThrow: ThrowWeapon method called.");
        isHolding = false;
        isThrowing = false;
        lastThrowTime = Time.time;

        if (currentWeapon == null)
        {
            //Debug.LogWarning("PlayerWeaponThrow: No weapon to throw.");
            return;
        }

        float holdDuration = Mathf.Clamp(Time.time - holdStartTime, 0, maxHoldTime);
        float penetrationDepth = Mathf.Lerp(minDepth, maxDepth, holdDuration / maxHoldTime) * 2.1f;

        Rigidbody2D rb = currentWeapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector2.up * throwSpeed;
            //Debug.Log("PlayerWeaponThrow: Weapon thrown with velocity " + rb.velocity);
        }

        currentWeapon.GetComponent<Weapon>().SetPenetrationDepth(penetrationDepth);
        currentWeapon = null;
    }

    public void SpawnNewWeapon()
    {
        //Debug.Log("PlayerWeaponThrow: SpawnNewWeapon method called.");

        if (weaponPrefab == null)
        {
            //Debug.LogError("PlayerWeaponThrow: weaponPrefab is not assigned.");
            return;
        }

        currentWeapon = Instantiate(weaponPrefab, transform.position, Quaternion.identity);
        //Debug.Log("PlayerWeaponThrow: Weapon instantiated.");

        isThrowing = false;
        Rigidbody2D rb = currentWeapon.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            //Debug.Log("PlayerWeaponThrow: New weapon set to kinematic.");
        }
    }
}