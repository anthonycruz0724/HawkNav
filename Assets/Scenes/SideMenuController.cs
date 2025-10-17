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

    [Header("Option Panel")]
    public GameObject optionsPanel;     // panel that appears when clicking "Options"

    [Header("Settings")]
    [Tooltip("Slide animation duration in seconds")]
    public float animationDuration = 0.3f;

    [Tooltip("How far the menu slides out when open")]
    public float openX = 250f;

    private Vector2 closedPosition;
    private Vector2 openPosition;
    private Coroutine currentAnimation;

    void Start()
    {
        if (sideMenu == null || toggleButton == null)
        {
            Debug.LogError("[SideMenuController] Assign sideMenu and toggleButton in the Inspector.");
            enabled = false;
            return;
        }

        float menuWidth = sideMenu.rect.width;

        // Define open/closed positions
        openPosition = new Vector2(openX, sideMenu.anchoredPosition.y);
        closedPosition = new Vector2(-menuWidth, sideMenu.anchoredPosition.y);

        // Start hidden
        sideMenu.anchoredPosition = closedPosition;

        // Hamburger toggles open/close
        toggleButton.onClick.AddListener(ToggleMenu);

        // Hook buttons
        if (button1_Options != null) button1_Options.onClick.AddListener(OnOptionsClicked);
        if (button2 != null) button2.onClick.AddListener(() => Debug.Log("Button 2 clicked"));
        if (button3 != null) button3.onClick.AddListener(() => Debug.Log("Button 3 clicked"));
        if (button4 != null) button4.onClick.AddListener(() => Debug.Log("Button 4 clicked"));

        // Start with options panel hidden
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
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
        if (optionsPanel != null)
            optionsPanel.SetActive(true); // show panel

        // Do NOT close the menu — it stays open
    }

    // === Called by a Back button on the options panel ===
    public void HideOptionsPanel()
    {
        if (optionsPanel != null)
            optionsPanel.SetActive(false);
    }
}
