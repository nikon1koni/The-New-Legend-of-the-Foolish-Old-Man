using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Text;

public class click : MonoBehaviour
{
    public GameObject DialogManagerPrefab1;//预制体
    public GameObject DialogManagerPrefab2;//预制体
    public GameObject DialogManagerPrefab3;//预制体
    public GameObject DialogManagerPrefab4;//预制体
    public GameObject DialogManagerPrefab5;//预制体

    public GameObject Village;
    public GameObject Workshop;
    public GameObject Eden;
    public SpriteRenderer Mountain1;
    public SpriteRenderer Mountain2;

    // 使用GameDataManager的数据
    public StringBigInt Productivity => GameDataManager.Instance.Productivity;
    public StringBigInt Tool_Count => GameDataManager.Instance.Tool_Count;
    public StringBigInt EdenPeople_Count => GameDataManager.Instance.EdenPeople_Count;

    public StringBigInt Consume_Workshop => GameDataManager.Consume_Workshop;
    public StringBigInt Consume_Eden => GameDataManager.Consume_Eden;
    public StringBigInt One => GameDataManager.One;

    public float Time_Eden = 0;

    // 点击冷却时间
    private float lastClickTime = 0f;
    private const float CLICK_COOLDOWN = 0.5f;

    // 单例实例
    private static click _instance;

