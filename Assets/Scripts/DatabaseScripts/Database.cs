using UnityEngine;

using UnityEditor;


/// <summary>
/// A scriptable object that can store an array of "elements"
/// Each element will know what index it lives at within the array
/// The elements will know only when a button in the inspector is pressed
/// </summary>
[CreateAssetMenu(menuName = "Database")]
public class Database : ScriptableObject
{
    public DatabaseElement[] elements;
}

/// <summary>
/// A custom inspector for a database
/// This script simply draw the inspector as it normally would but also adds a button
/// This button when pressed will assign an index number to all elements within the database
/// An editor script not within the editor folder is allowed provided the user has correctly
/// Added protective lines of code. Otherwise there is a high chance that the project will not build
/// </summary>
[CustomEditor(typeof(Database))]
public class DatabaseEditor : Editor
{
    private Database _curDatabase;

    private void OnEnable()
    {
        _curDatabase = target as Database;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Set All Indexes"))
        {
            for (int i = 0; i < _curDatabase.elements.Length; i++)
            {
                _curDatabase.elements[i].SetItemDatabaseIndex(i);
                EditorUtility.SetDirty(_curDatabase.elements[i]);
            }
        }
    }
}