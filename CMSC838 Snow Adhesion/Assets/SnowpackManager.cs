using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the bulk snowpack simulation on a specific surface.
/// Handles accumulation and melting based on environmental conditions.
/// </summary>
public class SnowpackManager : MonoBehaviour
{
    // Snowpack properties
    public float snowDepth = 0.0f; // depth of the snowpack (meters)
    public float snowWaterEquivalent = 0.0f; // Snow Water Equivalent (SWE) (meters)
    public float snowTemperature = -5.0f; // avg temperature of snowpack (Celsius) - Start below freezing

    // Snow properties
    public float newSnowDensity = 100.0f; // "Density of new snow (kg/m^3). Varies greatly, ~50-150 kg/m^3." Example: light, fluffy snow
    public float settledSnowDensity = 300.0f; // "Density of settled/aged snow (kg/m^3). ~200-400 kg/m^3." Example: more typical snowpack density
    public float densityIncreaseRate = 0.001f; // "Rate at which snow density increases over time (per second)." Simple linear increase rate (tune this)

    // Melting parameters
    public float meltingTemperature = 0.0f;
    // This is a simplified melting approach. More complex models use energy balance.
    public float degreeDayFactor = 3.0f; // "Degree-Day Factor (mm SWE per degree Celsius above 0 per day)." Example: 3 mm SWE melt per day per degree C above 0

    public EnvironmentManager environment; 
    public SurfaceProperties surfaceProperties; 
    private float currentSnowDensity; // current snowpack density
    public GameObject snowVisual; 

    void Awake()
    {
        currentSnowDensity = newSnowDensity;

        // Find required components if not assigned in the Inspector
        if (environment == null)
        {
            environment = FindObjectOfType<EnvironmentManager>();
            if (environment == null)
            {
                Debug.LogError("SurfaceSnowpackManager: EnvironmentManager not found in scene!");
            }
        }
        if (surfaceProperties == null)
        {
            surfaceProperties = GetComponent<SurfaceProperties>();
            if (surfaceProperties == null)
            {
                Debug.LogError("SurfaceSnowpackManager: SurfaceProperties not found on this GameObject!");
            }
        }
        if (snowVisual == null)
        {
            snowVisual = transform.Find("SnowVisual")?.gameObject;
            if (snowVisual == null)
            {
                Debug.LogError("SurfaceSnowpackManager: SnowVisual not found as child of this GameObject!");
            }
        }
    }

    void Update() {
        if (environment == null) return; 

        float P_snow = (environment.ambientTemperature <= 0.0f) ? environment.currentSnowfallRateSWE : 0f;
        float M = (environment.ambientTemperature > 0.0f)
            ? degreeDayFactor * (environment.ambientTemperature - 0.0f)
            : 0f;
        float retention = 0.05f * snowWaterEquivalent;
        float R = Mathf.Max(0f, M - retention);
        // Update SWE
        snowWaterEquivalent += P_snow - M - R;
        snowWaterEquivalent = Mathf.Max(snowWaterEquivalent, 0f); 
         Debug.Log("snowWaterEquivalent: " + snowWaterEquivalent);
        // Update snow depth
        snowDepth = (snowWaterEquivalent * 1000f) / 300f; // convert SWE to mm depth
        Debug.Log("Snow Depth: " + snowDepth);

        // Mass Balance Equation
        // float snowfallRateSWE_per_sec = environment.currentSnowfallRateSWE; 
        // float meltRate_per_sec = environment.ambientTemperature > meltingTemperature ? environment.ambientTemperature * degreeDayFactor / 86400.0f:  0.0f;
        // float runoffRate_per_sec = Mathf.Max(0.0f, meltRate_per_sec - 0.04f * snowWaterEquivalent); 
        // snowWaterEquivalent += (snowfallRateSWE_per_sec - meltRate_per_sec - runoffRate_per_sec) * Time.deltaTime;
        // snowWaterEquivalent = Mathf.Max(0.0f, snowWaterEquivalent); 
        // Debug.Log("SWE: " + snowWaterEquivalent);

        // currentSnowDensity = Mathf.MoveTowards(currentSnowDensity, settledSnowDensity, densityIncreaseRate * Time.deltaTime);
        // snowDepth = snowWaterEquivalent * (1000.0f / currentSnowDensity);

        if (snowVisual != null)
        {
            // Update the visual representation of the snowpack
            Transform snowVisualTransform = snowVisual.transform;
            snowVisualTransform.localScale = new Vector3(snowVisualTransform.localScale.x, snowDepth, snowVisualTransform.localScale.z);
            // snowVisualTransform.localPosition = new Vector3(snowVisualTransform.localPosition.x, snowDepth / 2f, snowVisualTransform.localPosition.z);
        }

    }

    // void Update()
    // {
    //     if (environment == null) return; 

    //     // --- 1. Simulate Accumulation ---
    //     float snowfallRateSWE_per_sec = environment.currentSnowfallRateSWE; 

    //     // Debug.Log("Snowfall rate: " + environment.currentSnowfallRateSWE);

    //     // Calculate accumulated SWE this frame and add to total SWE
    //     snowWaterEquivalent += snowfallRateSWE_per_sec * 3600.0f * Time.deltaTime;
    //     Debug.Log("SWE: " + snowWaterEquivalent);

    //     // Update current snow density (simple linear increase towards settled density)
    //     currentSnowDensity = Mathf.MoveTowards(currentSnowDensity, settledSnowDensity, densityIncreaseRate * Time.deltaTime);
    //     // Debug.Log("Snow Density: " + currentSnowDensity);