    // 对话播放状态标记
    private static bool dialogue3Played = false;
    private static bool dialogue4Played = false;
    private static bool dialogue5Played = false;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 加载对话播放状态
        LoadDialogueStates();
    }

    void Start()
    {
        Time.timeScale = 1f;
        EnsureGameDataManager();
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("游戏主控制器初始化完成");

        // 初始化场景1的对话系统
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            GameObject dialogInstance = Instantiate(DialogManagerPrefab1);
            Debug.Log("场景1对话系统初始化完成");
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SaveDialogueStates();
    }

    // 保存和加载对话播放状态
    private void SaveDialogueStates()
    {
        PlayerPrefs.SetInt("Dialogue3Played", dialogue3Played ? 1 : 0);
        PlayerPrefs.SetInt("Dialogue4Played", dialogue4Played ? 1 : 0);
        PlayerPrefs.SetInt("Dialogue5Played", dialogue5Played ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadDialogueStates()
    {
        dialogue3Played = PlayerPrefs.GetInt("Dialogue3Played", 0) == 1;
        dialogue4Played = PlayerPrefs.GetInt("Dialogue4Played", 0) == 1;
        dialogue5Played = PlayerPrefs.GetInt("Dialogue5Played", 0) == 1;
    }

    // 重置对话状态（用于测试或重置游戏时）
    public void ResetDialogueStates()
    {
        dialogue3Played = false;
        dialogue4Played = false;
        dialogue5Played = false;
        SaveDialogueStates();
    }

    private static bool isScene2DialogInitialized = false;

    // 在click.cs的OnSceneLoaded方法中添加
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"场景加载完成: {scene.name}");
        FindSceneObjects();

        if (scene.name == "Level1")
        {
            ReBindScene1Objects();
            CheckFirstMountainDialogue();
        }
        else if (scene.name == "Level2")
        {
            CheckFourthMountainDialogue();
            if (!isScene2DialogInitialized)
            {
                StartCoroutine(DelayedInstantiateDialogForScene2());
            }
        }
        // 新增Ending场景检测
        else if (scene.name == "Ending")
        {
            // 播放对话5（如果未播放过）
            if (!dialogue5Played)
            {
                PlayDialogue(5);
                dialogue5Played = true;
                SaveDialogueStates();
                Debug.Log("在Ending场景播放对话5");
            }
        }

        UpdateDisplayController();
        Debug.Log($"数据状态 - 生产力: {Productivity}, 工具: {Tool_Count}, 人口: {EdenPeople_Count}");
    }

    private IEnumerator DelayedInstantiateDialogForScene2()
    {
        yield return null;

        Instantiate(DialogManagerPrefab2);
        isScene2DialogInitialized = true;
        Debug.Log("场景2对话系统初始化完成");
    }

    // 修复：检查第一座山是否被挖掉并播放对话3
    private void CheckFirstMountainDialogue()
    {
        if (GameDataManager.Instance != null &&
            GameDataManager.Instance.CurrentMountainLevel == 1 &&
            GameDataManager.Instance.IsMountainDestroyed &&
            !dialogue3Played)
        {
            PlayDialogue(3);
            dialogue3Played = true;
            SaveDialogueStates();
            Debug.Log("触发第一座山挖完对话");
        }
    }

    // 检查切换到场景2时是否是第四座山并播放对话4
    private void CheckFourthMountainDialogue()
    {
        if (GameDataManager.Instance != null &&
            GameDataManager.Instance.CurrentMountainLevel == 4 &&
            !dialogue4Played)
        {
            PlayDialogue(4);
            dialogue4Played = true;
            SaveDialogueStates();
            Debug.Log("触发第四座山对话");
        }
    }

    // 播放对话的公共方法（供MountainHealthController调用）
    public void PlayMountainDestroyedDialogue(int mountainLevel)
    {
        if (mountainLevel == 1 && !dialogue3Played)
        {
            PlayDialogue(3);
            dialogue3Played = true;
            SaveDialogueStates();
            Debug.Log("MountainHealthController触发第一座山挖完对话");
        }
        else if (mountainLevel == 5 && !dialogue5Played)
        {
            PlayDialogue(5);
            dialogue5Played = true;
            SaveDialogueStates();
            Debug.Log("触发第五座山挖完对话");
        }
    }

    // 修复：播放对话的具体实现 - 实例化对应的预制体
    private void PlayDialogue(int dialogueIndex)
    {
        Debug.Log($"开始播放对话{dialogueIndex}");

        // 根据对话索引实例化对应的预制体
        GameObject dialogPrefab = GetDialogPrefabByIndex(dialogueIndex);
        if (dialogPrefab != null)
        {
            Instantiate(dialogPrefab);
            Debug.Log($"已实例化对话{dialogueIndex}的预制体");
        }
        else
        {
            Debug.LogError($"未找到对话{dialogueIndex}对应的预制体");
            // 备用方案：显示调试信息
            StartCoroutine(ShowDialogueMessage($"播放对话{dialogueIndex}"));
        }
    }

    // 根据对话索引获取对应的预制体
    private GameObject GetDialogPrefabByIndex(int dialogueIndex)
    {
        switch (dialogueIndex)
        {
            case 1:
                return DialogManagerPrefab1;
            case 2:
                return DialogManagerPrefab2;
            case 3:
                return DialogManagerPrefab3; // 第一座山挖完的对话
            case 4:
                return DialogManagerPrefab4; // 第四座山的对话
            case 5:
                return DialogManagerPrefab5; // 第五座山挖完的对话
            default:
                Debug.LogWarning($"未知的对话索引: {dialogueIndex}，使用默认预制体1");
                return DialogManagerPrefab1;
        }
    }

    private IEnumerator ShowDialogueMessage(string message)
    {
        // 创建临时UI显示对话信息（仅用于调试）
        GameObject dialogMsg = new GameObject("DialogueMsg");
        Canvas canvas = dialogMsg.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        dialogMsg.AddComponent<GraphicRaycaster>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(dialogMsg.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = message;
        text.color = Color.yellow;
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.8f);
        rect.anchorMax = new Vector2(0.5f, 0.8f);
        rect.sizeDelta = new Vector2(400, 60);
        rect.anchoredPosition = Vector2.zero;

        // 添加背景
        GameObject bgObj = new GameObject("Background");
        bgObj.transform.SetParent(textObj.transform);
        Image bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        RectTransform bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = new Vector2(20, 10);
        bgRect.anchoredPosition = Vector2.zero;

        yield return new WaitForSeconds(3f);
        Destroy(dialogMsg);
    }

    private void FindSceneObjects()
    {
        Village = GameObject.Find("村落") ?? GameObject.Find("Village");
        Workshop = GameObject.Find("生产间") ?? GameObject.Find("Workshop");
        Eden = GameObject.Find("伊甸园") ?? GameObject.Find("Eden");
        Mountain1 = GameObject.Find("山1")?.GetComponent<SpriteRenderer>();
        Mountain2 = GameObject.Find("山2")?.GetComponent<SpriteRenderer>();
        Debug.Log($"重新查找对象 - 村落: {Village != null}, 生产间: {Workshop != null}, 伊甸园: {Eden != null}, 山1: {Mountain1 != null}, 山2: {Mountain2 != null}");
    }

    private void ReBindScene1Objects()
    {
        Village = GameObject.Find("村落") ?? GameObject.Find("Village");
        Workshop = GameObject.Find("生产间") ?? GameObject.Find("Workshop");
        Eden = GameObject.Find("伊甸园") ?? GameObject.Find("Eden");
        Mountain1 = GameObject.Find("山1")?.GetComponent<SpriteRenderer>();
        Mountain2 = GameObject.Find("山2")?.GetComponent<SpriteRenderer>();
    }

    private void UpdateDisplayController()
    {
        DisplayController displayController = FindObjectOfType<DisplayController>();
        if (displayController != null)
        {
            displayController.ForceUpdateDisplay();
        }
        else
        {
            Debug.LogWarning("未找到DisplayController，将自动创建");
            CreateDisplayController();
        }
    }

    private void CreateDisplayController()
    {
        GameObject displayObj = new GameObject("DisplayController");
        displayObj.AddComponent<DisplayController>();
        Debug.Log("已自动创建DisplayController");
    }

    private void EnsureGameDataManager()
    {
        var manager = GameDataManager.Instance;
    }

    void Update()
    {
        ClickVillage();
        ClickWorkshop();
        ClickEden();
        EdenPeople_SelfAdd();
        if (!IsClickingBuildings())
        {
            CheckMountainClicks();
        }
        if (Time.time % 60f < Time.deltaTime)
        {
            GameDataManager.Instance.SaveGameData();
        }
    }

    private bool IsClickingBuildings()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Village != null && IsMouseClickInBounds(Village)) return true;
            if (Workshop != null && IsMouseClickInBounds(Workshop)) return true;
            if (Eden != null && IsMouseClickInBounds(Eden)) return true;
        }
        return false;
    }

    private void CheckMountainClicks()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time - lastClickTime < CLICK_COOLDOWN) return;

            // 添加：如果山有生命值系统，则让生命值系统处理点击
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                MountainHealthController healthController =
                    hit.collider.GetComponent<MountainHealthController>();
                if (healthController != null) return; // 让生命值系统处理
            }

            if (Time.time - lastClickTime < CLICK_COOLDOWN) return;
            lastClickTime = Time.time;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (IsClickingBuildings()) return;
            if (Mountain1 != null && IsPointInSprite(mousePosition, Mountain1))
            {
                SwitchToLevel2();
                return;
            }
            if (Mountain2 != null && IsPointInSprite(mousePosition, Mountain2))
            {
                SwitchToLevel2();
                return;
            }
        }
    }

    private bool IsPointInSprite(Vector2 point, SpriteRenderer spriteRenderer)
    {
        if (spriteRenderer == null) return false;

        // 使用射线检测
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        // 检查是否击中了目标精灵渲染器所在的游戏对象
        if (hit.collider != null && hit.collider.gameObject == spriteRenderer.gameObject)
        {
            return true;
        }

        return false;
    }

    public void SwitchToLevel2()
    {
        Debug.Log("开始切换到第二关...");
        GameDataManager.Instance.SaveGameData();

        // 检查山体状态
        if (GameDataManager.Instance.IsMountainDestroyed &&
            GameDataManager.Instance.CanSpawnNewMountain())
        {
            Debug.Log("生成新山体...");
            GameDataManager.Instance.InitializeNewMountain();
        }

        if (Application.CanStreamedLevelBeLoaded("Level2"))
        {
            SceneManager.LoadScene("Level2");
        }
        else
        {
            Debug.LogError("Level2场景不存在！");
            StartCoroutine(ShowErrorCoroutine("Level2场景不存在！\n请检查Build Settings"));
        }
    }

    private IEnumerator ShowErrorCoroutine(string message)
    {
        GameObject errorMsg = new GameObject("ErrorMsg");
        Canvas canvas = errorMsg.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        errorMsg.AddComponent<GraphicRaycaster>();
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(errorMsg.transform);
        Text text = textObj.AddComponent<Text>();
        text.text = message;
        text.color = Color.red;
        text.fontSize = 20;
        text.alignment = TextAnchor.MiddleCenter;
        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 100);
        rect.anchoredPosition = Vector2.zero;
        yield return new WaitForSeconds(3f);
        Destroy(errorMsg);
    }

    public void ManualSave()
    {
        GameDataManager.Instance.SaveGameData();
        ShowMessage("游戏进度已保存！");
        UpdateDisplayController();
    }

    public void ManualLoad()
    {
        GameDataManager.Instance.LoadGameData();
        ShowMessage("游戏进度已加载！");
        UpdateDisplayController();
    }

    public void ResetGameData()
    {
        GameDataManager.Instance.ResetGameData();
        ShowMessage("游戏数据已重置！");
        UpdateDisplayController();
        // 重置游戏数据时也重置对话状态
        ResetDialogueStates();
    }

    public void ShowResetConfirmation()
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorUtility.DisplayDialog("确认重置", "确定要重置所有游戏数据吗？", "确定", "取消"))
        {
            ResetGameData();
        }
