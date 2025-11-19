using UnityEngine;

/// <summary>
/// A singleton of a scriptable object that holds all databases
/// The current game only has 1 database, but more could be used in future such as a database for spells/psychic abilities
/// </summary>
[CreateAssetMenu(menuName = "Database Container")]
public class DatabaseContainer : ScriptableObjectSingleton<DatabaseContainer>
{
    public Database itemDatabase;
}
