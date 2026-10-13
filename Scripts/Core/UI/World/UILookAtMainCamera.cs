using UnityEngine;

/// <summary>
/// Rotates this UI object so it always faces the player's main camera.
/// </summary>
public class UILookAtMainCamera : MonoBehaviour
{
    private Transform _playerCam = null;
    private Transform PlayerCam
    {
        get
        {
            if (_playerCam == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    _playerCam = Camera.main.transform;
                }
            }

            return _playerCam;
        }
    }
    
    private void LateUpdate()
    {
        if (PlayerCam != null)
        {
            FaceCamera();
        }
    }

    /// <summary>
    /// Rotates the object to face the camera by aligning its forward direction toward the camera's position.
    /// </summary>
    private void FaceCamera()
    {
        Vector3 toCamDir = transform.position - PlayerCam.transform.position;
        transform.rotation = Quaternion.LookRotation(toCamDir, Vector3.up);
    }
}