#else
        ResetGameData();
#endif
    }

    public void ExportGameData()
    {
        try
        {
            string exportData = $"生产力: {Productivity}\n工具数量: {Tool_Count}\n人口: {EdenPeople_Count}\n导出时间: {System.DateTime.Now}";
            GUIUtility.systemCopyBuffer = exportData;
            ShowMessage("游戏数据已复制到剪贴板！");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"导出数据失败: {e.Message}");
        }
    }

    public static void StaticShowResetConfirmation()
    {
        click instance = FindObjectOfType<click>();
        if (instance != null)
        {
            instance.ShowResetConfirmation();
        }
        else
        {
            Debug.LogError("未找到click实例！");
#if UNITY_EDITOR
            if (UnityEditor.EditorUtility.DisplayDialog("确认重置", "确定要重置所有游戏数据吗？", "确定", "取消"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
            }
#endif
        }
    }

    private void ShowMessage(string message)
    {
        Debug.Log(message);
        StartCoroutine(ShowTempMessage(message));
    }

    private IEnumerator ShowTempMessage(string message)
    {
        yield return new WaitForSeconds(2f);
    }

    private void EdenPeople_SelfAdd()
    {
        Time_Eden += Time.deltaTime;
        if (EdenPeople_Count > new StringBigInt("0") && Time_Eden >= 5)
        {
            GameDataManager.Instance.Productivity = GameDataManager.Instance.Productivity + EdenPeople_Count;
            Time_Eden = 0;
        }
    }

    private void ClickEden()
    {
        if (IsGameActive() && Input.GetMouseButtonDown(0) && IsMouseClickInBounds(Eden) && Productivity >= Consume_Eden)
        {
            GameDataManager.Instance.EdenPeople_Count = EdenPeople_Count + One;
            GameDataManager.Instance.Productivity = Productivity - Consume_Eden;
        }
    }

    private void ClickWorkshop()
    {
        if (IsGameActive() && Input.GetMouseButtonDown(0) && IsMouseClickInBounds(Workshop) && Productivity >= Consume_Workshop)
        {
            GameDataManager.Instance.Tool_Count = Tool_Count + One;
            GameDataManager.Instance.Productivity = Productivity - Consume_Workshop;
        }
    }

    private void ClickVillage()
    {
        if (IsGameActive() && Input.GetMouseButtonDown(0) && IsMouseClickInBounds(Village))
        {
            GameDataManager.Instance.Productivity = Productivity + One;
        }
    }

    private bool IsGameActive()
    {
        SettingsController settingsController = FindObjectOfType<SettingsController>();
        return settingsController == null || SettingsController.isGameActive;
    }

    public bool IsMouseClickInBounds(GameObject targetObject)
    {
        if (Input.GetMouseButtonDown(0) && targetObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == targetObject)
            {
                return true;
            }
        }
        return false;
    }

    void OnApplicationQuit()
    {
        GameDataManager.Instance.SaveGameData();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            GameDataManager.Instance.SaveGameData();
        }
    }
}

