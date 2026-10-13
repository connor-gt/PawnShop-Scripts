using System.Collections.Generic;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Controls the informational UI displayed for the player's currently held item.
/// </summary>
public class HeldItemInfoUIController : MonoBehaviour
{
    public static HeldItemInfoUIController Instance { get; private set; }

    [Title("Canvas Group")]
    [SerializeField]
    private CanvasGroup canvasGroup;

    [Title("Title Text")]
    [SerializeField]
    private TextMeshProUGUI titleText;

    [Title("Description Text")]
    [SerializeField]
    private TextMeshProUGUI descriptionText;

    [Title("Body Text Section")]
    [Tooltip("The parent that item information pieces should be instantiated under.")]
    [SerializeField]
    private Transform bodyTextSectionParent;

    [Tooltip("This can either be empty or contain pre-instantiated prefabs.")]
    [SerializeField]
    private List<TextMeshProUGUI> bodyTextList = new();

    [Title("Body Text Prefab")]
    [SerializeField]
    private GameObject bodyTextPrefab;

    [Title("Fade Settings")]
    [Tooltip("The time it takes for the informational display to fade in/out.")]
    [SerializeField]
    [Min(0f)]
    private float fadeTime = 0.075f;

    private Coroutine lerpRoutine = null;

    /// <summary>
    /// Gets whether or not the UI is currently active and displaying information for the player's currently
    /// held item.
    /// </summary>
    public bool IsActive { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Hide();
        }
        else
        {
            Destroy(gameObject);
        }
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
    /// Sets the title text displayed by the informational UI.
    /// </summary>
    /// <param name="title">The title text to display.</param>
    public void SetTitle(string title)
    {
        titleText.text = title;
    }

    /// <summary>
    /// Sets the description text displayed by the informational UI.
    /// </summary>
    /// <param name="description">The description text to display.</param>
    public void SetDescription(string description)
    {
        descriptionText.text = description;
    }

    /// <summary>
    /// Displays the informational UI using the specified content.
    /// </summary>
    /// <param name="content">The content to display.</param>
    public void Show(ItemInfoContent content)
    {
        SetContent(content);
        Show();
    }

    /// <summary>
    /// Displays the informational UI.
    /// </summary>
    public void Show()
    {
        SetVisible(true);
    }

    /// <summary>
    /// Hides the informational UI.
    /// </summary>
    public void Hide()
    {
        SetVisible(false);
    }

    /// <summary>
    /// Sets whether the informational UI is visible and fades it to the desired visibility state.
    /// </summary>
    /// <param name="isVisible">Whether the informational UI should be visible.</param>
    private void SetVisible(bool isVisible)
    {
        if (lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
        }

        IsActive = isVisible;
        float targetAlpha = isVisible ? 1f : 0f;

        lerpRoutine = StartCoroutine(
            LerpUtils.LerpFloat(canvasGroup.alpha, targetAlpha, fadeTime, a => canvasGroup.alpha = a)
        );
    }

    /// <summary>
    /// Sets the content displayed by the informational UI.
    /// </summary>
    /// <param name="content">The content to display.</param>
    public void SetContent(ItemInfoContent content)
    {
        Clear();

        SetTitle(content.Title);
        SetDescription(content.Description);

        // Creates additional body text elements if there are not enough for the input content
        if (bodyTextList.Count < content.BodyTextElements.Length)
        {
            int numToCreate = content.BodyTextElements.Length - bodyTextList.Count;
            for (int i = 0; i < numToCreate; i++)
            {
                TextMeshProUGUI created = Instantiate(bodyTextPrefab, bodyTextSectionParent)
                    .GetComponent<TextMeshProUGUI>();

                bodyTextList.Add(created);
            }
        }

        for (int i = 0; i < content.BodyTextElements.Length; i++)
        {
            bodyTextList[i].text = content.BodyTextElements[i];
        }
    }

    /// <summary>
    /// Clears all content from the informational UI, including its title, description, and body text elements.
    /// </summary>
    public void Clear()
    {
        SetTitle("");
        SetDescription("");

        for (int i = 0; i < bodyTextList.Count; i++)
        {
            bodyTextList[i].text = "";
        }
    }
}
