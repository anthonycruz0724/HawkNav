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
    public Button buttonSearch;       // "New Search"
    public Button buttonHowToUse;     // "How To Use"
    public Button buttonQR;           // "New QR" (to open scanner)

    [Header("Panels")]
    public GameObject howToUsePanel;
    public Button howToStartButton;
    public Toggle howToDontShowToggle;

    public GameObject searchPanel;
    public TMP_Dropdown searchDropdown;
    public Button searchBackButton;

    public GameObject messagePanel;
    public Button messageOkButton;

    public GameObject qrScannerPanel;   // QR scanner page
    public Button fakeScanButton;       // simulate scan button

    [Header("Settings")]
    public float animationDuration = 0.3f;
    public float openX = 250f;

    private Vector2 closedPosition;
    private Vector2 openPosition;
    private Coroutine currentAnimation;
    private const string PREF_HAS_SEEN_HOWTO = "HasSeenHowTo";

    void Start()
    {
        // --- Setup side menu positions ---
        float menuWidth = sideMenu.rect.width;
        openPosition = new Vector2(openX, sideMenu.anchoredPosition.y);
        closedPosition = new Vector2(-menuWidth, sideMenu.anchoredPosition.y);
        sideMenu.anchoredPosition = closedPosition;

        // --- Hook main buttons ---
        toggleButton.onClick.AddListener(ToggleMenu);
        if (buttonSearch) buttonSearch.onClick.AddListener(OpenSearchPanel);
        if (buttonHowToUse) buttonHowToUse.onClick.AddListener(ShowHowToUsePanel);
        if (buttonQR) buttonQR.onClick.AddListener(OpenQRScanner);

        // --- Start with panels hidden ---
        if (searchPanel) searchPanel.SetActive(false);
        if (messagePanel) messagePanel.SetActive(false);
        if (qrScannerPanel) qrScannerPanel.SetActive(false);

        // --- Setup "How To Use" panel ---
        if (howToStartButton)
        {
            howToStartButton.onClick.AddListener(() =>
            {
                if (howToDontShowToggle && howToDontShowToggle.isOn)
                    PlayerPrefs.SetInt(PREF_HAS_SEEN_HOWTO, 1);

                if (howToUsePanel) howToUsePanel.SetActive(false);
            });
        }

        // --- Setup dropdown list ---
        if (searchDropdown)
        {
            searchDropdown.ClearOptions();

            // Add your room list here 👇
            searchDropdown.AddOptions(new List<string>
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
            });

            searchDropdown.onValueChanged.AddListener(OnSearchDropdownChanged);
        }

        // --- Setup back button ---
        if (searchBackButton)
            searchBackButton.onClick.AddListener(() => searchPanel.SetActive(false));

        // --- Setup message OK button ---
        if (messageOkButton)
            messageOkButton.onClick.AddListener(() => messagePanel.SetActive(false));

        // --- Setup QR Scanner panel ---
        if (fakeScanButton)
            fakeScanButton.onClick.AddListener(SimulateQRScan);

        // --- Show How-To panel on first run ---
        if (PlayerPrefs.GetInt(PREF_HAS_SEEN_HOWTO, 0) == 0)
            ShowHowToUsePanel();
        else
            howToUsePanel.SetActive(false);
    }

    // ====== SLIDE MENU ======
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
        while (t < animationDuration)
        {
            t += Time.deltaTime;
            sideMenu.anchoredPosition = Vector2.Lerp(start, target, t / animationDuration);
            yield return null;
        }
        sideMenu.anchoredPosition = target;
    }

    // ====== PANELS ======
    private void ShowHowToUsePanel()
    {
        if (howToUsePanel)
        {
            howToUsePanel.SetActive(true);
            Debug.Log("How-To panel shown.");
        }
    }

    private void OpenSearchPanel()
    {
        if (searchPanel)
        {
            searchPanel.SetActive(true);
            Debug.Log("Search panel opened.");
        }
    }

    // ====== DROPDOWN LOGIC ======
    private void OnSearchDropdownChanged(int index)
    {
        if (index == 0) return; // ignore "Select a Location"

        string selectedRoom = searchDropdown.options[index].text;
        Debug.Log($"Selected {selectedRoom} – showing WIP message");

        if (messagePanel != null)
        {
            // Optional: dynamically change popup text
            TextMeshProUGUI msgText = messagePanel.GetComponentInChildren<TextMeshProUGUI>();
            if (msgText != null)
                msgText.text = $"This is a WIP tester for {selectedRoom}.";

            messagePanel.SetActive(true);
        }
    }

    // ====== QR SCANNER ======
    private void OpenQRScanner()
    {
        if (qrScannerPanel)
        {
            qrScannerPanel.SetActive(true);
            Debug.Log("QR Scanner page opened.");
        }
    }

    private void SimulateQRScan()
    {
        Debug.Log("Simulated QR scan!");
        if (qrScannerPanel)
            qrScannerPanel.SetActive(false);

        // After "scanning", go directly to search page
        if (searchPanel)
            searchPanel.SetActive(true);
    }
}
