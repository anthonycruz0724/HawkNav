using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SideMenuController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform sideMenu;
    public Button toggleButton;

    [Header("Menu Buttons")]
    public Button buttonSearch;       // "New Search"
    public Button buttonHowToUse;     // "How To Use"

    [Header("Panels")]
    public GameObject howToUsePanel;
    public Button howToStartButton;
    public Toggle howToDontShowToggle;

    public GameObject searchPanel;
    public TMP_Dropdown searchDropdown;
    public Button searchBackButton;

    public GameObject messagePanel;
    public Button messageOkButton;

    [Header("Settings")]
    public float animationDuration = 0.3f;
    public float openX = 250f;

    private Vector2 closedPosition;
    private Vector2 openPosition;
    private Coroutine currentAnimation;
    private const string PREF_HAS_SEEN_HOWTO = "HasSeenHowTo";

    void Start()
    {
        float menuWidth = sideMenu.rect.width;
        openPosition = new Vector2(openX, sideMenu.anchoredPosition.y);
        closedPosition = new Vector2(-menuWidth, sideMenu.anchoredPosition.y);
        sideMenu.anchoredPosition = closedPosition;

        toggleButton.onClick.AddListener(ToggleMenu);
        if (buttonSearch) buttonSearch.onClick.AddListener(OpenSearchPanel);
        if (buttonHowToUse) buttonHowToUse.onClick.AddListener(ShowHowToUsePanel);

        if (searchPanel) searchPanel.SetActive(false);
        if (messagePanel) messagePanel.SetActive(false);

        if (howToStartButton)
            howToStartButton.onClick.AddListener(() =>
            {
                if (howToDontShowToggle && howToDontShowToggle.isOn)
                    PlayerPrefs.SetInt(PREF_HAS_SEEN_HOWTO, 1);
                if (howToUsePanel) howToUsePanel.SetActive(false);
            });

        if (searchDropdown)
        {
            searchDropdown.ClearOptions();
            searchDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Select a Location",
                "Lab 127"
            });
            searchDropdown.onValueChanged.AddListener(OnSearchDropdownChanged);
        }

        if (searchBackButton)
            searchBackButton.onClick.AddListener(() => searchPanel.SetActive(false));

        if (messageOkButton)
            messageOkButton.onClick.AddListener(() => messagePanel.SetActive(false));

        // show How-To at startup unless skipped
        if (PlayerPrefs.GetInt(PREF_HAS_SEEN_HOWTO, 0) == 0)
            ShowHowToUsePanel();
        else
            howToUsePanel.SetActive(false);
    }

    // === Slide Menu ===
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

    // === Panels ===
    private void ShowHowToUsePanel() => howToUsePanel?.SetActive(true);
    private void OpenSearchPanel() => searchPanel?.SetActive(true);

    private void OnSearchDropdownChanged(int index)
    {
        if (index == 1 && messagePanel != null)
            messagePanel.SetActive(true);
    }
}
