using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// 虚拟摄像机管理器 (使用Cinemachine)
/// 通过自动注册机制管理场景中的所有虚拟摄像机。
/// </summary>
public class VirtualCameraMgr : BaseManager<VirtualCameraMgr>
{

    #region 成员变量
    // 使用HashSet存储摄像机，提高查找和增删效率
    private readonly HashSet<CinemachineVirtualCamera> registeredCameras = new HashSet<CinemachineVirtualCamera>();

    // 当前激活的虚拟摄像机
    private CinemachineVirtualCamera activeCamera;

    // 定义优先级常量，方便管理
    private const int ActivePriority = 10;
    private const int InactivePriority = 0;
    #endregion

    #region 摄像机切换与状态获取
    /// <summary>
    /// 切换到指定的虚拟摄像机。
    /// </summary>
    /// <param name="camera">要激活的摄像机</param>
    public void SwitchCamera(CinemachineVirtualCamera camera)
    {
        if (camera == null || !registeredCameras.Contains(camera))
        {
            Debug.LogWarning($"尝试切换到一个未注册或为空的摄像机: {camera?.name}");
            return;
        }

        // 将所有其他摄像机的优先级设为低
        foreach (var vcam in registeredCameras)
        {
            vcam.Priority = InactivePriority;
        }

        // 提高目标摄像机的优先级，Cinemachine会自动切换
        camera.Priority = ActivePriority;
        activeCamera = camera;
    }

    /// <summary>
    /// 获取当前激活的虚拟摄像机。
    /// </summary>
    public CinemachineVirtualCamera GetActiveCamera()
    {
        return activeCamera;
    }
    #endregion

    #region 目标设置
    /// <summary>
    /// 为所有已注册的摄像机设置跟随目标 (Follow Target)。
    /// </summary>
    /// <param name="target">要跟随的目标Transform</param>
    public void SetFollowTargetForAll(Transform target)
    {
        foreach (var vcam in registeredCameras)
        {
            vcam.Follow = target;
        }
    }

    /// <summary>
    /// 为所有已注册的摄像机设置朝向目标 (LookAt Target)。
    /// </summary>
    /// <param name="target">要朝向的目标Transform</param>
    public void SetLookAtTargetForAll(Transform target)
    {
        foreach (var vcam in registeredCameras)
        {
            vcam.LookAt = target;
        }
    }
    #endregion

    #region 注册与管理
    /// <summary>
    /// 注册一个虚拟摄像机到管理器。
    /// 通常由VirtualCameraAutoRegister组件自动调用。
    /// </summary>
    /// <param name="camera">要注册的摄像机</param>
    public void RegisterCamera(CinemachineVirtualCamera camera)
    {
        if (camera != null && registeredCameras.Add(camera))
        {
            // 新注册的摄像机默认为非激活状态
            camera.Priority = InactivePriority;
        }
    }

    /// <summary>
    /// 从管理器注销一个虚拟摄像机。
    /// 通常由VirtualCameraAutoRegister组件自动调用。
    /// </summary>
    /// <param name="camera">要注销的摄像机</param>
    public void UnRegisterCamera(CinemachineVirtualCamera camera)
    {
        if (camera != null && registeredCameras.Remove(camera))
        {
            // 如果注销的是当前激活的摄像机，需要进行处理
            if (activeCamera == camera)
            {
                activeCamera = null;
                // 这里可以添加逻辑，比如切换到默认摄像机
            }
        }
    }

    /// <summary>
    /// 清空所有已注册的摄像机，用于场景切换等时机。
    /// </summary>
    public void ClearAllCameras()
    {
        registeredCameras.Clear();
        activeCamera = null;
    }
    #endregion
}
