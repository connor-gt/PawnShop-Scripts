using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(TextMeshProUGUI))]
public class KeybindingText : MonoBehaviour
{
    [SerializeField]
    private InputActionReference inputReference;

    [SerializeField]
    private string label = "Label";

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void SetInputActionReference(InputActionReference inputReference)
    {
        this.inputReference = inputReference;
        UpdateUI();
    }

    public void SetLabel(string label)
    {
        this.label = label;
        UpdateUI();
    }

    public void UpdateUI()
    {
        text.text = InputManager.Instance.GetFormattedBindingDisplay(inputReference, label);
    }
}
