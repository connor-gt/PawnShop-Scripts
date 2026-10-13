using TMPro;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the display and visibility of the item information popup.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ItemInfoPopup : MonoBehaviour
{
    [Title("Root Transform")]
    [SerializeField]
    private RectTransform rootTransform;

    [Title("Title")]
    [SerializeField]
    private TextMeshProUGUI titleText;

    [Title("Description")]
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    [Title("Body Elements")]
    [SerializeField]
    private TextMeshProUGUI[] bodyTextElements;

    [Title("Body Elements Parent")]
    [SerializeField]
    private RectTransform bodyElementParent;

    /// <summary>
    /// Gets the maximum number of body text elements that can be displayed by this popup.
    /// </summary>
    public int MaxBodyElements => bodyTextElements.Length;

    /// <summary>
    /// Gets the number of body text elements currently being displayed by this popup.
    /// </summary>
    public int ActiveBodyElementCount { get; private set; } = 0;

    /// <summary>
    /// Gets the vertical offset applied when positioning the popup above an object.
    /// </summary>
    [field: Title("Placement Settings")]
    [field: Tooltip("The vertical distance between the top of the target object and the bottom of the popup.")]
    [field: SerializeField]
    public float HeightOffset { get; private set; } = 0.25f;

    [Title("Fade Settings")]
    [Tooltip("The time it takes for the popup to fade in/out.")]
    [SerializeField]
    [Min(0f)]
    private float fadeTime = 0.1f;

    /// <summary>
    /// Gets whether the popup is currently active and being used.
    /// </summary>
    public bool IsActive { get; private set; }

    private CanvasGroup canvasGroup;
    private Coroutine lerpRoutine = null;

    private UILookAtMainCamera lookAtMainCamera;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        lookAtMainCamera = GetComponent<UILookAtMainCamera>();
        Clear();
    }

    private void OnDisable()
    {
        if (lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
            lerpRoutine = null;
        }

        IsActive = false;
    }

    /// <summary>
    /// Sets the title text displayed by this popup.
    /// </summary>
    /// <param name="title">The title text to display.</param>
    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    /// <summary>
    /// Sets the description text displayed by this popup.
    /// </summary>
    /// <param name="description">The description text to display.</param>
    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }

    /// <summary>
    /// Attempts to add text to the popup's body. If the maximum number of body elements is reached, the text
    /// is not added.
    /// </summary>
    /// <param name="bodyText">The text to add.</param>
    /// <returns>True if the text was added, false if otherwise.</returns>
    public bool TryAddBodyText(string bodyText)
    {
        if (ActiveBodyElementCount >= MaxBodyElements)
        {
            return false;
        }

        bodyTextElements[ActiveBodyElementCount].text = bodyText;
        bodyTextElements[ActiveBodyElementCount].gameObject.SetActive(true);
        ActiveBodyElementCount++;
        return true;
    }

    /// <summary>
    /// Clears all text and disables all body text elements in this popup.
    /// </summary>
    public void Clear()
    {
        SetTitle("");
        SetDescription("");

        for (int i = 0; i < bodyTextElements.Length; i++)
        {
            bodyTextElements[i].gameObject.SetActive(false);
        }

        ActiveBodyElementCount = 0;
    }

    /// <summary>
    /// Displays the popup with the specified content.
    /// </summary>
    /// <param name="content">A container for what information should be displayed in the popup.</param>
    public void Show(ItemInfoContent content)
    {
        SetContent(content);
        Show();
    }

    /// <summary>
    /// Displays the popup using.
    /// </summary>
    public void Show()
    {
        SetVisible(true);
    }

    /// <summary>
    /// Hides the popup.
    /// </summary>
    public void Hide()
    {
        SetVisible(false);
    }

    /// <summary>
    /// Sets whether the popup is visible and fades it to the corresponding visibility state.
    /// </summary>
    /// <param name="isVisible">Whether the popup should be visible.</param>
    private void SetVisible(bool isVisible)
    {
        if (lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
        }

        if (lookAtMainCamera != null)
        {
            lookAtMainCamera.enabled = isVisible;
        }

        IsActive = isVisible;
        float targetAlpha = isVisible ? 1f : 0f;

        lerpRoutine = StartCoroutine(
            LerpUtils.LerpFloat(canvasGroup.alpha, targetAlpha, fadeTime, a => canvasGroup.alpha = a)
        );
    }


    /// <summary>
    /// Set the content of this item information popup.
    /// </summary>
    /// <param name="content">A container for what information should be displayed in the popup.</param>
    public void SetContent(ItemInfoContent content)
    {
        Clear();

        SetTitle(content.Title);
        SetDescription(content.Description);
        
        for (int i = 0; i < content.BodyTextElements.Length; i++)
        {
            if (!TryAddBodyText(content.BodyTextElements[i]))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Adding too many body text elements to {gameObject.name}", this);
#endif
                break;
            }
        }

        // Fixes an issue where the background image isn't correctly resizing when looking at a different
        // object that shows a different amount of body text elements for its popup. The forced refresh makes
        // sure the popup is rebuilt
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(bodyElementParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rootTransform);
    }

    /// <summary>
    /// Move this popup above the specified location.
    /// <para>
    /// The <see cref="HeightOffset"/> is set in the inspector.
    /// </para>
    /// </summary>
    /// <param name="location">The position to place this popup above at.</param>
    public void PlaceAbove(Vector3 location)
    {
        transform.position = location + Vector3.up * HeightOffset;
    }
}