    //     // Update snow depth based on new SWE and current density
    //     // SWE (m) = Snow Depth (m) * (Density (kg/m^3) / Density of Water (1000 kg/m^3))
    //     // Snow Depth (m) = SWE (m) * (1000 kg/m^3 / Density (kg/m^3))
    //     if (currentSnowDensity > 0) 
    //     {
    //         snowDepth = snowWaterEquivalent * (1000.0f / currentSnowDensity);
    //         // Debug.Log("Snow Depth: " + snowDepth);
    //     } else {
    //         snowDepth = 0;
    //     }


    //     // --- 2. Simulate Melting (Simplified Degree-Day Model) ---
    //     // Note: A more accurate model would use energy balance (radiation, convection, conduction)
    //     // This degree-day approach assumes melting is proportional to air temperature above 0C.

    //     float meltSWE_this_frame = 0.0f;

    //     // Snow must be at melting temperature (0C) to melt
    //     if (snowTemperature >= meltingTemperature)
    //     {
    //         // Calculate degrees above melting temperature (using ambient temp for simplicity)
    //         // A more complex model would use the snow surface temperature or internal temp
    //         float degreesAboveMelting = Mathf.Max(0.0f, environment.ambientTemperature - meltingTemperature);
    //         // Debug.Log("temp: " + environment.ambientTemperature);
    //         // Debug.Log("deg above melting: " + degreesAboveMelting);

    //         // Calculate melt rate in SWE per day (using degree-day factor)
    //         float meltRateSWE_per_day = degreesAboveMelting * degreeDayFactor;
    //         // Debug.Log("meltRateSWE_per_day: " + degreesAboveMelting);

    //         // Convert melt rate to SWE per 
    //         float meltRateSWE_per_hr = meltRateSWE_per_day / 24.0f;

    //         // Calculate melted SWE this frame
    //         meltSWE_this_frame = meltRateSWE_per_hr * Time.deltaTime;
    //         Debug.Log("meltSWE_this_frame: " + meltSWE_this_frame);

    //         Debug.Log("Melt Rate: " + meltSWE_this_frame);
    //         // Ensure we don't melt more snow than exists
    //         meltSWE_this_frame = Mathf.Min(meltSWE_this_frame, snowWaterEquivalent);

    //         // Reduce SWE due to melting
    //         snowWaterEquivalent -= meltSWE_this_frame;

    //          // Re-calculate depth based on remaining SWE and current density
    //         if (currentSnowDensity > 0)
    //         {
    //              snowDepth = snowWaterEquivalent * (1000.0f / currentSnowDensity);
    //         } else {
    //              snowDepth = 0;
    //         }

    //         // If all snow has melted
    //         if (snowWaterEquivalent <= 0)
    //         {
    //             snowWaterEquivalent = 0;
    //             snowDepth = 0;
    //             currentSnowDensity = newSnowDensity; // Reset density for next snowfall
    //             snowTemperature = environment.ambientTemperature; // Snowless surface temp
    //         }
    //     }

    //     // --- 3. Simulate Snow Temperature Change ---
    //     // Simple model: Snow temperature moves towards ambient temperature,
    //     // but is capped at meltingTemperature if SWE > 0.
    //     // A more complex model would include heat transfer within the snow and with the ground.
    //     if (snowWaterEquivalent > 0)
    //     {
    //          // Snow temperature can't go above meltingTemperature (0C) if snow is present
    //          snowTemperature = Mathf.Min(meltingTemperature, Mathf.Lerp(snowTemperature, environment.ambientTemperature, Time.deltaTime * surfaceProperties.thermalResponsiveness));
    //     } else {
    //          // If no snow, surface temperature is managed by SurfaceProperties (reacting to ambient)
    //          snowTemperature = surfaceProperties.surfaceTemperature;
    //     }


    //     // --- 4. Visualize Snowpack ---
    //     if (snowVisual != null)
    //     {
    //         // Update the visual representation of the snowpack
    //         Transform snowVisualTransform = snowVisual.transform;
    //         snowVisualTransform.localScale = new Vector3(snowVisualTransform.localScale.x, snowDepth, snowVisualTransform.localScale.z);
    //         // snowVisualTransform.localPosition = new Vector3(snowVisualTransform.localPosition.x, snowDepth / 2f, snowVisualTransform.localPosition.z);
    //     }

    //     // You need to implement a way to visually represent the snow depth.
    //     // This could involve:
    //     // - Scaling a white cube/plane on top of the surface.
    //     // - Using a shader to displace the surface mesh based on snow depth.
    //     // - Enabling/disabling a separate snow mesh object.
    //     // Example (scaling a child object named "SnowVisual"):
    //     // Transform snowVisualTransform = transform.Find("SnowVisual");
    //     // if (snowVisualTransform != null)
    //     // {
    //     //     // Assuming SnowVisual is a plane or cube at the surface level
    //     //     // Adjust scale based on snowDepth. You might need to offset position too.
    //     //     snowVisualTransform.localScale = new Vector3(snowVisualTransform.localScale.x, snowDepth, snowVisualTransform.localScale.z);
    //     //     // You might also need to adjust its Y position: snowVisualTransform.localPosition = new Vector3(0, snowDepth / 2f, 0);
    //     // }
    // }

    // --- Public Methods (Optional) ---
    // You could add methods to manually set snow depth, trigger events, etc.
}