public class StringBigInt
{
    private string value;

    public StringBigInt(string numStr)
    {
        if (!IsValidNumber(numStr))
            throw new ArgumentException("无效的数字格式");

        this.value = numStr.TrimStart('0');
        if (string.IsNullOrEmpty(this.value))
            this.value = "0";
    }

    private bool IsValidNumber(string numStr)
    {
        if (string.IsNullOrEmpty(numStr))
            return false;

        foreach (char c in numStr)
        {
            if (c < '0' || c > '9')
                return false;
        }
        return true;
    }

    public static StringBigInt operator +(StringBigInt a, StringBigInt b)
    {
        StringBuilder result = new StringBuilder();
        int carry = 0;
        int i = a.value.Length - 1;
        int j = b.value.Length - 1;

        while (i >= 0 || j >= 0 || carry > 0)
        {
            int sum = carry;
            if (i >= 0) sum += a.value[i--] - '0';
            if (j >= 0) sum += b.value[j--] - '0';

            result.Insert(0, (sum % 10).ToString());
            carry = sum / 10;
        }

        return new StringBigInt(result.ToString());
    }

    public static StringBigInt operator -(StringBigInt a, StringBigInt b)
    {
        if (a < b)
            return new StringBigInt("0");

        StringBuilder result = new StringBuilder();
        int borrow = 0;
        int i = a.value.Length - 1;
        int j = b.value.Length - 1;

        while (i >= 0)
        {
            int digitA = a.value[i--] - '0' - borrow;
            int digitB = j >= 0 ? b.value[j--] - '0' : 0;

            if (digitA < digitB)
            {
                digitA += 10;
                borrow = 1;
            }
            else
            {
                borrow = 0;
            }

            result.Insert(0, (digitA - digitB).ToString());
        }

        string resultStr = result.ToString().TrimStart('0');
        return new StringBigInt(string.IsNullOrEmpty(resultStr) ? "0" : resultStr);
    }

