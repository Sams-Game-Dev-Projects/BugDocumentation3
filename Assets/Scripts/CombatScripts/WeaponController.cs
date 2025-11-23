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

    public System.Func<List<ItemToken>> AmmoProvider { get; set; } //Injected inventory accessor for auto-reload
    private bool _pendingFireAfterReload;
    private string _ammoDisplay = string.Empty;
    [SerializeField] private GUIStyle _ammoStyle;

    /// <summary>
    /// On start, equip the weapon this character apparently is using
    /// This functionality should be removed from start as it makes too many assumptions
    /// </summary>
    void Start()
    {
        if (AmmoProvider == null)
        {
            AmmoProvider = () => PlayerData.Instance?.GetInventory.GetAllItemsOfType("Ammo");
        }
        ChangeWeapon(equippedWeapon);
        UpdateAmmoHUD();
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
        UpdateAmmoHUD();
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
        StartCoroutine(equippedWeapon.ReloadRoutine(possibleAmmo, onStarted: () => Debug.Log($"Reloading {equippedWeapon.name}..."), onCompleted: () => Debug.Log($"Reloaded {equippedWeapon.name}")));
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
            Debug.LogWarning("OnAttack called but no weapon is equipped.");
            return;
        }

        // For non-automatic weapons, only process the initial press
        if ((equippedWeapon.attackConfiguration?.automaticAttacking ?? false) == false && firstAttack == false)
        {
            return;
        }

        if (equippedWeapon.IsReloading)
        {
            return;
        }

        if (equippedWeapon.CanFire == false)
        {
            TryAutoReload(fireAfterReload: firstAttack);
            return;
        }

        Debug.Log($"OnAttack: firstAttack={firstAttack}, weapon={equippedWeapon.name}, ammoInClip={(equippedWeapon as WeaponItem)?.GetAmmoInClip}");
        //Tell the weapon to perform its version of an attack
        equippedWeapon.Attack(firstAttack);
        UpdateAmmoHUD();
    }

    private void TryAutoReload(bool fireAfterReload = false)
    {
        if (AmmoProvider == null)
        {
            return;
        }

        List<ItemToken> ammo = AmmoProvider.Invoke();
        if (ammo == null || ammo.Count == 0)
        {
            return;
        }

        if (equippedWeapon.IsReloading)
        {
            _pendingFireAfterReload |= fireAfterReload;
            return;
        }

        _pendingFireAfterReload = fireAfterReload;
        StartCoroutine(equippedWeapon.ReloadRoutine(
            ammo,
            onStarted: () => Debug.Log($"Auto reloading {equippedWeapon.name}..."),
            onCompleted: () =>
            {
                Debug.Log($"Auto reloaded {equippedWeapon.name}");
                UpdateAmmoHUD();
                if (_pendingFireAfterReload && equippedWeapon.CanFire)
                {
                    equippedWeapon.Attack(true);
                }
                _pendingFireAfterReload = false;
            }));
    }

    private void OnGUI()
    {
        if (equippedWeapon == null)
        {
            return;
        }

        if (_ammoStyle == null)
        {
            _ammoStyle = new GUIStyle(GUI.skin.label);
            _ammoStyle.fontSize = (int)(_ammoStyle.fontSize * 4f);
            _ammoStyle.normal.textColor = Color.red;
        }

        GUI.Label(new Rect(15, 15, 400, 60), _ammoDisplay, _ammoStyle);
    }

    private void UpdateAmmoHUD()
    {
        if (equippedWeapon == null)
        {
            _ammoDisplay = string.Empty;
            return;
        }

        int clip = equippedWeapon.GetAmmoInClip;
        int size = equippedWeapon.GetClipSize;
        _ammoDisplay = size > 0 ? $"Ammo: {clip}/{size}" : $"Ammo: {clip}";
    }
}
