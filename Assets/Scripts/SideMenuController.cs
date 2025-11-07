using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SideMenuController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform sideMenu;
    public Button toggleButton;

    [Header("Menu Buttons")]
    public Button buttonDetectLocation;
    public Button buttonInputLocation;
    public Button buttonScanLocation;
    public Button buttonHowToUse;
    public Button buttonOptions;

    [Header("Panels")]
    public RectTransform detectPanel;
    public TMP_Dropdown detectDropdown;
    public Button detectNavigateButton;
    public Button detectCloseButton;

    public RectTransform inputPanel;
    public TMP_Dropdown inputStartDropdown;
    public TMP_Dropdown inputEndDropdown;
    public Button inputNavigateButton;
    public Button inputCloseButton;

    public RectTransform scanPanel;
    public Button scanQRButton;
    public TMP_Dropdown scanDropdown;
    public Button scanCloseButton;
    public Button scanNavigateButton;

    public RectTransform howToUsePanel;
    public RectTransform optionsPanel;

    [Header("Settings")]
    public float menuAnimationDuration = 0.3f;
    public float openX = 250f;
    public Vector2 panelOnscreenPosition = Vector2.zero;     // where panels appear
    public Vector2 panelOffscreenPosition = new Vector2(900f, 0f); // offscreen hiding position

    private Vector2 closedPosition;
    private Vector2 openPosition;
    private Coroutine currentAnimation;

    private string selectedDetectLocation = "";
    private string selectedScanLocation = "";

    private readonly List<string> roomList = new List<string>
    {
        "Select a Location",
        "Room 102",
        "Room 118",
        "Room 119A",
        "Room 119B",
        "Room 125",
        "Room 126",
        "Room 127",
        "Room 128",
        "Room 130",
        "Room 131",
        "Room 132",
        "Room 133",
        "Room 136",
        "Restroom 1"
    };

    void Start()
    {
        // Setup side menu slide positions
        float menuWidth = sideMenu.rect.width;
        openPosition = new Vector2(openX, sideMenu.anchoredPosition.y);
        closedPosition = new Vector2(-menuWidth, sideMenu.anchoredPosition.y);
        sideMenu.anchoredPosition = closedPosition;

        toggleButton.onClick.AddListener(ToggleMenu);

        // Hook up left-side menu buttons
        buttonDetectLocation.onClick.AddListener(() => ShowPanel(detectPanel));
        buttonInputLocation.onClick.AddListener(() => ShowPanel(inputPanel));
        buttonScanLocation.onClick.AddListener(() => ShowPanel(scanPanel));
        buttonHowToUse.onClick.AddListener(() => ShowPanel(howToUsePanel));
        buttonOptions.onClick.AddListener(() => ShowPanel(optionsPanel));

        HideAllPanels();

        SetupDetectPanel();
        SetupInputPanel();
        SetupScanPanel();
    }

    // ===== SIDE MENU ANIMATION =====
    public void ToggleMenu()
    {
        float x = sideMenu.anchoredPosition.x;
        float mid = (openPosition.x + closedPosition.x) * 0.5f;
        Vector2 target = (x > mid) ? closedPosition : openPosition;
        if (currentAnimation != null) StopCoroutine(currentAnimation);
        currentAnimation = StartCoroutine(SlideMenu(target));
    }

    private IEnumerator SlideMenu(Vector2 target)
    {
        Vector2 start = sideMenu.anchoredPosition;
        float t = 0f;
        while (t < menuAnimationDuration)
        {
            t += Time.deltaTime;
            sideMenu.anchoredPosition = Vector2.Lerp(start, target, t / menuAnimationDuration);
            yield return null;
        }
        sideMenu.anchoredPosition = target;
    }

    // ===== PANEL DISPLAY LOGIC =====
    private void ShowPanel(RectTransform panel)
    {
        HideAllPanels();
        if (panel == null) return;

        panel.gameObject.SetActive(true);
        panel.anchoredPosition = panelOnscreenPosition;
        Debug.Log($"{panel.name} opened.");
    }

    private void HidePanel(RectTransform panel)
    {
        if (panel == null) return;
        panel.anchoredPosition = panelOffscreenPosition;
        panel.gameObject.SetActive(false);
    }

    private void HideAllPanels()
    {
        HidePanel(detectPanel);
        HidePanel(inputPanel);
        HidePanel(scanPanel);
        HidePanel(howToUsePanel);
        HidePanel(optionsPanel);
    }

    // ===== DETECT PANEL =====
    private void SetupDetectPanel()
    {
        if (!detectDropdown) return;

        detectDropdown.ClearOptions();
        detectDropdown.AddOptions(roomList);
        detectDropdown.onValueChanged.AddListener(OnDetectLocationSelected);

        if (detectNavigateButton)
            detectNavigateButton.onClick.AddListener(() =>
            {
                Debug.Log($"Begin navigation from detected location to {selectedDetectLocation}");
                // TODO: Add BeginNavigation script call
            });

        if (detectCloseButton)
            detectCloseButton.onClick.AddListener(() => HidePanel(detectPanel));
    }

    private void OnDetectLocationSelected(int index)
    {
        selectedDetectLocation = detectDropdown.options[index].text;
        Debug.Log($"Selected destination location: {selectedDetectLocation}");
    }

    // ===== INPUT PANEL =====
    private void SetupInputPanel()
    {
        if (!inputStartDropdown || !inputEndDropdown) return;

        inputStartDropdown.ClearOptions();
        inputEndDropdown.ClearOptions();
        inputStartDropdown.AddOptions(roomList);
        inputEndDropdown.AddOptions(roomList);

        if (inputNavigateButton)
            inputNavigateButton.onClick.AddListener(() =>
            {
                string start = inputStartDropdown.options[inputStartDropdown.value].text;
                string end = inputEndDropdown.options[inputEndDropdown.value].text;
                Debug.Log($"Begin navigation from {start} to {end}");
                // TODO: Add navigation logic
                if (start == "Select a Location" || end == "Select a Location")
                {
                    Debug.LogWarning("Please select valid start and end locations.");
                    return;
                }
                NavigationContext.SetLocations(start, end);

                SceneLoader.Instance.LoadScene("2DMap");
            });

        if (inputCloseButton)
            inputCloseButton.onClick.AddListener(() => HidePanel(inputPanel));
    }

    // ===== SCAN PANEL =====
    private void SetupScanPanel()
    {
        if (scanDropdown)
        {
            scanDropdown.ClearOptions();
            scanDropdown.AddOptions(roomList);
            scanDropdown.onValueChanged.AddListener(OnScanLocationSelected);
        }

        if (scanNavigateButton)
        {
            scanNavigateButton.onClick.AddListener(() =>
            {
                Debug.Log($"Begin navigation to {selectedScanLocation} from scanned location");
                // TODO: Add BeginNavigation script call
            });
        }

        if (scanQRButton)
                scanQRButton.onClick.AddListener(() =>
                {
                    Debug.Log("Running QR scan...");
                    // TODO: Add Scan script call
                });

        if (scanCloseButton)
            scanCloseButton.onClick.AddListener(() => HidePanel(scanPanel));
    }

    private void OnScanLocationSelected(int index)
    {
        selectedScanLocation = scanDropdown.options[index].text;
        Debug.Log($"Selected location: {selectedScanLocation}");
    }
}
