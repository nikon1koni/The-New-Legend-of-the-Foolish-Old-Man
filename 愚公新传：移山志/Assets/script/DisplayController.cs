using UnityEngine;
using UnityEngine.UI;

public class DisplayController : MonoBehaviour
{
    [Header("显示文本设置")]
    public string productivityLabel = "生产力：";
    public string toolCountLabel = "工具数量：";
    public string populationLabel = "人口：";

    [Header("文本样式设置")]
    public Color textColor = Color.white;
    public int fontSize = 42;
    public Font textFont;

    [Header("左上角布局设置")]
    public float leftMargin = 10f;      // 距离左边距
    public float topMargin = 10f;      // 距离上边距
    public float lineSpacing = 35f;    // 行间距

    [Header("显示设置")]
    public float updateInterval = 0.1f; // 更新间隔（秒）

    // 动态生成的文本组件
    private Text productivityText;
    private Text toolCountText;
    private Text populationText;

    private Canvas canvas;
    private click mainGameController;
    private float lastUpdateTime = 0f;

    void Start()
    {
        // 获取主游戏控制器
        mainGameController = FindObjectOfType<click>();
        if (mainGameController == null)
        {
            //Debug.LogError("DisplayController: 未找到主游戏控制器！");
        }

        // 创建UI显示系统
        CreateUIDisplay();

        Debug.Log("左上角数据显示初始化完成");
    }

