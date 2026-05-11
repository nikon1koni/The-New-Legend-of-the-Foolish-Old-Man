using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public GameObject settingsPanel; // 拖入 SettingsPanel
    public SpriteRenderer settingsSprite; // 拖入 SettingsSprite
    public Button backButton; // 拖入 BackButton

    public static bool isGameActive = true;
    void Start()
    {
        // 初始隐藏设置面板
        settingsPanel.SetActive(false);

        // 绑定按钮事件
        backButton.onClick.AddListener(CloseSettings);
    }

    void Update()
    {
        // 检测鼠标点击精灵
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null && hit.collider.gameObject == settingsSprite.gameObject)
            {
                OpenSettings();
            }
        }
    }

    void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f; // 暂停游戏
        isGameActive = false;
    }

    void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏
        isGameActive = true;
    }

    void TogglePause()
    {
        Time.timeScale = (Time.timeScale == 0f) ? 1f : 0f;
        Debug.Log("游戏状态: " + (Time.timeScale == 0f ? "暂停" : "继续"));
    }
}