    public static StringBigInt operator *(StringBigInt a, StringBigInt b)
    {
        if (a.value == "0" || b.value == "0")
            return new StringBigInt("0");

        int[] resultArray = new int[a.value.Length + b.value.Length];

        for (int i = a.value.Length - 1; i >= 0; i--)
        {
            for (int j = b.value.Length - 1; j >= 0; j--)
            {
                int product = (a.value[i] - '0') * (b.value[j] - '0');
                int sum = product + resultArray[i + j + 1];

                resultArray[i + j + 1] = sum % 10;
                resultArray[i + j] += sum / 10;
            }
        }

        StringBuilder sb = new StringBuilder();
        bool leadingZero = true;
        for (int i = 0; i < resultArray.Length; i++)
        {
            if (leadingZero && resultArray[i] == 0)
                continue;
            leadingZero = false;
            sb.Append(resultArray[i]);
        }

        return new StringBigInt(sb.Length == 0 ? "0" : sb.ToString());
    }

    public static bool operator >(StringBigInt a, StringBigInt b)
    {
        if (a.value.Length != b.value.Length)
            return a.value.Length > b.value.Length;

        return string.Compare(a.value, b.value) > 0;
    }

    public static bool operator <(StringBigInt a, StringBigInt b)
    {
        if (a.value.Length != b.value.Length)
            return a.value.Length < b.value.Length;

        return string.Compare(a.value, b.value) < 0;
    }

    public static bool operator >=(StringBigInt a, StringBigInt b)
    {
        return a > b || a.value == b.value;
    }

    public static bool operator <=(StringBigInt a, StringBigInt b)
    {
        return a < b || a.value == b.value;
    }

    public override bool Equals(object obj)
    {
        if (obj is StringBigInt other)
            return this.value == other.value;
        return false;
    }

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public override string ToString()
    {
        return value;
    }

    public string ToFormattedString()
    {
        if (value == "0") return "0";

        StringBuilder sb = new StringBuilder();
        int count = 0;

        for (int i = value.Length - 1; i >= 0; i--)
        {
            if (count > 0 && count % 3 == 0)
                sb.Insert(0, ',');
            sb.Insert(0, value[i]);
            count++;
        }

        return sb.ToString();
    }
}