using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SurfaceType
{
  Concrete, Asphalt, Grass
}

public class SurfaceTile : MonoBehaviour
{
  // Inputs
  public float airTemperature; // °C
  public float precipitation; // mm/day
  public float degreeDayFactor = 3; // mm/°C/day
  public float meltTempThreshold = 0.0f; // °C
  public float retentionFraction = 0.05f; // % of SWE retained as snowmelt
  public float snowSurfaceTemperature = 0f; // °C
  public float albedo = 0.85f;
  public float incomingShortwave = 200f; // W/m^2
  public float incomingLongwave = 300f; // W/m^2
  public float windSpeed = 2f; // m/s
  public float relativeHumidity = 70f; // %
  public float soilTemperature = 2f; // °C  

  // State variables
  public float SWE = 0f; // mm
  public float snowDepth = 0f; // mm
  public float snowDensity = 300f; // kg/m^3
  const float sigma = 5.67e-8f; // W/m^2*K^4
  const float rhoAir = 1.225f; // kg/m^3
  const float cpAir = 1005f; // J/kg*K
  const float Lv = 2.5e6f; // J/kg
  const float Lf = 3.34e5f; // J/kg 
  const float rhoWater = 1000f; // kg/m^3
  const float snowConductivity = 0.2f; // W/m*K
  const float aerodynamicResistance = 100f; // s/m
  GameObject surfaceViz;

  void Start()
  {
    airTemperature = -2.0f;
    precipitation = 2.0f;

    surfaceViz = GameObject.CreatePrimitive(PrimitiveType.Cube);
    surfaceViz.transform.localScale = new Vector3(1, 0.0f, 1);
  }

  void Update()
  {
    SimulateDay(Time.deltaTime); // parameter means 1 day runs every second
  }

  void SimulateDay(float time)
  {
    float P_snow = airTemperature <= 0f ? precipitation : 0f;
    

    // snowDepth = (SWE * 1000f) / snowDensity;
    // SetDepth(snowDepth);
    
    float Ts_K = snowSurfaceTemperature + 273.15f;
    float Ta_K = airTemperature + 273.15f;

    float Q_SW = (1f - albedo) * incomingShortwave;
    float Q_LW = incomingLongwave - (0.98f * sigma * Mathf.Pow(Ts_K, 4));
    float Q_S = (rhoAir * cpAir * (airTemperature - snowSurfaceTemperature)) / aerodynamicResistance;

    float saturationHumidity = 0.611f * Mathf.Exp((17.3f * snowSurfaceTemperature) / (snowSurfaceTemperature + 237.3f)); 
    float actualHumidity = relativeHumidity / 100f * saturationHumidity;
    float Q_L = (rhoAir * Lv * (actualHumidity - saturationHumidity)) / aerodynamicResistance;

    float Q_G = snowConductivity * ((soilTemperature - snowSurfaceTemperature) / 0.1f);
    
    float Q_net = Q_SW + Q_LW + Q_S + Q_G - Q_L;
    float meltEnergyPerDay = Q_net * 86400f; 
    float meltDepth = Mathf.Max(0f, meltEnergyPerDay / (rhoWater * Lf)); 
    // float melt_mm = meltDepth * 1000f;

    float M = meltDepth;
    // float M = airTemperature > meltTempThreshold ? degreeDayFactor * (airTemperature - meltTempThreshold) : 0f;
    float retention = retentionFraction * SWE;
    float R = Mathf.Max(0f, M - retention);

    SWE += (P_snow - M - R) * time;
    SWE = Mathf.Max(SWE, 0f);

    // SWE -= melt_mm;
    // SWE = Mathf.Max(SWE, 0f);

    // snowDepth = (SWE * 1000f) / snowDensity;
    // SetDepth(snowDepth);

    Debug.Log($"SWE: {SWE:F2} mm, Depth: {snowDepth:F2} mm, Melt: {M:F2}");

    // Debug.Log($"SWE: {SWE:F2} mm, Depth: {snowDepth:F2} mm, Melt: {M:F2}, Runoff: {R:F2}");
  }

  void SetDepth(float depth)
  {
    surfaceViz.transform.localScale = new Vector3(1, depth / 1000, 1);
    surfaceViz.transform.position = new Vector3(0, depth / 2000, 0);
  }
}
