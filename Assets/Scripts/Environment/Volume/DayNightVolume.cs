using System.Collections;
using System.Collections.Generic;
using HauntedPSX.RenderPipelines.PSX.Runtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Controls the transition between day and night using volumetric effects and sky rotation.
/// </summary>
public class DayNightVolume : MonoBehaviour
{
    public Volume dayVolume;                // Volume profile for daytime
    public Volume nightVolume;              // Volume profile for nighttime
    [Space]
    public float transitionDuration = 5f;  // Duration of day-night transition
    public float skySpeed = 3f;             // Speed of sky rotation

    private SkyVolume currentSky;           // Currently active sky volume
    private Vector3 currentRotation = Vector3.zero;  // Current rotation of the sky

    private Coroutine dayRoutine;           // Coroutine handling the transition

    /// <summary>
    /// Initialize by registering with GameManager and setting daytime active.
    /// </summary>
    private void Start()
    {
        GameManager.Instance.dayNightVolume = this;
        SetDayTime(true);
    }

    /// <summary>
    /// Rotate the current sky every frame to simulate sky movement.
    /// </summary>
    private void Update()
    {
        if (currentSky != null)
        {
            currentRotation += Vector3.up * skySpeed * Time.deltaTime;
            currentSky.skyRotation.overrideState = true;
            currentSky.skyRotation.value = currentRotation;
        }
    }

    /// <summary>
    /// Starts transitioning to day or night volumes.
    /// </summary>
    /// <param name="isDay">True for day, false for night.</param>
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

    /// <summary>
    /// Coroutine that smoothly blends between day and night volumes over transitionDuration.
    /// </summary>
    /// <param name="isDay">True if transitioning to day, false for night.</param>
    /// <returns>IEnumerator for coroutine.</returns>
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
