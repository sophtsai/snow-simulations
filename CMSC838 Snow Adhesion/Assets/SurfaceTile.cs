using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceType
{
  Concrete, Asphalt, Grass
}

public class SurfaceTile : MonoBehaviour
{
  // Constants
  const float sigma = 5.67e-8f; // W/m^2*K^4
  const float rhoAir = 1.225f; // kg/m^3
  const float cpAir = 1005f; // J/kg*K
  const float Lv = 2.5e6f; // J/kg


  // Inputs
  public float airTemperature; // °C
  public float precipitation; // mm/day
  public float degreeDayFactor = 3; // mm/°C/day
  public float meltTempThreshold = 0.0f; // °C
  public float retentionFraction = 0.05f; // % of SWE retained as snowmelt
  public float snowSurfaceTemperature = 0f; // °C
  public float albedo = 0.85f;
  public float incomingShortwave = 450f; // W/m^2
  public float windSpeed = 2f; // m/s
  public float relativeHumidity = 70f; // %
  public float groundTemperature = -2f; // °C  

  // State variables
  public float SWE = 0f; // mm
  public float snowDepth = 0f; // mm
  public float snowDensity = 300f; // kg/m^3
  const float sigma = 5.67e-8f; // W/m^2*K^4
  const float rhoAir = 1.225f; // kg/m^3
  const float cpAir = 1005f; // J/kg*K
  const float Lv = 2.5e6f; // J/kg
  const float q_a = 0.01f; // kg/kg
  const float q_s = 0.0048f; // kg/kg
  const float Ls = 2.834e6f; // J/kg
  const float Lf = 3.34e5f; // J/kg 
  const float rhoWater = 1000f; // kg/m^3
  const float aerodynamicResistance = 100f; // s/m

  GameObject surfaceViz;
  GameObject depthTextObj;
  TextMesh depthText;

  void Start()
  {
    airTemperature = -2.0f;
    precipitation = 2.0f;

    surfaceViz = GameObject.CreatePrimitive(PrimitiveType.Cube);
    surfaceViz.transform.position = transform.position;
    surfaceViz.transform.localScale = new Vector3(1, 0.0f, 1);

    depthTextObj = new GameObject("SnowDepthText");
    depthTextObj.transform.SetParent(surfaceViz.transform);
    depthTextObj.transform.localPosition = new Vector3(0, 0, 0.5f);
    depthTextObj.transform.localRotation = Quaternion.Euler(90, 0, 0);
    depthTextObj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
    depthText = depthTextObj.AddComponent<TextMesh>();
    depthText.fontSize = 10;
    depthText.color = Color.black;
  }

  void Update()
  {
    SimulateDay(Time.deltaTime); // parameter means 1 day runs every second
  }

  void SimulateDay(float time)
  {
    float P_snow = airTemperature <= 0f ? precipitation : 0f;   
    float Ts_K = snowSurfaceTemperature + 273.15f;
    float Ta_K = airTemperature + 273.15f;

    // net shortwave radiation
    float Q_SW = (1f - albedo) * incomingShortwave;

    // net longwave radiation
    float Q_LW = (0.85f * sigma * Mathf.Pow(Ta_K, 4)) - (0.98f * sigma * Mathf.Pow(Ts_K, 4));
    
    // sensible heat flux
    float Q_S = (rhoAir * cpAir * (airTemperature - snowSurfaceTemperature)) / aerodynamicResistance;

    // latent heat flux
    float Q_L = (rhoAir * Lv * (q_a - q_s)) / aerodynamicResistance;

    // ground heat flux
    float snowConductivity = 0.138f * Mathf.Pow((snowDensity / 1000f), 2);
    float temperatureGradient = (snowSurfaceTemperature - groundTemperature) / snowDepth;
    float Q_G = -snowConductivity * temperatureGradient;

    float Q_net = Q_SW + Q_LW + Q_S + Q_L + Q_G ;
    float meltEnergyPerDay = Q_net * 86400f; 
    float M = Mathf.Max(0f, meltEnergyPerDay / (rhoWater * Lf)); 
    float retention = retentionFraction * SWE;
    float R = Mathf.Max(0f, M - retention);

    SWE += (P_snow - M - R) * time;
    SWE = Mathf.Max(SWE, 0f);

    snowDepth = (SWE * 1000f) / snowDensity;
    SetDepth(snowDepth);

    depthText.text = $"{snowDepth:F2}";

    Debug.Log($"SWE: {SWE:F2} mm, Depth: {snowDepth:F2} mm, Melt: {M:F2}");
  }

  void SetDepth(float depth)
  {
    surfaceViz.transform.localScale = new Vector3(1, depth / 1000, 1);
    surfaceViz.transform.position = new Vector3(transform.position.x, depth / 2000, transform.position.z);
  }

  void SetGroundTemperature(float airTemperature)
  {
    switch (surfaceType)
    {
      case SurfaceType.Concrete:
        groundTemperature = airTemperature + (0.28f * incomingShortwave / 1000f) + 2.5f;
        break;
      case SurfaceType.Asphalt:
        float airTemperatureF = airTemperature * 9f / 5f + 32f;
        groundTemperature = 26.081f - 0.844f * windSpeed - 0.187f * relativeHumidity - 0.0173f * incomingShortwave + 0.0042254f * windSpeed * airTemperature + 0.00565f * windSpeed * relativeHumidity + 0.0016f * windSpeed * incomingShortwave + 0.00342f * airTemperature * relativeHumidity + 0.000117f * airTemperature * incomingShortwave + 5.7029e-5f * relativeHumidity * incomingShortwave + 0.00425f * airTemperature * airTemperature + 1.9125e-5f * incomingShortwave * incomingShortwave;
        groundTemperature -= 32f;
        groundTemperature *= 5f / 9f;
        break;
      case SurfaceType.Grass:
        groundTemperature = airTemperature + (0.12f * incomingShortwave / 1000f) - 1.5f;
        break;
    }
  }
}
