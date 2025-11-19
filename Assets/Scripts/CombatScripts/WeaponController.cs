using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All characters such as the player and enemies that can use weapons will need this script
/// It contains the currently equipped weapon
/// The code at the moment is very bad, as it makes a lot of assumptions and performs actions on start
/// Ideally, some other system, such as the inventory, can interact with this script to swap the equipped weapon
/// </summary>
public class WeaponController : MonoBehaviour
{
    public WeaponItem equippedWeapon;   //A public variable that stores the weapon the charater is using. This should be a private variable

    /// <summary>
    /// On start, equip the weapon this character apparently is using
    /// This functionality should be removed from start as it makes too many assumptions
    /// </summary>
    void Start()
    {
        ChangeWeapon(equippedWeapon);
    }

    /// <summary>
    /// Some weapon scriptable object is passed in
    /// Before the new weapon can be used
    /// If there is a weapon already in use it should be unequiped first
    /// Once that is done, instantiate a clone of the new weapon for this object to use
    /// Then equip the newly made clone
    /// </summary>
    public void ChangeWeapon(WeaponItem newWeapon)
    {
        //Tell the weapon to perform its version of an unequip
        equippedWeapon.Unequip();

        equippedWeapon = newWeapon;

        if (equippedWeapon == null)
        {
            return;
        }

        equippedWeapon = Instantiate(equippedWeapon);

        //Tell the weapon to perform its version of equip
        equippedWeapon.Equip(transform, this);
    }

    /// <summary>
    /// A list of ammos is passed in
    /// This list is needed so that the system can find ammo of the correct type
    /// This list is passed in to our currently equipped weapon
    /// </summary>
    public void ReloadEquippedWeapon(List<ItemToken> possibleAmmo)
    {
        if(equippedWeapon == null)
        {
            return;
        }

        //Tell the weapon to perform its version of reload
        equippedWeapon.ReloadWeapon(possibleAmmo);
    }

    /// <summary>
    /// This method allows the weapon to make an attack request
    /// The passed in "firstAttack" variable is used to determine if this is the first attack or not made in this request
    /// This is needed so that Automatic and non automatic weapons can perform their accutions correctly when the attack command is held
    /// </summary>
    public void OnAttack(bool firstAttack)
    {
        if (equippedWeapon == null)
        {
            return;
        }

        //Tell the weapon to perform its version of an attack
        equippedWeapon.Attack(false);
    }
}