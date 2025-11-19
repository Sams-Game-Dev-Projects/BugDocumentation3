using UnityEngine;

/// <summary>
/// Similar to the Monobehaviour version
/// This script makes it quicker to make singletons of scriptable objects
/// The only downside of this is that something in a scene must contain this object
/// A common way to do this is, in the main menu, a scene literally every player must pass
/// Have an object that stores the scriptable object that way it is certain to trigger
/// </summary>
public abstract class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                T[] results = Resources.FindObjectsOfTypeAll<T>();

                if (results.Length == 0)
                {
                    //None exist
                    return null;
                }
                else if (results.Length > 1)
                {
                    //Too many
                    return null;
                }

                _instance = results[0];
                _instance.hideFlags = HideFlags.DontUnloadUnusedAsset;
            }

            return _instance;
        }
    }


    /// <summary>
    /// This method is empty but REALLY important
    /// The FirstInitialize method is what is going to allow us to turn this into a singleton
    /// Despite being empty
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    protected static void FirstInitialize()
    {
        //Only used to make sure this object is stored
    }
}