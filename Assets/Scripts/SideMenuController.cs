using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

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
    public Button detectLocationButton;
    public Text locationOutputText;

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
    public TMP_Text warnScanText;

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

    private List<string> roomList;
    void Start()
    {

        roomList = new List<string>{
            "Hallway Node 1",
            "Room 132",
            "Room 133",
            "Room 136",
            "Bathroom",
            "Room 124"
        };
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
                var locationDictionary = NavGraph.Instance.nameToNode;
                NavigationContext.EndLocation = locationDictionary[detectDropdown.options[detectDropdown.value].text];
                HideAllPanels();
            });

        if (detectCloseButton)
            detectCloseButton.onClick.AddListener(() => HidePanel(detectPanel));

        if (detectLocationButton)
        {
            detectLocationButton.onClick.AddListener(() =>
            {
                Debug.Log("Detecting current location...");

                try
                {

                    var sorted = BeaconManager.Instance.currentBeacons
                        .OrderBy(b => ProximityRank(b.proximity)).ThenByDescending(b => b.rssi).ToList();

                    var closestBeacon = sorted.FirstOrDefault();
                    var locationDictionary = NavGraph.Instance.locationMap;
                    if (closestBeacon.minor != 0)
                    {
                        string locationName = locationDictionary[closestBeacon.minor].shortname;
                        Debug.Log($"Closest Beacon: {locationName}");
                        NavigationContext.StartLocation = locationDictionary[closestBeacon.minor];
                        if (locationOutputText != null)
                            locationOutputText.text = $"Detected: {locationName}";
                    }
                    else
                    {
                        Debug.Log($"NO BEACONS NEAR");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Exception: " + e);
                }



            });
        }
    }

    private int ProximityRank(string proximity)
    {
        // Lower rank = closer
        return proximity switch
        {
            "immediate" => 0,
            "near" => 1,
            "far" => 2,
            _ => 3
        };
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
                NavigationContext.StartLocation = NavGraph.Instance.nameToNode[start];
                NavigationContext.EndLocation = NavGraph.Instance.nameToNode[end];
                Debug.Log($"{NavigationContext.EndLocation.shortname}");
                Debug.Log($"{NavigationContext.StartLocation.shortname}");
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
                Dictionary<string,Node> nameToNode = GenerateDictionary();
                Debug.Log($"Begin navigation to {selectedScanLocation} from {NavigationContext.StartLocation.shortname}");
                // TODO: Add BeginNavigation script call
                if (selectedScanLocation == "Select a Location" || NavigationContext.EndLocation != null)
                {
                    Debug.LogWarning("Please select a valid destination location and ensure start location is set.");
                    return;
                }
            });
        }

        if (scanQRButton)
        {
            scanQRButton.onClick.AddListener(() =>
                {
                    if (NavigationContext.EndLocation != null)
                    {
                        Debug.Log("Running QR scan...");
                        // TODO: Add Scan script call
                        SceneLoader.Instance.LoadScene("QRScene");
                    }
                    else
                    {
                        warnScanText.text = "Please Select END Location Before Proceeding";
                        Debug.Log("TEST");
                    }

                });
        }

        if (scanCloseButton)
            scanCloseButton.onClick.AddListener(() => HidePanel(scanPanel));
    }

    private void OnScanLocationSelected(int index)
    {
        selectedScanLocation = scanDropdown.options[index].text;
        NavigationContext.StartLocation = NavGraph.Instance.nameToNode[selectedScanLocation];
        Debug.Log($"Selected location: {selectedScanLocation}");
    }

    private static List<String> GenerateMenuOptions()
    {
        List<String> menu = new List<String>();

        foreach (var node in NavGraph.Instance.nodes)
        {
            if (node.isLocation)
            {
                menu.Add(node.shortname);
            }
        }

        return menu;
    }

    private static Dictionary<string, Node> GenerateDictionary()
    {
        Dictionary<string, Node> shortToNode = new Dictionary<string, Node>();
        foreach (var node in NavGraph.Instance.nodes)
        {
            if (node.isLocation)
            {
                shortToNode.Add(node.shortname, node);
            }
        }
        return shortToNode;
    }
}
