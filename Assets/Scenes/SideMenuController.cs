using UnityEngine;
using UnityEngine.UI;

public class SideMenuController : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform sideMenu;  // your menu panel
    public Button toggleButton;     // hamburger button

    [Header("Settings")]
    public float animationDuration = 0.3f;

    private Vector2 closedPosition;
    private Vector2 openPosition;
    private bool isOpen = false;          // track state
    private Coroutine currentAnimation;

    void Start()
    {
        float menuWidth = sideMenu.rect.width;

        // Positions
        openPosition = new Vector2(250, sideMenu.anchoredPosition.y);
        closedPosition = new Vector2(-menuWidth, sideMenu.anchoredPosition.y);

        // Start closed
        sideMenu.anchoredPosition = closedPosition;

        // Hook button
        toggleButton.onClick.AddListener(ToggleMenu);
    }

    public void ToggleMenu()
    {
        // Stop any running animation
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        // Decide target based on state
        Vector2 target = isOpen ? closedPosition : openPosition;

        // Start sliding
        currentAnimation = StartCoroutine(SlideMenu(target));

        // Toggle the state for next click
        isOpen = !isOpen;
    }

    private System.Collections.IEnumerator SlideMenu(Vector2 target)
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
}
