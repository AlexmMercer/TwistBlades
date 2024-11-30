
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // References to different UI panels
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject chooseLevelPanel;
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject VictoryPanel;
    [SerializeField] private GameObject LosePanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Button[] levelButtons;

    // Canvas Group for smooth transitions
    private CanvasGroup currentActivePanel;

    // Singleton instance for easy access
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        try
        {
            ShowPanel(startScreen);
            LoadLevelProgress();
        } catch(Exception e)
        {
            Debug.Log("Aboba");
        }
    }

    // Public method to show the desired panel
    public void ShowPanel(GameObject panelToShow)
    {
        // Hide current panel immediately
        if (currentActivePanel != null && currentActivePanel.gameObject != panelToShow)
        {
            currentActivePanel.alpha = 0;
            currentActivePanel.gameObject.SetActive(false);
        }

        // Set new active panel and show it with a fade-in effect
        CanvasGroup panelCanvasGroup = panelToShow.GetComponent<CanvasGroup>();
        if (panelCanvasGroup == null)
        {
            panelCanvasGroup = panelToShow.AddComponent<CanvasGroup>();
        }

        currentActivePanel = panelCanvasGroup;
        StartCoroutine(FadeIn(currentActivePanel, 0.3f));
    }

    // Method to hide all panels (helper function)
    private void HideAllPanels()
    {
        startScreen.SetActive(false);
        chooseLevelPanel.SetActive(false);
        levelPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    // Fade-in effect for smooth panel transitions
    private IEnumerator FadeIn(CanvasGroup canvasGroup, float duration)
    {
        float time = 0;
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    // Fade-out effect for smooth panel transitions
    private IEnumerator FadeOut(CanvasGroup canvasGroup, float duration)
    {
        float time = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }

    // Specific panel display methods for easier use
    public void ShowStartScreen()
    {
        ShowPanel(startScreen);
    }

    public void ShowChooseLevelPanel()
    {
        ShowPanel(chooseLevelPanel);
    }

    public void ShowLevelPanel()
    {
        ShowPanel(levelPanel);
    }

    public void ShowSettingsPanel()
    {
        ShowPanel(settingsPanel);
    }

    // Method to update score text
    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
            //Debug.Log("UIManager: Score updated to " + score);
        }
    }

    public void UpdateTimer(float timeLeft)
    {
        if (timerText != null)
        {
            timerText.text = timeLeft.ToString();
            //Debug.Log("UIManager: Timer updated to " + timeLeft);
        }
    }

    public void OpenVictoryWindow()
    {
        ShowPanel(VictoryPanel);
    }

    public void OpenLoseWindow()
    {
        ShowPanel(LosePanel);
    }

    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene($"Level_{levelNumber}");
    }

    public void GoHome()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void LoadLevelProgress()
    {
        int progress = PlayerPrefs.GetInt("LevelProgress", 0);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i <= progress)
            {
                levelButtons[i].interactable = true;
            }
            else
            {
                levelButtons[i].interactable = false;
            }
        }
    }


}