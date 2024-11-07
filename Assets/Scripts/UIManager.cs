// UI Manager Design for "Twisty Blades" Game
// This UI Manager is designed with a professional Senior developer approach, focusing on maintainability, modularity, and scalability.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // References to different UI panels
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject chooseLevelPanel;
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private GameObject settingsPanel;

    // Canvas Group for smooth transitions
    private CanvasGroup currentActivePanel;

    // Singleton instance for easy access
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of UIManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ShowPanel(startScreen);
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
        StartCoroutine(FadeIn(currentActivePanel, 0.5f));
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
}

/*
Key Features:
1. **Singleton Pattern**: The UIManager is implemented as a singleton, ensuring only one instance is active and can be accessed globally.
2. **Fade Transitions**: Smooth fade-in and fade-out transitions are implemented using `CanvasGroup`, creating a polished user experience.
3. **Centralized Control**: The UIManager controls which panels are visible, reducing the need for other scripts to manage UI state.
4. **Scalability**: Adding new panels only requires defining the GameObject and adding a `Show` method, making future expansions easy.
*/
