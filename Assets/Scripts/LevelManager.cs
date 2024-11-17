// LevelManager.cs - Singleton that manages level logic
using UnityEngine;
using System;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public event Action OnLevelStart;
    public event Action OnLevelCompleted;
    public event Action OnLevelFailedEvent;

    [SerializeField] private int knivesToCompleteLevel = 5;
    [SerializeField] private float levelTimeLimit = 30f;
    [SerializeField] private GameObject levelTarget;
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    private int currentLevelIndex = 0;
    private LevelData currentLevelData;

    private int successfulHits;
    private float levelStartTime;
    private bool isLevelActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("LevelManager: Instance created.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("LevelManager: Duplicate instance detected and destroyed.");
        }
    }

    private void Start()
    {
        //currentLevelData = levels[currentLevelIndex];
        UIManager.Instance.UpdateScore(knivesToCompleteLevel);
        Debug.Log("LevelManager: Start method called.");
        Invoke("StartLevel", 0.05f);
    }


    private void Update()
    {
        //if (isLevelActive)
        //{
            float timeRemaining = Mathf.Round(Mathf.Max(0, levelTimeLimit - (Time.time - levelStartTime)));
            UIManager.Instance.UpdateTimer(timeRemaining);

            if (timeRemaining <= 0 && knivesToCompleteLevel != 0)
            {
                Debug.Log("LevelManager: Time limit reached. Level failed.");
                TriggerLevelFailed();
            }
        //}
    }


    public void StartLevel()
    {
        Debug.Log("LevelManager: StartLevel called.");
        successfulHits = 0;
        OnLevelStart?.Invoke();
        Debug.Log("LevelManager: OnLevelStart event invoked.");
    }

    public void OnKnifeHitTarget()
    {
        Debug.Log("LevelManager: OnKnifeHitTarget called. Successful hits: " + successfulHits);
        knivesToCompleteLevel--;
        UIManager.Instance.UpdateScore(knivesToCompleteLevel);

        if (knivesToCompleteLevel == 0)
        {
            Debug.Log("LevelManager: Level completed.");
            UIManager.Instance.OpenVictoryWindow();
            OnLevelCompleted?.Invoke();
        }
        else
        {
            PlayerWeaponThrow playerWeaponThrow = FindObjectOfType<PlayerWeaponThrow>();
            if (playerWeaponThrow != null)
            {
                playerWeaponThrow.SpawnNewWeapon();
            }
        }
    }


    public void TriggerLevelFailed()
    {
        Debug.Log("LevelManager: OnLevelFailed called.");
        UIManager.Instance.OpenLoseWindow();
        levelTarget.GetComponent<TargetRotator>().enabled = false;
        OnLevelFailedEvent?.Invoke();
    }
}
