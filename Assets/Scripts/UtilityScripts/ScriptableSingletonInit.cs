using UnityEngine;

/// <summary>
/// This script is a convenient way to store all of the scriptable objects that need to be singletons
/// Place this script and all the needed singletons into the array in a scene you KNOW the player will see
/// For example the main menu
/// When the FirstInitialize method is run in the ScriptableObjectSinglton method it will be set up for our use
/// </summary>
public class ScriptableSingletonInit : MonoBehaviour
{
    [SerializeField] private ScriptableObject[] _scriptableSingletons;
}
