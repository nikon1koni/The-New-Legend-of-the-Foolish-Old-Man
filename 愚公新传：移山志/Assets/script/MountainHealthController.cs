using UnityEngine;
using UnityEngine.UI;

public class MountainHealthController : MonoBehaviour
{
    [Header("点击设置")]
    public int damagePerClick = 1;
    public float clickCooldown = 0.0f;
    private float lastClickTime = 0f;

    [Header("UI显示")]
    public Slider healthSlider;
    public Text healthText;
    public Text levelText; // 新增：显示山体等级
    public Canvas healthCanvas;
    public Vector3 uiOffset = new Vector3(0, 2f, 0);

    private click gameController;
    private Camera mainCamera;

    void Start()
    {
        // 从全局数据加载山体
        LoadMountainFromGameData();

        // 获取引用
        gameController = FindObjectOfType<click>();
        mainCamera = Camera.main;

        // 创建或初始化UI
        InitializeHealthUI();

        Debug.Log($"山体系统初始化 - 第{GameDataManager.Instance.CurrentMountainLevel}座山，生命值: {GameDataManager.Instance.MountainCurrentHealth}/{GameDataManager.Instance.MountainMaxHealth}");
    }

    void Update()
    {
        UpdateUIPosition();

        // 点击检测（只在山体未被摧毁时有效）
        if (!GameDataManager.Instance.IsMountainDestroyed &&
            Input.GetMouseButtonDown(0) &&
            Time.time - lastClickTime >= clickCooldown)
        {
            if (IsMouseClickOnMountain())
            {
                HandleMountainClick();
            }
        }
    }

    // 从GameDataManager加载山体数据
    private void LoadMountainFromGameData()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager实例不存在！");
            return;
        }

        // 如果山体已被摧毁，初始化新山体
        if (GameDataManager.Instance.IsMountainDestroyed)
        {
            GameDataManager.Instance.InitializeNewMountain();
        }

        // 确保有有效的山体数据
        if (GameDataManager.Instance.MountainMaxHealth == 0)
        {
            GameDataManager.Instance.InitializeNewMountain();
        }
    }

    private void InitializeHealthUI()
    {
        if (healthCanvas == null)
        {
            CreateHealthUI();
        }
        UpdateHealthUI();
    }

    private void CreateHealthUI()
    {
        // 创建Canvas（同原来代码）
        GameObject canvasObj = new GameObject("MountainHealthCanvas");
        healthCanvas = canvasObj.AddComponent<Canvas>();
        healthCanvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<GraphicRaycaster>();

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2f, 0.8f); // 增加高度以容纳等级显示

        // 创建等级文本
        GameObject levelObj = new GameObject("LevelText");
        levelObj.transform.SetParent(canvasObj.transform);
        levelText = levelObj.AddComponent<Text>();
        levelText.color = Color.yellow;
        levelText.fontSize = 16;
        levelText.alignment = TextAnchor.MiddleCenter;
        levelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        levelText.text = $"第{GameDataManager.Instance.CurrentMountainLevel}座山";

        RectTransform levelRect = levelObj.GetComponent<RectTransform>();
        levelRect.sizeDelta = new Vector2(2f, 0.3f);
        levelRect.anchoredPosition = new Vector2(0, 0.4f);

        // 创建血条Slider（同原来代码）
        GameObject sliderObj = new GameObject("HealthSlider");
        sliderObj.transform.SetParent(canvasObj.transform);
        healthSlider = sliderObj.AddComponent<Slider>();

        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.sizeDelta = new Vector2(2f, 0.2f);
        sliderRect.anchoredPosition = new Vector2(0, 0.1f);

        healthSlider.minValue = 0;
        healthSlider.maxValue = GameDataManager.Instance.MountainMaxHealth;
        healthSlider.value = GameDataManager.Instance.MountainCurrentHealth;

        // 创建背景
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(sliderObj.transform);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = Color.gray;
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // 创建填充
        GameObject fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(sliderObj.transform);
        Image fillImage = fillObj.AddComponent<Image>();
        fillImage.color = Color.red;
        healthSlider.fillRect = fillObj.GetComponent<RectTransform>();
        RectTransform fillRect = fillObj.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;

        // 创建血量文本
        GameObject textObj = new GameObject("HealthText");
        textObj.transform.SetParent(canvasObj.transform);
        healthText = textObj.AddComponent<Text>();
        healthText.color = Color.white;
        healthText.fontSize = 14;
        healthText.alignment = TextAnchor.MiddleCenter;
        healthText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(2f, 0.3f);
        textRect.anchoredPosition = new Vector2(0, -0.2f);
    }

    private void UpdateUIPosition()
    {
        if (healthCanvas != null)
        {
            healthCanvas.transform.position = transform.position + uiOffset;
            healthCanvas.transform.rotation = Quaternion.LookRotation(
                healthCanvas.transform.position - mainCamera.transform.position);
        }
    }

    private void HandleMountainClick()
    {
        if (gameController == null || !IsMouseClickOnMountain()) return;

        if (GameDataManager.Instance.Tool_Count >= new StringBigInt(damagePerClick.ToString()))
        {
            // 消耗工具并造成伤害
            GameDataManager.Instance.Tool_Count -= new StringBigInt(damagePerClick.ToString());
            TakeDamage(damagePerClick);
            lastClickTime = Time.time;
        }
        else
        {
            Debug.Log("工具不足，无法攻击山!");
        }
    }

    private bool IsMouseClickOnMountain()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit)
        {
            return hit.collider.gameObject == gameObject;
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        GameDataManager.Instance.MountainCurrentHealth -= damage;
        if (GameDataManager.Instance.MountainCurrentHealth < 0)
            GameDataManager.Instance.MountainCurrentHealth = 0;

        UpdateHealthUI();
        GameDataManager.Instance.SaveGameData();

        if (GameDataManager.Instance.MountainCurrentHealth <= 0)
        {
            DestroyMountain();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = GameDataManager.Instance.MountainMaxHealth;
            healthSlider.value = GameDataManager.Instance.MountainCurrentHealth;
        }
        if (healthText != null)
            healthText.text = $"{GameDataManager.Instance.MountainCurrentHealth}/{GameDataManager.Instance.MountainMaxHealth}";
        if (levelText != null)
            levelText.text = $"第{GameDataManager.Instance.CurrentMountainLevel}座山";
    }

    // 在MountainHealthController.cs的DestroyMountain方法中修改
    private void DestroyMountain()
    {
        int currentLevel = GameDataManager.Instance.CurrentMountainLevel;
        Debug.Log($"第{currentLevel}座山已被挖塌!");

        // 特殊处理第五座山
        if (currentLevel == 5)
        {
            // 保存游戏数据
            GameDataManager.Instance.SaveGameData();
            // 直接跳转到Ending场景
            UnityEngine.SceneManagement.SceneManager.LoadScene("Ending");
            return; // 跳过后续处理
        }

        // 通知游戏控制器播放相应的对话
        if (gameController != null)
        {
            gameController.PlayMountainDestroyedDialogue(currentLevel);
        }

        // 设置山为已摧毁状态
        GameDataManager.Instance.IsMountainDestroyed = true;

        // 检查是否可以生成新的山
        if (GameDataManager.Instance.CanSpawnNewMountain())
        {
            Debug.Log("准备生成下一座新的山...");
        }
        else
        {
            Debug.Log("已达到最大山脉等级，游戏结束!");
        }

        // 保存当前状态
        GameDataManager.Instance.SaveGameData();

        // 清理当前山的UI
        if (healthCanvas != null) Destroy(healthCanvas.gameObject);
        Destroy(gameObject);
    }
}