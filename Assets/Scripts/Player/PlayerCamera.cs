using UnityEngine;
using Cinemachine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _camera;

    public void Initialize(Transform followNLook, int prio)
    {
        _camera.Follow = followNLook;
        _camera.LookAt = followNLook;

        SetPriority(prio);
    }

    public void SetPriority(int prio)
    {
        _camera.Priority = prio;
    }
}