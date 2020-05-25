using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour
{
    // Start is called before the first frame update
    public Slider TimeSlider;
    void Start()
    {
        TimeSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = TimeSlider.value;
    }
}
