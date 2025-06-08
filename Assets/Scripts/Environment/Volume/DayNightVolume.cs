using System.Collections;
using System.Collections.Generic;
using HauntedPSX.RenderPipelines.PSX.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class DayNightVolume : MonoBehaviour
{
    public Volume dayVolume;
    public Volume nightVolume;
    [Space]
    public float transitionDuration = 5f;
    public float skySpeed = 3f;

    private SkyVolume currentSky;
    private Vector3 currentRotation = Vector3.zero;

    private Coroutine dayRoutine;

    private void Start()
    {
        GameManager.Instance.dayNightVolume = this;

        SetDayTime(true);
    }

    private void Update()
    {
        if (currentSky != null)
        {
            currentRotation += Vector3.up * skySpeed * Time.deltaTime;

            currentSky.skyRotation.overrideState = true;
            currentSky.skyRotation.value = currentRotation;
        }
    }

    public void SetDayTime(bool isDay = true)
    {
        if (dayRoutine != null)
        {
            StopCoroutine(dayRoutine);
        }

        if (isDay)
        {
            dayVolume.profile.TryGet<SkyVolume>(out currentSky);
        }
        else
        {
            nightVolume.profile.TryGet<SkyVolume>(out currentSky);
        }

        if (currentSky != null) currentSky.active = false;

        dayRoutine = StartCoroutine(DayNightToggle(isDay));
    }

    private IEnumerator DayNightToggle(bool isDay)
    {
        float targetDayWeight = isDay ? 1f : 0f;
        float targetNightWeight = isDay ? 0f : 1f;

        while (!Mathf.Approximately(dayVolume.weight, targetDayWeight) ||
               !Mathf.Approximately(nightVolume.weight, targetNightWeight))
        {
            dayVolume.weight = Mathf.MoveTowards(dayVolume.weight, targetDayWeight, Time.deltaTime / transitionDuration);
            nightVolume.weight = Mathf.MoveTowards(nightVolume.weight, targetNightWeight, Time.deltaTime / transitionDuration);

            yield return null;
        }

        dayVolume.weight = targetDayWeight;
        nightVolume.weight = targetNightWeight;

        if (currentSky != null) currentSky.active = true;
    }
}