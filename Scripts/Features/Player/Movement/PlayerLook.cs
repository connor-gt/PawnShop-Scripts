using Sirenix.OdinInspector;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerLook : MonoBehaviour
{
    [Title("References")]
    [SerializeField]
    private CinemachineInputAxisController inputAxisController; // For updating look sensitivity

    [SerializeField]
    private CinemachinePanTilt panTilt;
}
