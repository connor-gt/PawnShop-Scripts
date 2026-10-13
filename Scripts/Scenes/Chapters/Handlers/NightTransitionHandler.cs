using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class NightTransitionHandler : MonoBehaviour
{
    [Tooltip("The skybox to change to when entering the night chapter phase.")]
    [SerializeField]
    [Required]
    private Material nightSkybox;

    private SkyboxChanger skyboxChanger;

    private void Start()
    {
        skyboxChanger = new SkyboxChanger();
    }

    /// <summary>
    /// Set the skybox to the night skybox that's set in the inspector.
    /// </summary>
    public async UniTask SetToNight()
    {
        // NOTE: this is async for now because there might need to be awaitable stuff to make sure that the
        // player doesn't awaken before the night scene is fully ready

        skyboxChanger.ChangeTo(nightSkybox);
    }
}
