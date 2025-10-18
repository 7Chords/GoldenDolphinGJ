using DG.Tweening.Plugins.Core.PathCore;
using System.IO;
using UnityEngine;

/// <summary>
/// 资源加载辅助（最简单实现：传入 Resources 下的相对或绝对路径，返回对应 Sprite）
//— path 示例：
//— "UI/Icon/square" 或 "Assets/Resources/UI/Icon/square.png"
/// </summary>
public static class ResourceUtils
{
    /// <summary>
    /// 根据传入路径加载 Sprite（会规范化路径并去掉扩展名）。
    /// </summary>
    public static Sprite LoadSprite(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            Debug.LogWarning("ResourceUtils.LoadSprite: path 为空");
            return null;
        }

        // 规范化路径：替换反斜杠、去除多余空格与首尾斜杠
        string p = path.Replace('\\', '/').Trim().Trim('/');

        // 去掉可能的 "Assets/Resources/" 前缀
        const string assetsResourcesPrefix = "Assets/Resources/";
        if (p.StartsWith(assetsResourcesPrefix, System.StringComparison.OrdinalIgnoreCase))
            p = p.Substring(assetsResourcesPrefix.Length);

        // 去掉扩展名（如果有）
        p = System.IO.Path.ChangeExtension(p, null);

        p = p.Trim('/');
        if (string.IsNullOrEmpty(p))
        {
            Debug.LogWarning("ResourceUtils.LoadSprite: 规范化后路径为空");
            return null;
        }

        Sprite sprite = Resources.Load<Sprite>(p);
        if (sprite == null)
            Debug.LogWarning($"ResourceUtils: Sprite not found at Resources/{p}");

        return sprite;
    }
}