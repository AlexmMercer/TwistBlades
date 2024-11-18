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
    [SerializeField] private GameObject levelTarget;
    [SerializeField] private GameObject Player;
    [SerializeField] private List<LevelData> levels = new List<LevelData>();
    private int currentLevelIndex = 0;
    private LevelData currentLevelData;

    private int successfulHits = 0;
    private float levelStartTime;
    private bool isLevelActive;
    //private float timeRemaining;

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
                //Debug.Log("LevelManager: Time limit reached. Level failed.");
                TriggerLevelFailed();
            }
        }
    }


    public void StartLevel()
    {
        //Debug.Log("LevelManager: StartLevel called.");
        //successfulHits = 0;
        isLevelActive = true;
        levelStartTime = Time.time;

        Player.GetComponent<PlayerWeaponThrow>().enabled = true;
        levelTarget.GetComponent<TargetRotator>().enabled = true;
        Debug.Log(levelTimeLimit);
        Debug.Log(knivesToCompleteLevel);
        OnLevelStart?.Invoke();
        //Debug.Log("LevelManager: OnLevelStart event invoked.");
    }

    public void OnKnifeHitTarget()
    {
        knivesToCompleteLevel--;
        UIManager.Instance.UpdateScore(knivesToCompleteLevel);

        if (knivesToCompleteLevel == 0)
        {
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
