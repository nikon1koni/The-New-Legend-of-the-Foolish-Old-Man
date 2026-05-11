using UnityEngine;

public class BGMManager : MonoBehaviour
{
    // 单例实例
    public static BGMManager Instance { get; private set; }

    // BGM音频剪辑
    public AudioClip bgmClip;

    // 音频源组件
    private AudioSource audioSource;

    private void Awake()
    {
        // 确保全局唯一且不随场景销毁
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 关键：场景切换时不销毁
            InitializeAudioSource();
        }
        else
        {
            // 如果已有实例，销毁重复对象
            Destroy(gameObject);
        }
    }

    // 初始化音频源
    private void InitializeAudioSource()
    {
        // 添加音频源组件
        audioSource = gameObject.AddComponent<AudioSource>();

        // 配置音频属性
        audioSource.clip = bgmClip;
        audioSource.loop = true; // 循环播放
        audioSource.volume = 0.5f; // 音量（可根据需要调整）
        audioSource.playOnAwake = true; // 唤醒时自动播放

        // 开始播放
        if (bgmClip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError("请在Inspector中赋值BGM音频文件");
        }
    }

    // 外部控制方法（可选）
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
        }
    }

    public void Pause()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void Resume()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}