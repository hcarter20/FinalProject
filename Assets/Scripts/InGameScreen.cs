using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameScreen : MonoBehaviour
{
    // Canvas Group component used to set alpha/interactable for children buttons
    private CanvasGroup canvasGroup;
    // Animator component which animates the canvas group fade anim
    private Animator animator;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
        if (canvasGroup == null || animator == null)
            Debug.LogError("Screen is missing component (canvas group or animator)");
    }

    public void OnEnable()
    {
        // Animate the canvas group to fade in
        canvasGroup.interactable = true;
        animator.SetBool("IsVisible", true);
        canvasGroup.alpha = 1.0f;
    }

    public void HideScreen()
    {
        // Animate the canvas group to fade out
        animator.SetBool("IsVisible", false);
        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
    }
}
