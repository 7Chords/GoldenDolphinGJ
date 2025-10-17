using UnityEngine;

/// <summary>
/// 管理音符的生成
/// </summary>
public class NoteMgr : MonoBehaviour
{
    [Header("配置参数")]
    public float yOffset = 0;  // Y轴偏移（控制高度，UI像素单位）
    public float timeCounter = 2f; // 生成间隔（秒）
    public float margin = 50f; // UI与屏幕边缘的留白（像素）
    [SerializeField] private GameObject notePrefab; // 必须是带RectTransform的UI预制体
    [SerializeField] private Camera targetCamera;   // 正交相机
    [SerializeField] private Canvas targetCanvas;   // UI所属的Canvas（必填）

    private RectTransform canvasRect; // Canvas的RectTransform组件

    void Start()
    {
        // 自动获取必要组件，避免配置遗漏
        if (targetCamera == null)
            targetCamera = Camera.main;

        // 校验相机是否为正交模式
        if (targetCamera != null && !targetCamera.orthographic)
            Debug.LogError("目标相机必须设置为正交模式（Orthographic）！");

        // 自动获取Canvas（优先使用指定的，其次找场景中第一个Canvas）
        if (targetCanvas == null)
            targetCanvas = FindObjectOfType<Canvas>();

        // 获取Canvas的RectTransform（UI坐标转换必需）
        if (targetCanvas != null)
            canvasRect = targetCanvas.GetComponent<RectTransform>();
        else
            Debug.LogError("未找到Canvas！请在场景中创建Canvas并赋值给脚本");
    }

    void Update()
    {
        // 配置不全时不执行生成逻辑
        if (notePrefab == null || targetCamera == null || targetCanvas == null || canvasRect == null)
            return;

        timeCounter -= Time.deltaTime;
        if (timeCounter < 0f)
        {
            SpawnInViewRandom();
        }
    }

    // 随机生成UI音符预制体（仅适配正交相机）
    void SpawnInViewRandom()
    {
        // 复位计时器
        timeCounter = 2f;

        // 正交相机参数计算
        float orthoSize = targetCamera.orthographicSize;
        float aspectRatio = targetCamera.aspect;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 1. 计算屏幕可用范围（扣除边缘留白）
        float minScreenX = margin;
        float maxScreenX = screenWidth - margin;
        float minScreenY = margin;
        float maxScreenY = screenHeight - margin;

        // 2. 生成屏幕内随机像素坐标
        float randomScreenX = Random.Range(minScreenX, maxScreenX);
        float randomScreenY = Random.Range(minScreenY, maxScreenY);
        Vector2 randomScreenPos = new Vector2(randomScreenX, randomScreenY);

        // 3. 将屏幕坐标转换为Canvas局部坐标（UI专用定位）
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            randomScreenPos,
            null, // Overlay模式Canvas无需相机
            out Vector2 uiLocalPos))
        {
            // 4. 实例化UI预制体
            GameObject tempObj = Instantiate(notePrefab, targetCanvas.transform);
            RectTransform noteRect = tempObj.GetComponent<RectTransform>();
            if (noteRect != null)
            {
                // 设置UI位置（叠加Y轴偏移，像素单位）
                noteRect.anchoredPosition = new Vector2(uiLocalPos.x, uiLocalPos.y + yOffset);
                // 重置旋转和缩放（避免继承父物体属性）
                noteRect.localRotation = Quaternion.identity;
                noteRect.localScale = Vector3.one;
            }
        }
    }

   
}