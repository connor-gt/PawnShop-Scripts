using UnityEngine;

/// <summary>
/// Change the skybox via <see cref="RenderSettings.skybox"/>.
/// </summary>
public class SkyboxChanger
{
    /// <summary>
    /// The skybox that is being used when this class is created.
    /// </summary>
    public readonly Material DefaultSkybox;

    public SkyboxChanger()
    {
        DefaultSkybox = RenderSettings.skybox;
    }

    /// <summary>
    /// Change the current skybox to the desired skybox.
    /// </summary>
    /// <param name="newSkybox">The skybox to change to.</param>
    public void ChangeTo(Material newSkybox)
    {
        RenderSettings.skybox = newSkybox;
    }

    /// <summary>
    /// Reset the skybox back to <see cref="DefaultSkybox"/>.
    /// <para>
    /// The default is considered to be the skybox that was active when <see cref="SkyboxChanger"/> was created.
    /// </para>
    /// </summary>
    public void Reset()
    {
        ChangeTo(DefaultSkybox);
    }
}
