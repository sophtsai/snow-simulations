using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceTileManager : MonoBehaviour
{
  // Constants
  const float sigma = 5.67e-8f; // W/m^2*K^4
  const float rhoAir = 1.225f; // kg/m^3 (density of air)
  const float cpAir = 1005f; // J/kg*K (specific heat of air)
  const float Lv = 2.5e6f; // J/kg (latent heat of vaporization)
  const float Ls = 2.834e6f; // J/kg (latent heat of sublimation)
  const float Lf = 3.34e5f; // J/kg (latent heat of fusion)


  // Environment Variables
  public float airTemperature = -3.0f; // °C
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

  // Simulation Viz Variables
  public SurfaceTile[,] surfaceTiles;
  public GameObject surfaceTilePrefab;

  void Start()
  {
    LoadSimulationData("New York");
  }

  void Update()
  {

  }

  void LoadSimulationData(string simName)
  {
    surfaceTiles = new SurfaceTile[10, 10];
    for (int i = 0; i < surfaceTiles.GetLength(0); i++)
    {
      for (int j = 0; j < surfaceTiles.GetLength(1); j++)
      {
        GameObject surfaceTileInstance = Instantiate(surfaceTilePrefab, new Vector3(i, 0, j), Quaternion.identity);
        SurfaceTile tile = surfaceTileInstance.GetComponent<SurfaceTile>();
        surfaceTiles[i, j] = tile;
        surfaceTileInstance.name = $"SurfaceTile_{i}_{j}";
        tile.Initialize(SurfaceType.Concrete);
      }
    }
  }
}