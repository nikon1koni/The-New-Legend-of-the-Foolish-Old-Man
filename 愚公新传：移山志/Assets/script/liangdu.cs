using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    [Header("滑动条引用")]
    public Slider brightnessSlider;

    [Header("亮度范围")]
    [Range(0f, 1f)] public float minBrightness = 0.2f;
    [Range(0f, 1f)] public float maxBrightness = 1f;

    // 可选：调节光源强度（通用方法）
    public Light mainLight;

    private Camera mainCamera;
    private Color originalBgColor; // 原始背景色

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalBgColor = mainCamera.backgroundColor; // 保存原始背景色
        }

        // 初始化滑动条
        brightnessSlider.minValue = 0;
        brightnessSlider.maxValue = 1;
        brightnessSlider.value = 0.5f;

        // 初始亮度设置
        UpdateBrightness(brightnessSlider.value);

        // 绑定滑动事件
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
    }

    // 亮度更新方法（适配所有渲染管线）
    void UpdateBrightness(float value)
    {
        // 方法1：调节相机背景亮度（内置管线适用）
        if (mainCamera != null)
        {
            float brightness = Mathf.Lerp(minBrightness, maxBrightness, value);
            mainCamera.backgroundColor = new Color(
                originalBgColor.r * brightness,
                originalBgColor.g * brightness,
                originalBgColor.b * brightness
            );
        }

        // 方法2：调节光源强度（通用方法，所有管线都支持）
        if (mainLight != null)
        {
            // 光源强度范围可以根据需要调整
            mainLight.intensity = Mathf.Lerp(minBrightness * 10, maxBrightness * 10, value);
        }
    }
}