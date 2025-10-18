using UnityEngine;

/// <summary>
/// 最简单的 Z 轴匀速移动器
/// 挂到任意对象上，若需要对其它对象移动，拖入 target 引用；否则会移动自身 transform。
/// speed 为正向 +Z；若想反向，填负值即可。
/// </summary>
public class UIZMover : MonoBehaviour
{
    [Header("可拖入要移动的目标（留空则移动自身）")]
    [SerializeField] private Transform target;

    [Header("Z 轴速度（单位：单位/秒；正为 +Z，负为 -Z）")]
    [SerializeField] private float speed = 10f;

    [Header("是否在局部坐标系下移动（勾选则修改 localPosition.z，否则修改 position.z）")]
    [SerializeField] private bool useLocal = true;

    private void Reset()
    {
        // 方便在 Inspector 中默认使用自身
        if (target == null) target = transform;
    }

    private void OnValidate()
    {
        // 编辑器下也保持默认
        if (target == null) target = transform;
    }

    private void Update()
    {
        if (target == null || NoteMgr.instance.IsCurrentPause) return;

        float dz = speed * Time.deltaTime;
        if (useLocal)
        {
            Vector3 lp = target.localPosition;
            lp.z += dz;
            target.localPosition = lp;
        }
        else
        {
            Vector3 wp = target.position;
            wp.z += dz;
            target.position = wp;
        }
    }
}