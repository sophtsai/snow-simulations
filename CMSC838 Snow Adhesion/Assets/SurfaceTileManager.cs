using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceTileManager : MonoBehaviour
{
  // Constants


  // Environment Variables
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

  // Simulation Viz Variables
  public SurfaceTile[,] surfaceTiles;
  public GameObject surfaceTilePrefab;

  void Start()
  {
    LoadSimulationData("temp");
  }

  void Update()
  {

  }

  void LoadSimulationData(string simName)
  {
    surfaceTiles = new SurfaceTile[10, 10];
    Debug.Log("length" + surfaceTiles.GetLength(0) + " " + surfaceTiles.GetLength(1));
    for (int i = 0; i < surfaceTiles.GetLength(0); i++)
    {
      for (int j = 0; j < surfaceTiles.GetLength(1); j++)
      {
        Debug.Log("i" + i + " j" + j);
        GameObject surfaceTileInstance = Instantiate(surfaceTilePrefab, new Vector3(i, 0, j), Quaternion.identity);
        surfaceTiles[i, j] = surfaceTileInstance.GetComponent<SurfaceTile>();
      }
    }
  }
}