using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Serializable wrapper that allows referencing scenes within the Unity inspector.
/// </summary>
[System.Serializable]
public class SceneField
{
    [SerializeField]
    private Object _sceneAsset;

    [field: SerializeField]
    private string _sceneName = "";
    public string SceneName
    {
        get { return _sceneName; }
    }

    /// <summary>
    /// Makes this work with the existing Unity methods (LoadLevel, LoadScene, etc.) by converting the SceneAsset
    /// to its string name automatically. This allows the passing of SceneField to anywhere a string is expected.
    /// </summary>
    /// <param name="sceneField">The scene to reference.</param>
    public static implicit operator string(SceneField sceneField)
    {
        return sceneField.SceneName;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawar : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        SerializedProperty sceneAsset = property.FindPropertyRelative("_sceneAsset");
        SerializedProperty sceneName = property.FindPropertyRelative("_sceneName");
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        if (sceneAsset != null)
        {
            sceneAsset.objectReferenceValue = EditorGUI.ObjectField(
                position,
                sceneAsset.objectReferenceValue,
                typeof(SceneAsset),
                false
            );

            if (sceneAsset.objectReferenceValue != null)
            {
                sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
