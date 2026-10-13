using TMPro;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class ButtonController : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Gets or sets whether or not this <see cref="ButtonController"/> is interactable.
    /// </summary>
    [field: SerializeField]
    public bool IsInteractable { get; set; } = true;

    [Title("Text")]
    [Tooltip("If enabled, the button component's text will be overwritten by the below string.")]
    [SerializeField]
    private bool useCustomText = true;

    [SerializeField]
    private string _buttonText = "Button";

    /// <summary>
    /// Gets or sets the button's current text.
    /// <para>
    /// <b>Note:</b> This is dependent on the <see cref="useCustomText"/> private field being set to true in the
    /// inspector.
    /// </para>
    /// </summary>
    public string ButtonText
    {
        get => _buttonText;
        set
        {
            if (!useCustomText)
            {
                return;
            }

            _buttonText = value;

            if (NormalText != null)
            {
                NormalText.text = _buttonText;
            }

            if (HighlightedText != null)
            {
                HighlightedText.text = _buttonText;
            }
        }
    }

    [Title("Normal Button References")]
    public CanvasGroup NormalCG;
    public TextMeshProUGUI NormalText;

    [Title("Highlighted Button References")]
    public CanvasGroup HighlightCG;
    public TextMeshProUGUI HighlightedText;

    [Title("Disabled Button References")]
    public CanvasGroup DisabledCG;
    public TextMeshProUGUI DisabledText;

    /// <summary>
    /// Gets this <see cref="ButtonController"/>'s audio source.
    /// </summary>
    [field: Title("Audio")]
    [field: Tooltip("This should be left null if no audio should be played from this button.")]
    [field: SerializeField]
    public AudioSource AudioSource { get; private set; }

    [SerializeField]
    private AudioClip hoverClip;

    [SerializeField]
    private AudioClip clickClip;

    [Title("Events")]
    public UnityEvent OnHover = new();
    public UnityEvent OnClick = new();
    public UnityEvent OnLeave = new();

#if UNITY_EDITOR
    public void OnValidate()
    {
        // Update the button text live in the editor outside of playmode
        if (!Application.isPlaying)
        {
            ButtonText = _buttonText;
        }
    }
#endif

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable)
        {
            return;
        }

        PlayClip(hoverClip);
        OnHover?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable)
        {
            return;
        }

        PlayClip(clickClip);
        OnClick?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsInteractable)
        {
            return;
        }

        OnLeave?.Invoke();
    }

    private void PlayClip(AudioClip clip)
    {
        if (AudioSource != null)
        {
            AudioSource.PlayOneShot(clip);
        }
    }
}
