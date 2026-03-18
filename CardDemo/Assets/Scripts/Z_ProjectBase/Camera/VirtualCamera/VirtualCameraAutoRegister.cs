using UnityEngine;
using Cinemachine;

/// <summary>
/// 虚拟摄像机自动注册组件。
/// 将此组件添加到每个CinemachineVirtualCamera对象上，它会自动处理注册和注销。
/// </summary>
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VirtualCameraAutoRegister : MonoBehaviour
{
    private CinemachineVirtualCamera vcam;

    private void Awake()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        if (vcam != null)
        {
            VirtualCameraMgr.Instance.RegisterCamera(vcam);
        }
    }

    private void OnDisable()
    {
        if (vcam != null)
        {
            VirtualCameraMgr.Instance.UnRegisterCamera(vcam);
        }
    }
}