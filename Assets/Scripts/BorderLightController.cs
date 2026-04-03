using System;
using System.Collections;
using UnityEngine;

public class BorderLightController : MonoBehaviour
{
    [SerializeField] private Animator[] borderAnimators;
    [SerializeField] private float startInterval = 0.05f;
    [SerializeField] private float slowdownStep = 0.01f;
    [SerializeField] private float maxInterval = 0.12f;

    private int currentIndex = 0;
    private Coroutine lightRoutine;

    public void StartLight()
    {
        if (lightRoutine != null) StopCoroutine(lightRoutine);
        lightRoutine = StartCoroutine(RunLights());
    }

    public void StopLightAt(int targetIndex)
    {
        if (lightRoutine != null) StopCoroutine(lightRoutine);
        lightRoutine = StartCoroutine(SlowDownLights(targetIndex));
    }

    IEnumerator RunLights()
    {
        float currentInterval = startInterval;
        while (true)
        {
            PlayFlash(currentIndex);
            currentIndex = (currentIndex + 1) % borderAnimators.Length;
            yield return new WaitForSeconds(currentInterval);
        }
    }

    IEnumerator SlowDownLights(int targetIndex)
    {
        float currentInterval = startInterval;

        while (true)
        {
            PlayFlash(currentIndex);

            if (currentIndex == targetIndex && currentInterval >= maxInterval)
                break;

            currentIndex = (currentIndex + 1) % borderAnimators.Length;
            currentInterval = Mathf.Min(currentInterval + slowdownStep, maxInterval);

            yield return new WaitForSeconds(currentInterval);
        }

        PlayFlash(targetIndex);
        borderAnimators[targetIndex].SetTrigger("Stop");
    }

    void PlayFlash(int index)
    {
        borderAnimators[index].SetTrigger("Flash");
    }
}
