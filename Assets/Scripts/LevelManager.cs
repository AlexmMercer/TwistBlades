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

    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    private int currentLevelIndex = 0;
    private LevelData currentLevelData;

    private int successfulHits;

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
        //UIManager.Instance.UpdateScore(currentLevelData.knivesRequired);
        Debug.Log("LevelManager: Start method called.");
        Invoke("StartLevel", 0.1f); // ???????? ?? 0.1 ???????
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
        successfulHits++;
        //currentLevelData.knivesRequired--;
        //UIManager.Instance.UpdateScore(currentLevelData.knivesRequired);

        // ?????????, ???????? ?? ???????
        if (successfulHits >= knivesToCompleteLevel)
        {
            Debug.Log("LevelManager: Level completed.");
            OnLevelCompleted?.Invoke();
        }
        else
        {
            // ???? ??????? ??? ?? ????????, ??????? ????? ???
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
        OnLevelFailedEvent?.Invoke();
    }
}
