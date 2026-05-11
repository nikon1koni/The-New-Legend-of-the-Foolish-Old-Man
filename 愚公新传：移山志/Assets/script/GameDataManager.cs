using UnityEngine;
using System;

public class GameDataManager : MonoBehaviour
{
    // 单例实例
    public static GameDataManager Instance { get; private set; }

    // 游戏数据
    public StringBigInt Productivity { get; set; }
    public StringBigInt Tool_Count { get; set; }
    public StringBigInt EdenPeople_Count { get; set; }

    // 山体系统数据（新增）
    public int MountainCurrentHealth { get; set; }
    public int MountainMaxHealth { get; set; }
    public int CurrentMountainLevel { get; set; } // 当前是第几座山 (1-5)
    public bool IsMountainDestroyed { get; set; } // 当前山是否已被摧毁

    // 山体难度配置（可调整）
    public static readonly int[] MountainHealthLevels = { 10, 1, 1, 1, 1 };
    public const int MAX_MOUNTAIN_LEVEL = 5;

    // 常量
    public static readonly StringBigInt Consume_Workshop = new StringBigInt("10");
    public static readonly StringBigInt Consume_Eden = new StringBigInt("15");
    public static readonly StringBigInt One = new StringBigInt("1");

    void Awake()
    {
        // 实现单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
            Debug.Log("GameDataManager初始化完成");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 初始化新山体
    public void InitializeNewMountain()
    {
        if (CurrentMountainLevel >= MAX_MOUNTAIN_LEVEL)
        {
            Debug.Log("已达到最大山体等级，不再生成新山体");
            IsMountainDestroyed = true;
            MountainCurrentHealth = 0;
            MountainMaxHealth = 0;
            return;
        }

        // 如果是第一次或者山被摧毁后，升级到下一座山
        if (CurrentMountainLevel == 0 || IsMountainDestroyed)
        {
            CurrentMountainLevel++;
            if (CurrentMountainLevel > MAX_MOUNTAIN_LEVEL)
            {
                CurrentMountainLevel = MAX_MOUNTAIN_LEVEL;
            }
        }

        // 设置新山体的生命值
        MountainMaxHealth = MountainHealthLevels[CurrentMountainLevel - 1]; // 数组索引从0开始
        MountainCurrentHealth = MountainMaxHealth;
        IsMountainDestroyed = false;

        Debug.Log($"初始化第{CurrentMountainLevel}座山，生命值: {MountainCurrentHealth}/{MountainMaxHealth}");
    }

    // 保存游戏数据
    public void SaveGameData()
    {
        try
        {
            // 保存基础数据
            PlayerPrefs.SetString("Productivity", Productivity.ToString());
            PlayerPrefs.SetString("Tool_Count", Tool_Count.ToString());
            PlayerPrefs.SetString("EdenPeople_Count", EdenPeople_Count.ToString());

            // 保存山体系统数据
            PlayerPrefs.SetInt("MountainCurrentHealth", MountainCurrentHealth);
            PlayerPrefs.SetInt("MountainMaxHealth", MountainMaxHealth);
            PlayerPrefs.SetInt("CurrentMountainLevel", CurrentMountainLevel);
            PlayerPrefs.SetInt("IsMountainDestroyed", IsMountainDestroyed ? 1 : 0);

            PlayerPrefs.SetString("LastSaveTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            PlayerPrefs.Save();
            Debug.Log("游戏数据已保存（包含山体系统）");
        }
        catch (Exception e)
        {
            Debug.LogError($"保存数据失败: {e.Message}");
        }
    }

    // 加载游戏数据
    public void LoadGameData()
    {
        try
        {
            if (PlayerPrefs.HasKey("Productivity"))
            {
                // 加载基础数据
                Productivity = new StringBigInt(PlayerPrefs.GetString("Productivity"));
                Tool_Count = new StringBigInt(PlayerPrefs.GetString("Tool_Count"));
                EdenPeople_Count = new StringBigInt(PlayerPrefs.GetString("EdenPeople_Count"));

                // 加载山体系统数据
                MountainCurrentHealth = PlayerPrefs.GetInt("MountainCurrentHealth", 0);
                MountainMaxHealth = PlayerPrefs.GetInt("MountainMaxHealth", 0);
                CurrentMountainLevel = PlayerPrefs.GetInt("CurrentMountainLevel", 0);
                IsMountainDestroyed = PlayerPrefs.GetInt("IsMountainDestroyed", 0) == 1;

                // 如果没有山体数据，初始化第一座山
                if (CurrentMountainLevel == 0 && MountainMaxHealth == 0)
                {
                    InitializeNewMountain();
                }

                Debug.Log($"游戏数据加载成功 - 第{CurrentMountainLevel}座山，生命值: {MountainCurrentHealth}/{MountainMaxHealth}");
            }
            else
            {
                // 新游戏初始化
                Productivity = new StringBigInt("0");
                Tool_Count = new StringBigInt("0");
                EdenPeople_Count = new StringBigInt("0");
                InitializeNewMountain(); // 初始化第一座山
                Debug.Log("新游戏初始化完成");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载数据失败: {e.Message}");
            // 容错默认值
            Productivity = new StringBigInt("0");
            Tool_Count = new StringBigInt("0");
            EdenPeople_Count = new StringBigInt("0");
            InitializeNewMountain();
        }
    }

    // 重置游戏数据
    public void ResetGameData()
    {
        Productivity = new StringBigInt("0");
        Tool_Count = new StringBigInt("0");
        EdenPeople_Count = new StringBigInt("0");
        CurrentMountainLevel = 0;
        InitializeNewMountain(); // 重置后初始化第一座山
        SaveGameData();
    }

    // 检查是否可以生成新山体
    public bool CanSpawnNewMountain()
    {
        return CurrentMountainLevel < MAX_MOUNTAIN_LEVEL && IsMountainDestroyed;
    }
}