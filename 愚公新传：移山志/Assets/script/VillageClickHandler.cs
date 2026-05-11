using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VillageClickHandler : MonoBehaviour
{
    [Header("调试设置")]
    public bool enableDebug = true;

    private void OnMouseDown()
    {
        if (SettingsController.isGameActive)
        {
            if (enableDebug) Debug.Log("检测到村落点击事件");
            HandleVillageClick();
        }
        else
        {
            if (enableDebug) Debug.Log("游戏处于非活动状态，点击被忽略");
        }
    }

    private void HandleVillageClick()
    {
        SwitchToLevel1Direct();

    }

    private void SwitchToLevel1Direct()
    {
        if (enableDebug) Debug.Log("开始同步切换到Level1...");

        // 保存游戏数据（确保数值同步）
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.SaveGameData();
            if (enableDebug) Debug.Log("游戏数据已保存");
        }

        // 直接加载场景
        SceneManager.LoadScene("level1");
        if (enableDebug) Debug.Log("Level1场景加载完成");
    }
}