    void Update()
    {
        // 按间隔更新显示，避免每帧更新
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateDisplay();
            lastUpdateTime = Time.time;
        }
    }

    // 创建UI显示系统
    private void CreateUIDisplay()
    {
        // 创建或获取Canvas
        CreateCanvas();

        // 创建三个文本显示（精确左上角定位）
        CreateTextDisplay(ref productivityText, productivityLabel, 0);
        CreateTextDisplay(ref toolCountText, toolCountLabel, 1);
        CreateTextDisplay(ref populationText, populationLabel, 2);

        // 初始更新一次显示
        UpdateDisplay();
    }

    // 创建Canvas
    private void CreateCanvas()
    {
        // 检查是否已存在Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            // 创建新的Canvas
            GameObject canvasObj = new GameObject("UI_Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // 添加Canvas Scaler用于屏幕自适应
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasObj.AddComponent<GraphicRaycaster>();
        }
    }

    // 创建单个文本显示（精确左上角定位）
    private void CreateTextDisplay(ref Text textComponent, string label, int lineIndex)
    {
        // 创建文本游戏对象
        GameObject textObj = new GameObject(label.Replace("：", "") + "_Text");
        textObj.transform.SetParent(canvas.transform);

        // 添加Text组件
        textComponent = textObj.AddComponent<Text>();

        // 设置文本基本属性
        textComponent.text = label + "0";
        textComponent.color = textColor;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAnchor.UpperLeft; // 左上角对齐

        // 设置字体
        if (textFont != null)
        {
            textComponent.font = textFont;
        }
        else
        {
            // 使用Arial字体确保清晰度
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        // 设置RectTransform - 关键：精确左上角定位
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();

        // 设置锚点为左上角
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1); // 中心点在左上角

        // 计算精确位置：从屏幕左上角开始
        float yPosition = -topMargin - (lineIndex * lineSpacing);
        rectTransform.anchoredPosition = new Vector2(leftMargin, yPosition);

        // 设置合适的大小
        rectTransform.sizeDelta = new Vector2(400, 35);

        // 添加文字阴影效果（让白色文字在任何背景下都清晰可见）
        Shadow shadow = textObj.AddComponent<Shadow>();
        shadow.effectColor = new Color(0, 0, 0, 0.8f);
        shadow.effectDistance = new Vector2(2, -2);
    }

    // 更新所有显示
    public void UpdateDisplay()
    {
        if (mainGameController == null) return;

        UpdateProductivityDisplay();
        UpdateToolCountDisplay();
        UpdatePopulationDisplay();
    }

    // 更新生产力显示
    private void UpdateProductivityDisplay()
    {
        if (productivityText != null)
        {
            // 直接使用 StringBigInt 的 ToString 或 ToFormattedString 方法
            productivityText.text = $"{productivityLabel}{GetStringBigIntFormattedValue(mainGameController.Productivity)}";
        }
    }

    // 更新工具数量显示
    private void UpdateToolCountDisplay()
    {
        if (toolCountText != null)
        {
            toolCountText.text = $"{toolCountLabel}{GetStringBigIntFormattedValue(mainGameController.Tool_Count)}";
        }
    }

    // 更新人口显示
    private void UpdatePopulationDisplay()
    {
        if (populationText != null)
        {
            populationText.text = $"{populationLabel}{GetStringBigIntFormattedValue(mainGameController.EdenPeople_Count)}";
        }
    }

    // 专门处理 StringBigInt 类型的格式化方法
    private string GetStringBigIntFormattedValue(object stringBigIntValue)
    {
        if (stringBigIntValue == null) return "0";

        // 方法1：尝试调用 ToFormattedString 方法
        try
        {
            var method = stringBigIntValue.GetType().GetMethod("ToFormattedString");
            if (method != null)
            {
                return method.Invoke(stringBigIntValue, null) as string ?? stringBigIntValue.ToString();
            }
        }
        catch
        {
            // 如果反射失败，继续尝试其他方法
        }

        // 方法2：尝试调用 ToString 方法
        try
        {
            var toStringMethod = stringBigIntValue.GetType().GetMethod("ToString", new System.Type[0]);
            if (toStringMethod != null)
            {
                return toStringMethod.Invoke(stringBigIntValue, null) as string ?? "0";
            }
        }
        catch
        {
            // 如果反射失败，继续尝试其他方法
        }

        // 方法3：直接使用 ToString()
        return stringBigIntValue.ToString();
    }

    // 手动强制更新显示
    public void ForceUpdateDisplay()
    {
        UpdateDisplay();
        Debug.Log("左上角显示已强制更新");
    }

    // 设置显示文本颜色
    public void SetTextColor(Color color)
    {
        textColor = color;
        if (productivityText != null) productivityText.color = color;
        if (toolCountText != null) toolCountText.color = color;
        if (populationText != null) populationText.color = color;
    }

    // 设置显示文本字体大小
    public void SetTextFontSize(int newFontSize)
    {
        fontSize = newFontSize;
        if (productivityText != null) productivityText.fontSize = newFontSize;
        if (toolCountText != null) toolCountText.fontSize = newFontSize;
        if (populationText != null) populationText.fontSize = newFontSize;
    }

    // 显示/隐藏所有文本
    public void SetDisplayVisible(bool visible)
    {
        if (productivityText != null) productivityText.enabled = visible;
        if (toolCountText != null) toolCountText.enabled = visible;
        if (populationText != null) populationText.enabled = visible;
    }

    // 调整左上角边距
    public void SetMargins(float left, float top)
    {
        leftMargin = left;
        topMargin = top;
        UpdateTextPositions();
    }

    // 更新所有文本位置
    private void UpdateTextPositions()
    {
        UpdateTextPosition(productivityText, 0);
        UpdateTextPosition(toolCountText, 1);
        UpdateTextPosition(populationText, 2);
    }

    // 更新单个文本位置
    private void UpdateTextPosition(Text textComponent, int lineIndex)
    {
        if (textComponent != null)
        {
            RectTransform rectTransform = textComponent.GetComponent<RectTransform>();
            float yPosition = -topMargin - (lineIndex * lineSpacing);
            rectTransform.anchoredPosition = new Vector2(leftMargin, yPosition);
        }
    }

    // 设置行间距
    public void SetLineSpacing(float spacing)
    {
        lineSpacing = spacing;
        UpdateTextPositions();
    }

    // 调试方法：显示当前使用的值类型信息
    public void DebugValueInfo()
    {
        if (mainGameController != null)
        {
            Debug.Log($"Productivity 类型: {mainGameController.Productivity?.GetType().Name}");
            Debug.Log($"Tool_Count 类型: {mainGameController.Tool_Count?.GetType().Name}");
            Debug.Log($"EdenPeople_Count 类型: {mainGameController.EdenPeople_Count?.GetType().Name}");
        }
    }
}