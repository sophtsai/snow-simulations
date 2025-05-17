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

  // State variables
  public float SWE = 0f; // mm
  public float snowDepth = 0f; // mm
  public float snowDensity = 300f; // kg/m^3
  GameObject surfaceViz;

  void Start()
  {
    airTemperature = -2.0f;
    precipitation = 1.0f;

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
    float M = airTemperature > meltTempThreshold ? degreeDayFactor * (airTemperature - meltTempThreshold) : 0f;
    float retention = retentionFraction * SWE;
    float R = Mathf.Max(0f, M - retention);

    SWE += (P_snow - M - R) * time;
    SWE = Mathf.Max(SWE, 0.000001f);

    snowDepth = (SWE * 1000f) / snowDensity;
    SetDepth(snowDepth);
    
    Debug.Log($"SWE: {SWE:F2} mm, Depth: {snowDepth:F2} mm, Melt: {M:F2}, Runoff: {R:F2}");
  }

  void SetDepth(float depth)
  {
    surfaceViz.transform.localScale = new Vector3(1, depth / 1000, 1);
    surfaceViz.transform.position = new Vector3(0, depth / 2000, 0);
  }
}
