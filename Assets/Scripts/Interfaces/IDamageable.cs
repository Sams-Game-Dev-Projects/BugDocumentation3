/// <summary>
/// Determines what can take damage
/// Anything that uses the IDamageable interface can be harmed by calling the TakeDamage method
/// </summary>

public interface IDamageable
{
    void TakeDamage(int amount);
}
