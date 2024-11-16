// TargetRotator.cs - Advanced Version for Senior-Level Implementation
// This script is designed for flexible configuration of target behavior in the "Twisty Blades" game, allowing individual calibration for each level.

using UnityEngine;

[RequireComponent(typeof(Transform))]
public class TargetRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Base rotation speed in degrees per second.")]
    [SerializeField] private float baseSpeed = 30f;
    public float BaseSpeed { get { return baseSpeed; } set { baseSpeed = value; } }
    [Tooltip("Maximum variance in rotation speed from the base speed.")]
    [SerializeField] private float speedVariance = 20f;
    [Tooltip("Interval in seconds at which the target changes its direction of rotation.")]
    [SerializeField] private float changeDirectionInterval = 5f;
    [Tooltip("Should the direction of rotation change at random intervals?")]
    [SerializeField] private bool randomizeDirection = false;
    [Tooltip("Should the speed change randomly at each interval?")]
    [SerializeField] private bool randomizeSpeed = false;

    private float currentSpeed;
    private float timer;
    private int direction = 1;

    private void Start()
    {
        InitializeRotation();
    }

    private void Update()
    {
        HandleRandomizedRotation(Time.deltaTime);
    }

    // Initialize base rotation speed
    private void InitializeRotation()
    {
        currentSpeed = baseSpeed;
        timer = 0f;
    }

    // Handles random rotation changes
    private void HandleRandomizedRotation(float deltaTime)
    {
        timer += deltaTime;

        if (timer >= changeDirectionInterval)
        {
            ApplyRandomizedChanges();
            timer = 0f;
        }

        RotateTarget(deltaTime);
    }

    // Applies randomized changes to the rotation speed and direction
    private void ApplyRandomizedChanges()
    {
        if (randomizeDirection)
        {
            direction *= -1;
        }

        if (randomizeSpeed)
        {
            currentSpeed = Random.Range(baseSpeed - speedVariance, baseSpeed + speedVariance);
        }
    }

    // Rotates the target based on the current speed and direction
    private void RotateTarget(float deltaTime)
    {
        transform.Rotate(0f, 0f, currentSpeed * direction * deltaTime);
    }
}