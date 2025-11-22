using UnityEngine;

/// <summary>
/// A base type of scriptable object that all elements that exist in a database must derive from
/// The main advantage to this is that all elements will know their index within the database
/// This means instead of doing long complex tasks such as a for loop we can jump to the exact known position
/// </summary>
public abstract class DatabaseElement : ScriptableObject
{
    [SerializeField] protected int _index;

    public int GetDatabaseIndex { get { return _index; } }
    public void SetItemDatabaseIndex(int value)
    {
        _index = value;
    }
}
