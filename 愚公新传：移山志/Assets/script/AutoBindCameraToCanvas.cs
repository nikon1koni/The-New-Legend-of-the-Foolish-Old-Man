using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AutoBindCameraToCanvas : MonoBehaviour
{
    void Start()
    {
        // 延迟1帧执行，确保场景中摄像机已加载
        Invoke(nameof(BindCamera), 0.01f);
    }

    void BindCamera()
    {
        Canvas canvas = GetComponent<Canvas>();
        Camera mainCam = Camera.main;

        if (mainCam != null)
        {
            // 强制将Canvas渲染模式设为“屏幕空间-摄像机”，再绑定
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = mainCam;
            Debug.Log("预制体Canvas已成功绑定到Main Camera");
        }
        else
        {
            Debug.LogError("场景中未找到标签为MainCamera的摄像机！");
        }
    }
}