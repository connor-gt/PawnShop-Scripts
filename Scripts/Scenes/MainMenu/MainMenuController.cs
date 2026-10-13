using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        SceneSwapManager.Instance.CompleteSceneRegistration();
    }
}
