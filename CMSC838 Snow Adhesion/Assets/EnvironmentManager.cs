using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public float ambientTemperature = 2.0f; // ambient air temperature (Celsius)
    public float currentSnowfallRateSWE = 10.0f; // snowfall accumulation rate (mm/hour); 

    // You could add other environmental factors here:
    // public float windSpeed = 0.0f;
    // public float solarRadiation = 0.0f; // Needed for a more accurate energy balance melt model

    // Example: Function to simulate changing temperature over time
    public void SetAmbientTemperature(float temperature)
    {
        ambientTemperature = temperature;
    }

    // Example: Function to start/stop snowfall
    public void SetSnowfallRate(float sweRate_m_per_sec)
    {
        currentSnowfallRateSWE = sweRate_m_per_sec;
    }
}

