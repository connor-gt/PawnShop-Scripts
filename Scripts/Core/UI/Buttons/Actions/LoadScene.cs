using UnityEngine;

public class LoadScene : MonoBehaviour
{
    [SerializeField]
    private SceneField scene;

    public async void Load()
    {
        await SceneSwapManager.Instance.LoadSceneWithFade(scene);
    }
}
