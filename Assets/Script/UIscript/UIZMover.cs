using UnityEngine;

/// <summary>
/// 最简单的 Z 轴匀速旋转器
/// 挂到任意对象上，若需要对其它对象旋转，拖入 target 引用；否则会作用于自身 transform。
/// speed 为角速度（度/秒），正值为正向旋转。
/// </summary>
public class UIZMover : MonoBehaviour
{
    [Header("可拖入要旋转的目标（留空则作用于自身）")]
    [SerializeField] private Transform target;

    [Header("Z 轴角速度（度/秒），正为正向")]
    [SerializeField] private float speed = 90f;

    [Header("是否使用局部坐标旋转（勾选则使用 Space.Self，否则使用 Space.World）")]
    [SerializeField] private bool useLocal = true;

    private void Reset()
    {
        if (target == null) target = transform;
    }

    private void OnValidate()
    {
        if (target == null) target = transform;
    }

    private void Update()
    {
        if (target == null) return;
        if (NoteMgr.instance != null && NoteMgr.instance.IsCurrentPause) return;

        float deltaAngle = speed * Time.deltaTime;
        // 正确旋转方式：使用 Rotate（避免直接修改 Quaternion 分量）
        if (useLocal)
            target.Rotate(Vector3.forward, deltaAngle, Space.Self);
        else
            target.Rotate(Vector3.forward, deltaAngle, Space.World);
    }
}