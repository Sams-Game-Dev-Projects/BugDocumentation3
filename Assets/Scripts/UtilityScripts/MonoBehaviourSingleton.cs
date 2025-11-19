using UnityEngine;

/// <summary>
/// Making a singleton every 2 seconds can get annoying
/// This script acts as a base for all future scriptable objects that are monobehabiours
/// </summary>
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static T _instance = null;

    public static T Instance { get { return _instance; } }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }
}