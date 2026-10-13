using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LockCursor();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        // Unlock the cursor when going from a game scene back to the main menu
        UnlockCursor();
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void LockCursor()
    {
        // Hides cursor during gameplay, ESC/Pause can be pressed to reveal it
        Cursor.lockState = CursorLockMode.Locked;
    }
}
