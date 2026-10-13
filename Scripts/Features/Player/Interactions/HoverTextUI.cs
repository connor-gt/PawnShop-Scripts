using TMPro;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HoverTextUI : MonoBehaviour
{
    [field: Title("References")]
    [field: SerializeField]
    public TextMeshProUGUI TextMesh { get; private set; }
    private float textDefaultAlpha;

    [field: SerializeField]
    public Image BackgroundImage { get; private set; }
    private float backgroundDefaultAlpha;

    private void Awake()
    {
        textDefaultAlpha = TextMesh.color.a;
        backgroundDefaultAlpha = BackgroundImage.color.a;
        Hide();
    }

    private void OnEnable()
    {
        HoverableHandler.OnUpdateHoverText += UpdateText;
    }

    private void OnDisable()
    {
        HoverableHandler.OnUpdateHoverText -= UpdateText;
    }

    /// <summary>
    /// Updates the displayed hover text. "" or null can be used to clear the hide the text instead.
    /// </summary>
    /// <param name="newMessage"></param>
    public void UpdateText(string newMessage)
    {
        if (string.IsNullOrEmpty(newMessage))
        {
            Hide();
        }
        else
        {
            TextMesh.text = newMessage;
            Show();
        }
    }

    /// <summary>
    /// Hides the hover text by setting the alpha of the text and background image to 0.
    /// </summary>
    public void Hide()
    {
        TextMesh.color = new Color(
            TextMesh.color.r,
            TextMesh.color.g,
            TextMesh.color.b,
            0f
        );
        
        BackgroundImage.color = new Color(
            BackgroundImage.color.r,
            BackgroundImage.color.g,
            BackgroundImage.color.b,
            0f
        );
    }

    /// <summary>
    /// Shows the hover text by setting the alpha of the text and background image to their original values.
    /// </summary>
    public void Show()
    {
        TextMesh.color = new Color(
            TextMesh.color.r,
            TextMesh.color.g,
            TextMesh.color.b,
            textDefaultAlpha
        );
        
        BackgroundImage.color = new Color(
            BackgroundImage.color.r,
            BackgroundImage.color.g,
            BackgroundImage.color.b,
            backgroundDefaultAlpha
        );
    }
}
