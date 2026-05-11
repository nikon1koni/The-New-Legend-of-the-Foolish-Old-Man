using UnityEngine;
using UnityEngine.UI;
using System;

public class SettingsUIController : MonoBehaviour
{
    [Header("UI组件引用")]
    public GameObject settingsPanel;
    public Button saveButton;
    public Button loadButton;
    public Button resetButton;
    public Button exportButton;
    public Button closeSettingsButton;
    public Text saveInfoText;

    [Header("设置按钮")]
    public GameObject settingsButton;

    private click mainGameController;

    void Start()
    {
        //获取主游戏控制器
        mainGameController = FindObjectOfType<click>();

        //设置按钮事件
        SetupUIEvents();

        //初始隐藏设置面板
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    void Update()
    {
        //检测设置按钮点击
        CheckSettingsButtonClick();
    }

    //设置UI按钮事件
    private void SetupUIEvents()
    {
        if (saveButton != null) saveButton.onClick.AddListener(ManualSave);
        if (loadButton != null) loadButton.onClick.AddListener(ManualLoad);
        if (resetButton != null) resetButton.onClick.AddListener(ShowResetConfirmation);
        if (exportButton != null) exportButton.onClick.AddListener(ExportGameData);
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(CloseSettings);

        UpdateSaveInfo();
    }

    //检测设置按钮点击
    private void CheckSettingsButtonClick()
    {
        if (Input.GetMouseButtonDown(0) && IsMouseClickInBounds(settingsButton))
        {
            OpenSettings();
        }
    }

    //打开设置面板
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            SettingsController settingsController = FindObjectOfType<SettingsController>();
            if (settingsController != null)
            {
                SettingsController.isGameActive = false;
            }

            //更新存档信息显示
            UpdateSaveInfo();
        }
    }

    //关闭设置面板
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
            SettingsController settingsController = FindObjectOfType<SettingsController>();
            if (settingsController != null)
            {
                SettingsController.isGameActive = true;
                Time.timeScale = 1f;
            }

            // 关闭设置时自动保存
            if (mainGameController != null)
            {
                mainGameController.ManualSave();
            }
        }
    }

    //手动保存
    public void ManualSave()
    {
        if (mainGameController != null)
        {
            mainGameController.ManualSave();
        }
        UpdateSaveInfo();
    }

    //手动加载
    public void ManualLoad()
    {
        if (mainGameController != null)
        {
            mainGameController.ManualLoad();
        }
        UpdateSaveInfo();
    }

    //显示重置确认
    public void ShowResetConfirmation()
    {
        if (mainGameController != null)
        {
            mainGameController.ShowResetConfirmation();
        }
        else
        {
            // 备用方案：直接调用静态方法
            click.StaticShowResetConfirmation();
        }
        UpdateSaveInfo();
    }

    //导出游戏数据
    public void ExportGameData()
    {
        if (mainGameController != null)
        {
            mainGameController.ExportGameData();
        }
    }

    //更新存档信息显示
    public void UpdateSaveInfo()
    {
        if (saveInfoText != null)
        {
            if (PlayerPrefs.HasKey("LastSaveTime"))
            {
                string lastSaveTime = PlayerPrefs.GetString("LastSaveTime");
                saveInfoText.text = $"最后保存: {lastSaveTime}";
            }
            else
            {
                saveInfoText.text = "暂无存档数据";
            }
        }
    }

    //检查鼠标点击是否在对象边界内
    private bool IsMouseClickInBounds(GameObject targetObject)
    {
        if (Input.GetMouseButtonDown(0) && targetObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == targetObject)
                {
                    Bounds bounds = GetObjectBounds(targetObject);
                    return bounds.Contains(hit.point);
                }
            }
        }
        return false;
    }

    //获取对象边界
    private Bounds GetObjectBounds(GameObject targetObject)
    {
        Collider collider = targetObject.GetComponent<Collider>();
        if (collider != null)
        {
            return collider.bounds;
        }
        return new Bounds(targetObject.transform.position, Vector3.zero);
    }
}