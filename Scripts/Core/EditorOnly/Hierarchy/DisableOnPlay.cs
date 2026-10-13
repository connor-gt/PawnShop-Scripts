#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Editor-only script that can be used to turn off game objects when entering play mode.
/// </summary>
public class DisableOnPlay : MonoBehaviour
{
    [Tooltip("If enabled, this game object will be turned off via Start() when entering play mode.")]
    [SerializeField]
    private bool disableInPlayMode = true;

    private void Start()
    {
        if (disableInPlayMode)
        {
            gameObject.SetActive(false);
        }
    }
}
#endif
