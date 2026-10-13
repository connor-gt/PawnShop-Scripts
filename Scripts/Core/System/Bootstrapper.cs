using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    /// <summary>
    /// Load the Systems prefab from /Resources into DontDestroyOnLoad before the first scene is loaded.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        DontDestroyOnLoad(Instantiate(Resources.Load("Systems")));
    }
}
