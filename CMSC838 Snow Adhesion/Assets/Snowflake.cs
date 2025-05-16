using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowflake : MonoBehaviour
{
    public float baseAdhesionProbability = 0.5f;
    public float temperatureAdhesionMultiplier = 0.1f; // Adjusts how much temp affects sticking
    private bool hasStuck = false; // Flag to prevent multiple adhesion events


    /// Called when this collider/rigidbody has begun touching another rigidbody/collider.
    void OnCollisionEnter(Collision collision)
    {
        if (hasStuck)
        {
            return;
        }

        GameObject hitObject = collision.gameObject;

        // Try to get the SurfaceProperties script from the hit object
        SurfaceProperties surfaceProps = hitObject.GetComponent<SurfaceProperties>();

        // If the hit object has SurfaceProperties, apply our custom adhesion logic
        if (surfaceProps != null)
        {
            // --- Adhesion Logic ---
            // Calculate a modified adhesion probability based on surface properties.
            // This is a simple example: base probability + factor * surface adhesion factor
            // + temperature influence (e.g., warmer surfaces near 0C might increase sticking)

            float currentAdhesionProbability = baseAdhesionProbability * surfaceProps.adhesionFactor;

            // Simple temperature influence: Assuming temperatureAdhesionMultiplier scales
            // the effect of temperature difference from freezing (0C).
            // Surfaces near freezing (0C) might increase sticking probability.
            // You might need a more complex function depending on your research needs.
            // Example: increase probability as temp approaches 0 from below, decrease above 0.
            float tempInfluence = 0f;
            if (surfaceProps.surfaceTemperature < 0.1f && surfaceProps.surfaceTemperature > -5.0f) // Example range near freezing
            {
                 // Simple linear increase towards 0C from -5C, max at 0C
                 tempInfluence = (0.1f - surfaceProps.surfaceTemperature) / 5.1f * temperatureAdhesionMultiplier;
            } else if (surfaceProps.surfaceTemperature >= 0.1f)
            {
                 // Simple linear decrease above 0C
                 tempInfluence = - (surfaceProps.surfaceTemperature - 0.1f) * temperatureAdhesionMultiplier;
            }

            currentAdhesionProbability += tempInfluence;

            // Clamp the probability between 0 and 1
            currentAdhesionProbability = Mathf.Clamp01(currentAdhesionProbability);

            // Decide whether to stick based on the calculated probability
            if (Random.value < currentAdhesionProbability)
            {
                Stick();
            }
            else
            {
                // If not sticking, Unity's physics will handle the bounce/slide
                // based on the Rigidbody and Collider properties (friction, bounciness).
                // You could add custom bounce/slide logic here if needed.
            }
        }
        else
        {
            // If the hit object doesn't have SurfaceProperties (e.g., hitting the environment boundary)
            // you might want to destroy the snowflake or handle it differently.
            // For now, we'll just let it bounce/interact with standard physics.
            // If it hits something without SurfaceProperties, it won't stick via this script's logic.
        }
    }

    /// <summary>
    /// Makes the snowflake stick to the surface it collided with.
    /// </summary>
    void Stick()
    {
        if (hasStuck) return; // Prevent sticking multiple times

        // Mark as stuck
        hasStuck = true;

        // Stop physics simulation for this snowflake
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Stop being affected by forces
            rb.velocity = Vector3.zero; // Stop movement
            rb.angularVelocity = Vector3.zero; // Stop rotation
        }

        // Disable the collider so it doesn't interfere with future collisions
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Optional: Parent the snowflake to the object it stuck to
        // This makes the snowflake move with the surface if the surface moves.
        // transform.SetParent(collision.transform); // This requires access to the collision data still,
        // A better approach might be to store the hit object in OnCollisionEnter
        // For simplicity here, we'll skip parenting or assume it's not needed for static surfaces.
        // If parenting is crucial, store collision.transform in a variable in OnCollisionEnter
        // and access it here.

        // You could also add the snowflake to a list on the surface object
        // to track accumulated snow.
        // Example: surfaceProps.AddStuckSnowflake(this.gameObject);

        // Optionally, destroy the snowflake after a delay if you don't need
        // persistent visual accumulation, or if you use a different accumulation method.
        // Destroy(gameObject, 10f); // Destroy after 10 seconds
    }

    // You might add other functions here, e.g., to handle melting over time
    // based on ambient temperature or surface temperature.
}
