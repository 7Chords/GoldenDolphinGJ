using GJFramework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理音符的生成
/// </summary>
public class NoteMgr : SingletonMono<NoteMgr>
{
    [Header("配置参数")]
    public float yOffset = 0;  // Y轴偏移（控制高度，UI像素单位）
    public float timeCounter = 2f; // 生成间隔（秒）
    public float margin = 50f; // UI与屏幕边缘的留白（像素）
    [SerializeField] private GameObject notePrefab; // 必须是带RectTransform的UI预制体
    [SerializeField] private Camera targetCamera;   // 正交相机
    [SerializeField] private Canvas targetCanvas;   // UI所属的Canvas（必填）
    private RectTransform canvasRect; // Canvas的RectTransform组件
    private bool isCurrentPause = false;
    public bool isBack = false;
    public bool IsCurrentPause { get { return isCurrentPause; } }
    [SerializeField] private float totalTime;
    public float TotalTime { get { return totalTime; } }

    private int remainNoteNum;// 剩余可获得音符的数量

    public bool isEnd = false;

    [Header("禁止生成区域（把对应的 UI RectTransform 拖到这里）")]
    [Tooltip("如果某个区域不希望生成音符，把该区域的 RectTransform 拖到数组里。支持多区域。")]
    [SerializeField] private RectTransform[] forbiddenZones;

    [Header("生成尝试次数（如果多次随机落入禁止区则放弃本次生成）")]
    [SerializeField] private int maxSpawnAttempts = 12;

    void Start()
    {
        // 赋值
        remainNoteNum = ConstVar.MAX_NOTE_NUM;
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
        DoInUpdate();
    }

    public void ReduceRemainNoteNum()
    {
        remainNoteNum--;
    }

    private void DoInUpdate()
    {
        // 生成时检测暂停状态
        // 一旦进入就开始减少总收集时间
        // 非暂停时计时生成间隔
        if (remainNoteNum <= 0 || (totalTime < 0f && !isEnd))
        {
            isEnd = true;
            PanelUIMgr.Instance.OpenPanel(EPanelType.ColloctFinishPanel);
            remainNoteNum = ConstVar.MAX_NOTE_NUM;
        }
        totalTime -= Time.deltaTime;
        // 非暂停状态下计时生成音符

        if (!isCurrentPause && !isEnd)
        {
            timeCounter -= Time.deltaTime;
            if (timeCounter < 0f)
            {
                SpawnInViewRandom();
            }
        }

    }
    // 随机生成UI音符预制体（仅适配正交相机）
    void SpawnInViewRandom()
    {
        // 复位计时器
        timeCounter = 0.5f;

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

        // 将可用范围在中心缩小为 80%（长宽各 x0.8），避免大幅改动其它逻辑
        const float scaleFactor = 0.8f;
        float usableWidth = (maxScreenX - minScreenX) * scaleFactor;
        float usableHeight = (maxScreenY - minScreenY) * scaleFactor;
        float centerX = (minScreenX + maxScreenX) * 0.5f;
        float centerY = (minScreenY + maxScreenY) * 0.5f;
        minScreenX = centerX - usableWidth * 0.5f;
        maxScreenX = centerX + usableWidth * 0.5f;
        minScreenY = centerY - usableHeight * 0.5f;
        maxScreenY = centerY + usableHeight * 0.5f;

        // 2. 生成屏幕内随机像素坐标，允许多次尝试避免落在禁止区域
        Vector2 randomScreenPos = Vector2.zero;
        bool foundValid = false;
        int attempts = 0;
        while (attempts < maxSpawnAttempts)
        {
            float randomScreenX = Random.Range(minScreenX, maxScreenX);
            float randomScreenY = Random.Range(minScreenY, maxScreenY);
            randomScreenPos = new Vector2(randomScreenX, randomScreenY);

            if (!IsInForbiddenScreenPoint(randomScreenPos))
            {
                foundValid = true;
                break;
            }
            attempts++;
        }

        if (!foundValid)
        {
            // 多次尝试仍未找到有效位置，则本次不生成（避免卡死循环或生成在禁区）
            return;
        }

        // 3. 将屏幕坐标转换为Canvas局部坐标（UI专用定位）
        // 对于 Overlay 模式 camera 参数传 null；其他模式传 targetCamera
        Camera camForScreenPoint = (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : targetCamera;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            randomScreenPos,
            camForScreenPoint,
            out Vector2 uiLocalPos))
        {
            // 4. 实例化UI预制体
            GameObject tempObj = Instantiate(notePrefab, targetCanvas.transform);
            RectTransform noteRect = tempObj.GetComponent<RectTransform>();
            Image image = tempObj.GetComponentInChildren<Image>();
            if (image != null) image.SetNativeSize();
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

    // 判断屏幕坐标点是否落在任一禁止区域内（禁止区域以 RectTransform 指定）
    private bool IsInForbiddenScreenPoint(Vector2 screenPoint)
    {
        if (forbiddenZones == null || forbiddenZones.Length == 0)
            return false;

        // 根据 Canvas 渲染模式选择用于 WorldToScreenPoint 的相机
        Camera camForScreenPoint = (targetCanvas != null && targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : targetCamera;

        foreach (var rt in forbiddenZones)
        {
            if (rt == null) continue;
            // 获取世界四个角并转换为屏幕坐标
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
            for (int i = 0; i < 4; i++)
            {
                Vector2 sp = RectTransformUtility.WorldToScreenPoint(camForScreenPoint, corners[i]);
                if (sp.x < minX) minX = sp.x;
                if (sp.x > maxX) maxX = sp.x;
                if (sp.y < minY) minY = sp.y;
                if (sp.y > maxY) maxY = sp.y;
            }
            // 判断点是否在矩形内（包含边界）
            if (screenPoint.x >= minX && screenPoint.x <= maxX && screenPoint.y >= minY && screenPoint.y <= maxY)
                return true;
        }
        return false;
    }

    // 当前是否暂停 对外暴露接口
    public void SetPauseState(bool isPause)
    {
        isCurrentPause = isPause;
    }

    private void OpenStore()
    {
    }
}