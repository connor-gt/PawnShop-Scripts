using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuitApplication : MonoBehaviour
{
    public void Quit()
    {
        // Prevents OperationCanceledException when quitting the game before the sceneswap manager has
        // finished doing fade in
        if (SceneSwapManager.Instance.IsLoadingScene)
        {
            return;
        }

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
