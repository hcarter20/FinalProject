using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyController : MonoBehaviour
{
    // Singleton definition
    public static SkyController S;

    // The Y-positions we scroll between
    public float startHeight, midHeight, endHeight;

    // The component on this game object
    private RectTransform rect;

    // The current coroutine scrolling the sky
    private Coroutine skyCoroutine;

    private void Awake()
    {
        if (S == null)
            S = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void StartStacking(float stackTime)
    {
        skyCoroutine = StartCoroutine(Scroll(startHeight, midHeight, stackTime));
    }

    public void StartSleeping(float sleepTime)
    {
        skyCoroutine = StartCoroutine(Scroll(midHeight, endHeight, sleepTime));
    }

    public void FinishStacking()
    {
        // Instantly skip to the end of the stacking phase
        if (skyCoroutine != null)
            StopCoroutine(skyCoroutine);

        // Set the sky to the nighttime position
        rect.anchoredPosition3D = new Vector3(0.0f, midHeight, 0.0f);
    }

    private IEnumerator Scroll(float initialHeight, float goalHeight, float totalTime)
    {
        // Make sure sky starts at the initial height
        rect.anchoredPosition3D = new Vector3(0.0f, initialHeight, 0.0f);
        
        float timer = 0.0f;
        while (timer < totalTime)
        {
            // Wait a little bit (using physics time)
            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;

            // Move the sky correspondingly
            float lerped = Mathf.LerpUnclamped(initialHeight, goalHeight, timer / totalTime);
            rect.anchoredPosition3D = new Vector3(0.0f, lerped, 0.0f);
        }

        // Make sure sky ends at the goal height
        rect.anchoredPosition3D = new Vector3(0.0f, goalHeight, 0.0f);
    }
}
