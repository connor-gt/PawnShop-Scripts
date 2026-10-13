#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Editor-only script that provides functionality for marking entire hierarchies of game objects as static.
/// </summary>
[ExecuteInEditMode]
public class StaticManager : MonoBehaviour
{
    [Tooltip("The list of game objects to set static. Each game object in this list will have its entire "
        + "hierarchy set to the specified static value.")]
    [SerializeField]
    private List<GameObject> gameObjects = new();

    [Tooltip("The static value to set for the specified game objects and their hierarchies.")]
    [SerializeField]
    private bool isStatic = true;

    [Tooltip("If enabled, the specified game objects will automatically be marked static when this component "
        + "is enabled (only executes in edit mode, not in play mode).")]
    [SerializeField]
    private bool setStaticOnEnable = true;

    private readonly StaticUtility staticUtility = new();

    private void OnEnable()
    {
        if (!Application.isPlaying && setStaticOnEnable)
        {
            SetStaticForAll();
        }
    }

    [Button("Set Static Recursive")]
    public void SetStaticForAll()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            staticUtility.SetStatic(gameObjects[i], isStatic);
        }
    }
}
#endif
