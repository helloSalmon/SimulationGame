using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour
{
    Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = 0;
    }

    void Update()
    {
        float currentValue = slider.value;
        float targetValue = slider.maxValue;
        float? gameEndTime = Managers.Time.GetGameEndTime();

        if (currentValue < targetValue)
        {
            if (gameEndTime.HasValue)
            {
                currentValue += Time.deltaTime / gameEndTime.Value;
            }
            currentValue = Mathf.Clamp(currentValue, slider.minValue, targetValue);
            slider.value = currentValue;
        }
    }
}
