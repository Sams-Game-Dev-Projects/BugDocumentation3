using UnityEngine;

/// <summary>
/// A scriptable object singleton that stores all effects that objects might need
/// Currently just the trail, but more might be needed such as explosions
/// </summary>
[CreateAssetMenu(menuName = "Effect Container")]
public class EffectContainer : ScriptableObjectSingleton<EffectContainer>
{
    public GameObject _gunTrailEffect;
}
