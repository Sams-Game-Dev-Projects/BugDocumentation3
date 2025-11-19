using UnityEngine;

/// <summary>
/// Determines how the attack trail will look
/// Has no real effect on gameplay, just appearance
/// </summary>
[CreateAssetMenu(menuName = "Trail Configuration")]
public class TrailConfiguration : ScriptableObject
{
    public Material material;               //What material will the trail use
    public AnimationCurve widthCurve;       //How thick will the trail be at any given point of its life
    public float duration = 0.5f;           //How long will the trail last
    public float minVertexDistance = 0.1f;  //What is the minimum registered distance allowed between points
    public Gradient colour;                 //What colour will the trail be at any given point of its life

    public float missDistance = 100f;       //If the attack misses, how far can the trail go
    public float simulationSpeed = 100f;    //How quickly will the trail be simulated, 100 is standard/normal
}
