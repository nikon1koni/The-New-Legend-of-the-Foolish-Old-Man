/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    private Animator animator;
    public string animationTrigger = "Play"; // 在Animator中设置的Trigger名称

    void Start()
    {
        animator = GetComponent<Animator>();

        // 可选：确保动画在开始时处于静止
        if (animator != null)
        {
            // 设置初始状态
            animator.Play("Idle", 0, 0f);
        }
    }

    void OnMouseDown()
    {
        if (animator != null)
        {
            animator.SetTrigger(animationTrigger);
        }
    }
}*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    private Animator animator;
    public string animationTrigger = "Play"; // 对应Animator中设置的Trigger名称

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            // 1. 重置Trigger（避免残留状态导致意外触发）
            animator.ResetTrigger(animationTrigger);
            // 2. 强制将动画停在「初始状态」（如Idle，需在Animator中存在该状态且Entry指向它）
            animator.Play("Idle", 0, 0f);
        }
    }

    void OnMouseDown()
    {
        if (animator != null && gameObject.activeInHierarchy)
        {
            // 1. 再次重置Trigger（防止连续点击时的状态残留）
            animator.ResetTrigger(animationTrigger);
            // 2. 触发Transition进入Scale状态
            animator.SetTrigger(animationTrigger);
        }
    }
}