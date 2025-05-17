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
  public float airTemperature; // °C
  public float precipitation; // mm/day
  public float degreeDayFactor = 3; // mm/°C/day
  public float meltTempThreshold = 0.0f; // °C
  public float retentionFraction = 0.05f; // % of SWE retained as snowmelt
  public float snowSurfaceTemperature = 0f; // °C
  public float albedo = 0.85f;
  public float incomingShortwave; // W/m^2
  public float windSpeed = 2f; // m/s
  public float relativeHumidity = 70f; // %
  public float groundTemperature = -2f; // °C

  // Simulation Viz Variables
  public SurfaceTile[,] surfaceTiles;
  public GameObject surfaceTilePrefab;

  void Start()
  {
    airTemperature = -5.0f;
    precipitation = 3.0f;
    incomingShortwave = 450.0f;

    LoadSimulationData("New York");
  }

  void Update()
  {
    for (int i = 0; i < surfaceTiles.GetLength(0); i++)
    {
      for (int j = 0; j < surfaceTiles.GetLength(1); j++)
      {
        SurfaceTile tile = surfaceTiles[i, j];
        UpdateTile(tile);
      }
    }
  }

  void LoadSimulationData(string simName)
  {
    surfaceTiles = new SurfaceTile[10, 10];
    for (int i = 0; i < surfaceTiles.GetLength(0); i++)
    {
      for (int j = 0; j < surfaceTiles.GetLength(1); j++)
      {
        GameObject surfaceTileInstance = Instantiate(surfaceTilePrefab, new Vector3(i, 0, j), Quaternion.identity);
        surfaceTileInstance.transform.parent = transform;
        SurfaceTile tile = surfaceTileInstance.GetComponent<SurfaceTile>();
        surfaceTiles[i, j] = tile;
        surfaceTileInstance.name = $"SurfaceTile_{i}_{j}";
        UpdateTile(tile);
        if (i < 1 || i > 8)
        {
          tile.surfaceType = SurfaceType.Grass;
        }
        else if (i <= 3 || i >= 6)
        {
          tile.surfaceType = SurfaceType.Concrete;
        }
        else
        {
          tile.surfaceType = SurfaceType.Asphalt;
        }
      }
    }
  }

  void UpdateTile(SurfaceTile tile)
  {
    tile.airTemperature = airTemperature;
    tile.precipitation = precipitation;
    tile.windSpeed = windSpeed;
    tile.relativeHumidity = relativeHumidity;
    tile.incomingShortwave = incomingShortwave;

  }
}