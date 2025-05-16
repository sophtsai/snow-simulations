using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceProperties : MonoBehaviour
{
    public float adhesionFactor = 0.5f;
    public float surfaceTemperature = 0.0f; // Surface temperature in degrees Celsius
    public float thermalResponsiveness = 0.5f; // How quickly surfance responds to temperature changes

    // You could add other properties here, like roughness, material type, etc.
    // public float roughness = 0.1f;
    // public enum MaterialType { Concrete, Metal, Plastic }
    // public MaterialType material = MaterialType.Concrete;

    // You might add a method here to handle adding stuck snowflakes
    // public void AddStuckSnowflake(GameObject snowflake)
    // {
    //     // Keep track of stuck snowflakes, maybe for accumulation logic
    //     // For example, add to a List<GameObject>
    // }
}
