using UnityEngine;

/// <summary>
/// Helper script that can be placed on held item prefabs to auto-set common functions/requirements.
/// </summary>
public class HeldItemCreationHelper : MonoBehaviour
{
    public const string HELD_ITEM_LAYER = "Held Item";

#if UNITY_EDITOR
    // Triggers when this component is added to a prefab
    private void Reset()
    {
        // Set this game object and all children layers to the held item layer
        gameObject.SetLayerRecursive(LayerMask.NameToLayer(HELD_ITEM_LAYER));

        // Turn off shadows for held items. Since there are no player arms/body, I think it's a bit weird to see a
        // random floating flashlight shadow with nothing else (as an example) so held items should not have shadows
        if (TryGetComponent(out Renderer renderer))
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        // Auto-clear this script safely by waiting until the next editor frame update
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null)
            {
                Debug.Log($"{name} successful prefab initialization and {GetType().Name} self clean-up.");
                DestroyImmediate(this, true);
            }
        };
    }
#endif
}
