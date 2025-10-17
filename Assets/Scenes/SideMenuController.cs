using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SideMenuController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform sideMenu;      // sliding panel (Panel_SideMenu)
    public Button toggleButton;         // hamburger (Button_Hamburger)

    [Header("Menu Buttons")]
    public Button button1_Options;      // "Options" button
    public Button button2;              // "Button 2"
    public Button button3;              // "Button 3"
    public Button button4;              // "Button 4"
    public Button buttonHowToUse;       // ← your "How to Use" button on the side menu

    [Header("Panels")]
    public GameObject optionsPanel;     // "Options" panel
    public GameObject howToUsePanel;    // ← "How to Use" pop-up panel
    public Button howToStartButton;     // ← "Start" button inside that panel
    public Toggle howToDontShowToggle;  // ← optional "Don't show again" toggle

    [Header("Settings")]
    [Tooltip("Slide animation duration in seconds")]
    public float animationDuration = 0.3f;

    [Tooltip("How far the menu slides out when open")]
    public float openX = 250f;

    private Vector2 closedPosition;
    private Vector2 openPosition;
    private Coroutine currentAnimation;
    private const string PREF_HAS_SEEN_HOWTO = "HasSeenHowTo";

    void Start()
    {
        // ✅ setup and sanity check
        if (sideMenu == null || toggleButton == null)
        {
            Debug.LogError("[SideMenuController] Assign sideMenu and toggleButton in the Inspector.");
            enabled = false;
            return;
        }

        float menuWidth = sideMenu.rect.width;

        // menu slide range
        openPosition = new Vector2(openX, sideMenu.anchoredPosition.y);
        closedPosition = new Vector2(-menuWidth, sideMenu.anchoredPosition.y);

        sideMenu.anchoredPosition = closedPosition;
        toggleButton.onClick.AddListener(ToggleMenu);

        // === Buttons ===
        if (button1_Options) button1_Options.onClick.AddListener(OnOptionsClicked);
        if (button2) button2.onClick.AddListener(() => Debug.Log("Button 2 clicked"));
        if (button3) button3.onClick.AddListener(() => Debug.Log("Button 3 clicked"));
        if (button4) button4.onClick.AddListener(() => Debug.Log("Button 4 clicked"));
        if (buttonHowToUse) buttonHowToUse.onClick.AddListener(ShowHowToUsePanel);

        // === Panels start hidden ===
        if (optionsPanel) optionsPanel.SetActive(false);
        if (howToUsePanel) howToUsePanel.SetActive(false);

        // === Hook up Start button in HowTo panel ===
        if (howToStartButton)
            howToStartButton.onClick.AddListener(() =>
            {
                if (howToDontShowToggle && howToDontShowToggle.isOn)
                    PlayerPrefs.SetInt(PREF_HAS_SEEN_HOWTO, 1);

                if (howToUsePanel) howToUsePanel.SetActive(false);
            });

        // === Show How-To on first launch ===
        if (PlayerPrefs.GetInt(PREF_HAS_SEEN_HOWTO, 0) == 0)
            ShowHowToUsePanel();
    }

    /// <summary>
    /// Opens or closes the side menu when the hamburger is tapped.
    /// </summary>
    public void ToggleMenu()
    {
        float currentX = sideMenu.anchoredPosition.x;
        float midpoint = (openPosition.x + closedPosition.x) * 0.5f;
        Vector2 target = (currentX > midpoint) ? closedPosition : openPosition;
        StartSlideTo(target);
    }

    private void StartSlideTo(Vector2 target)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(SlideMenu(target));
    }

    private IEnumerator SlideMenu(Vector2 target)
    {
        Vector2 start = sideMenu.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            sideMenu.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        sideMenu.anchoredPosition = target;
        currentAnimation = null;
    }

    // === OPTIONS BUTTON ===
    private void OnOptionsClicked()
    {
        Debug.Log("Options clicked");
        if (optionsPanel)
            optionsPanel.SetActive(true);
    }

    // === HOW TO USE ===
    private void ShowHowToUsePanel()
    {
        Debug.Log("Showing How-To panel");
        if (howToUsePanel)
            howToUsePanel.SetActive(true);
    }

    // === Reset helper (for testing) ===
    public void ResetHowToFlag()
    {
        PlayerPrefs.DeleteKey(PREF_HAS_SEEN_HOWTO);
    }
}
