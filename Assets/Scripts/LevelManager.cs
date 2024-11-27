// LevelManager.cs - Singleton that manages level logic
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public event Action OnLevelStart;
    public event Action OnLevelCompleted;
    public event Action OnLevelFailedEvent;

    [SerializeField] private int knivesToCompleteLevel = 5;
    [SerializeField] private float levelTimeLimit = 30f;
    [SerializeField] private int currentLevelIndex = 0;
    public int CurrentLevelIndex {  get { return currentLevelIndex; } }
    [SerializeField] private GameObject levelTarget;
    [SerializeField] private GameObject Player;
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    private LevelData currentLevelData;

    private int successfulHits = 0;
    private float levelStartTime;
    private bool isLevelActive;
    private int isLevelCompleted = 0;
    public int LevelCompleted { get { return isLevelCompleted; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Debug.Log("LevelManager: Instance created.");
        }
        else
        {
            Destroy(gameObject);
            //Debug.LogWarning("LevelManager: Duplicate instance detected and destroyed.");
        }
    }

    private void Start()
    {
        Invoke("StartLevel", 0.01f);
    }


    private void Update()
    {
        if (isLevelActive)
        {
            var timeRemaining = Mathf.Round(Mathf.Max(0, levelTimeLimit - (Time.time - levelStartTime)));
            UIManager.Instance.UpdateTimer(timeRemaining);
            Debug.Log(timeRemaining);

            if (timeRemaining <= 0 && knivesToCompleteLevel != 0)
            {
                TriggerLevelFailed();
            }
        }
    }


    public void StartLevel()
    {
        isLevelActive = true;
        levelStartTime = Time.time;
        Player.GetComponent<PlayerWeaponThrow>().enabled = true;
        levelTarget.GetComponent<TargetRotator>().enabled = true;
        UIManager.Instance.UpdateScore(knivesToCompleteLevel);
        Debug.Log(levelTimeLimit);
        Debug.Log(knivesToCompleteLevel);
        OnLevelStart?.Invoke();
    }

    public void OnKnifeHitTarget()
    {
        knivesToCompleteLevel--;
        UIManager.Instance.UpdateScore(knivesToCompleteLevel);

        if (knivesToCompleteLevel == 0)
        {
            UIManager.Instance.OpenVictoryWindow();
            isLevelCompleted = 1;
            PlayerPrefs.SetInt("isLevelCompleted", isLevelCompleted);
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
        Player.GetComponent<PlayerWeaponThrow>().enabled = false;
        levelTarget.GetComponent<TargetRotator>().enabled = false;
        UIManager.Instance.OpenLoseWindow();
        OnLevelFailedEvent?.Invoke();
    }

    public void RestartLevel()
    {
        Debug.Log("Restarting level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //OnLevelStart?.Invoke();
    }


}
