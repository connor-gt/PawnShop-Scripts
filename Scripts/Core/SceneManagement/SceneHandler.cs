using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles generic scene management/loading and utility.
/// </summary>
public static class SceneHandler
{
    /// <summary>
    /// Load a scene by name (if the input sceneName is null or empty, the current active scene is reloaded).
    /// <para>
    /// This will unload all other currently loaded scenes, including additively loaded scenes.
    /// </para>
    /// </summary>
    /// <param name="sceneName">The name of the scene to load. If this is null or empty, the current active
    /// scene is reloaded.</param>
    public static async UniTask LoadSceneAsync(string sceneName)
    {
        // Reloads the current active scene if sceneName is null or empty. The reason this is not apart of the
        // overloaded method is because for scenes to be reloaded, it should not be additively reloaded which
        // may lead to weird behavior since I'm not sure how to handle preventing that
        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        await LoadSceneAsync(sceneName, additive: false, setActive: true);
    }

    /// <summary>
    /// Asynchronously loads a scene by name, with optional additive loading and setting as the active scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    /// <param name="additive">If true, loads the scene additively; otherwise loads it in single mode, replacing
    /// the current scene.</param>
    /// <param name="setActive">If true, sets the loaded scene as the active scene after loading completes.</param>
    /// <returns>A task that completes once the scene has finished loading.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <paramref name="setActive"/> is true and the loaded
    /// scene cannot be set as active.</exception>
    public static async UniTask LoadSceneAsync(string sceneName, bool additive = false, bool setActive = false)
    {
        await SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);

        if (setActive)
        {
            SetActiveScene(sceneName);
        }
    }

    /// <summary>
    /// Asynchronously unloads a scene by name.
    /// </summary>
    /// <param name="sceneName">The name of the scene to unload. The scene must already be loaded.</param>
    /// <exception cref="InvalidOperationException">Thrown if the specified scene is not valid or is not currently
    /// loaded.</exception>
    public static async UniTask UnloadSceneAsync(string sceneName)
    {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);

        if (!loadedScene.IsValid())
        {
            throw new InvalidOperationException($"Failed to unload '{sceneName}'. Scene '{sceneName}' is invalid.");
        }

        await SceneManager.UnloadSceneAsync(sceneName);
    }

    /// <summary>
    /// Sets the specified scene as the active scene.
    /// </summary>
    /// <param name="sceneName">The name of the scene to set as active. The scene must already be loaded.</param>
    /// <exception cref="InvalidOperationException">Thrown if the specified scene is not valid or is not currently
    /// loaded.</exception>
    public static void SetActiveScene(string sceneName)
    {
        Scene loadedScene = SceneManager.GetSceneByName(sceneName);

        if (!loadedScene.IsValid())
        {
            throw new InvalidOperationException($"Failed to set new active scene. Scene '{sceneName}' is invalid.");
        }

        SceneManager.SetActiveScene(loadedScene);
    }

    /// <summary>
    /// Get a list of scene names that are currently included in the build.
    /// </summary>
    /// <returns>An array of strings containing the scene names.</returns>
    public static string[] GetSceneNamesInBuild()
    {
        int count = SceneManager.sceneCountInBuildSettings;
        string[] names = new string[count];
        for (int i = 0; i < count; i++)
        {
            names[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }

        return names;
    }

    /// <summary>
    /// Wrapper for <see cref="SceneManager.GetActiveScene"/>.
    /// </summary>
    /// <returns>The current active scene.</returns>
    public static Scene GetActiveScene()
    {
        return SceneManager.GetActiveScene();
    }

    /// <summary>
    /// Check if a scene is currently loaded.
    /// <para>
    /// <b>Note:</b> this returns false while a scene is still loading asynchronously.
    /// </para>
    /// </summary>
    /// <param name="sceneName">The name of the scene to check.</param>
    /// <returns>True if the scene is fully loaded in, false if otherwise.</returns>
    public static bool IsSceneLoaded(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        return scene.isLoaded;
    }
}
