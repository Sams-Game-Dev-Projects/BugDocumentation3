using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Weapons can be used by characters and have a lot of complexity
/// Weapons can deal damage to targets but will cost ammo to do so
/// Weapons when in use are cloned from the original version which allows them to have different values such as different particle systems
/// </summary>
[CreateAssetMenu(menuName = "Weapon Configuration")]
public class WeaponItem : EquipItem
{
    public Vector3 SpawnOffset; //When the weapon is spawned (in player hand for example) what offset does it use to look correct

    
    public AttackConfiguration attackConfiguration;     //A configurator that dictates how attacks are made
    public TrailConfiguration trailConfiguration;       //A configurator that dictates how trail effects appear

    public AmmoType validAmmoType;                      //What kind of ammo can this object use

    private ParticleSystem _particleSystem;             //A local reference the particle system used by the spawned prefab of this weapon
    private float _lastShootTime;                       //Tracks the last time this weapon attacked, needed to know if it is allowed to attack again

    private AmmoItem _currentAmmoItem;                  //Stores the current ammo within the firearm
    private int _ammoInClip;                            //How many of the ammo type are within the firearm
    [SerializeField] private float _reloadDuration = 1.5f; //How long a reload takes
    private bool _isReloading;                          //Is the weapon currently reloading?

    public override string GetItemType { get { return "Weapon"; } } //Allows the system to know this is a weapon
    public int GetAmmoInClip { get { return _ammoInClip; } }        //Expose current clip count
    public int GetClipSize { get { return _currentAmmoItem != null ? _currentAmmoItem.clipSize : 0; } }
    public bool IsReloading { get { return _isReloading; } }
    public bool CanFire { get { return _currentAmmoItem != null && _ammoInClip >= attackConfiguration.ammoNeededPerAttack && _isReloading == false; } }

    /// <summary>
    /// When this it is equip to the user
    /// Reset the last time it attacked to 0
    /// Get a reference to the particle effect of the spawned prefab
    /// Move the prefab to the offset position
    /// </summary>
    public override void Equip(Transform parent, MonoBehaviour user)
    {
        base.Equip(parent, user);
        _lastShootTime = 0f;

        _particleSystem = _model.GetComponentInChildren<ParticleSystem>();
        _particleSystem?.Stop();

        _model.transform.localPosition = SpawnOffset;
    }

    /// <summary>
    /// Pass in a list of item tokens, it is assumed that all items in the list are ammo
    /// Go through each item until one of the correct type is found
    /// Calculate the difference between how many are needed to max fill the weapon
    /// Fill the weapon with the difference making sure not to exceed the amount of ammo stored by the item token
    /// </summary>
    public ItemToken ReloadWeapon(List<ItemToken> possibleAmmo)
    {
        for (int i = possibleAmmo.Count - 1; i >= 0; i--)
        {
            AmmoItem curAmmo = possibleAmmo[i].GetItemBase as AmmoItem;
            if(curAmmo.ammoType != validAmmoType)
            {
                continue;
            }

            if(_currentAmmoItem == null && curAmmo.ammoType == validAmmoType)
            {
                _currentAmmoItem = curAmmo;
            }

            if(_currentAmmoItem == curAmmo)
            {
                _currentAmmoItem = possibleAmmo[i].GetItemBase as AmmoItem;

                int ammoMissingFromClip = Mathf.Abs(_currentAmmoItem.clipSize - _ammoInClip);

                int amountToRemove = Mathf.Min(ammoMissingFromClip, possibleAmmo[i].GetAmount);

                _ammoInClip += amountToRemove;
                possibleAmmo[i].AdjustAmount(-amountToRemove);
                return possibleAmmo[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Calls the base method that should despawn the generated prefab
    /// </summary>
    public override void Unequip()
    {
        base.Unequip();
    }

    /// <summary>
    /// Make sure the weapon is allowed to attack, for example, has it been too soon since the last attack request was made? Is there enougha ammo? Etc.
    /// Assuming the weapon can attack, remove the required ammo
    /// Set the last shoot time to the current time
    /// Then play the particle system to simulate the fire from a gun barrel
    /// Using the attack configuration, send a hitscan check based on variables such as the spread
    /// </summary>
    public void Attack(bool firstAttack)
    {
        if ((attackConfiguration.automaticAttacking == false && firstAttack == false) || _particleSystem == null || Time.time <= attackConfiguration.attackRate + _lastShootTime)
        {
            return;
        }

        if (_isReloading == true)
        {
            return;
        }

        if(_currentAmmoItem == null)
        {
            return;
        }

        if(_ammoInClip < attackConfiguration.ammoNeededPerAttack)
        {
            return;
        }

        _ammoInClip -= attackConfiguration.ammoNeededPerAttack;

        _lastShootTime = Time.time;
        _particleSystem.Play();

        Debug.Log("Attack fired: weapon=" + name + ", remaining clip=" + _ammoInClip);

        AttackConfiguration attackConfig = attackConfiguration;
        int projectiles = Mathf.Max(1, attackConfig.projectilesPerShot);

        for (int i = 0; i < projectiles; i++)
        {
            Vector3 attackDirection = _user.transform.forward;
            attackDirection += new Vector3
                (Random.Range(-attackConfig.spread.x, attackConfig.spread.x),
                Random.Range(-attackConfig.spread.y, attackConfig.spread.y),
                Random.Range(-attackConfig.spread.z, attackConfig.spread.z));
            attackDirection.Normalize();

            HitscanAttack(attackDirection);
        }
    }

    /// <summary>
    /// Does a raycast to see if it hit anything within the attack configurations layermask
    /// If it does, damage that object and make a trail renderer to represent the hit
    /// If it misses, simulate where the shot would have gone and make a trail renderer to represent the miss
    /// </summary>
    private void HitscanAttack(Vector3 attackDirection)
    {
        if (Physics.SphereCast(
        CalculateRaycastOrigin(),
        attackConfiguration.sphereRadius,
        attackDirection,
        out RaycastHit hit,
        attackConfiguration.maxDistance,
        attackConfiguration.hitMask))
        {
            _user.StartCoroutine(PlayTrailRenderer(_particleSystem.transform.position, hit.point, hit));

            hit.collider.GetComponent<IDamageable>()?.TakeDamage(attackConfiguration.CalculateDamage + _currentAmmoItem.CalculateDamage);
        }
        else
        {
            float missDistance = Mathf.Min(trailConfiguration.missDistance, attackConfiguration.maxDistance);
            _user.StartCoroutine(PlayTrailRenderer(
                _particleSystem.transform.position,
                _particleSystem.transform.position + (attackDirection * missDistance),
                new RaycastHit()));
        }
    }

    /// <summary>
    /// Begin a timed reload using available ammo.
    /// </summary>
    public System.Collections.IEnumerator ReloadRoutine(List<ItemToken> possibleAmmo, System.Action onStarted = null, System.Action onCompleted = null)
    {
        if (_isReloading)
        {
            yield break;
        }

        ItemToken tokenUsed = ReloadWeapon(possibleAmmo);
        if (tokenUsed == null)
        {
            yield break; // no ammo available
        }

        _isReloading = true;
        onStarted?.Invoke();

        float elapsed = 0f;
        while (elapsed < _reloadDuration)
        {
            elapsed += UnityEngine.Time.deltaTime;
            yield return null;
        }

        onCompleted?.Invoke();
        _isReloading = false;
    }

    /// <summary>
    /// Where is the shot coming from? Use the weapon itself and the character using it to determine the position.
    /// For example, the camera in the case of the player is the "user" where the camera is pointed is where the shots should go
    /// However, the tip of the weapon and the camera are not always looking at the same thing
    /// </summary>
    private Vector3 CalculateRaycastOrigin()
    {
        Vector3 origin = _particleSystem.transform.position;

        origin = _user.transform.position
            + _user.transform.forward
            * Vector3.Distance(_user.transform.position, _particleSystem.transform.position);

        return origin;
    }

    /// <summary>
    /// A trial is passed in
    /// Based on the trail configurator
    /// Set the colour, width, etc
    /// </summary>
    private void InitTrailRenderer(TrailRenderer instance)
    {
        instance.colorGradient = trailConfiguration.colour;
        instance.material = trailConfiguration.material;
        instance.widthCurve = trailConfiguration.widthCurve;
        instance.time = trailConfiguration.duration;
        instance.minVertexDistance = trailConfiguration.minVertexDistance;

        instance.emitting = false;
        instance.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    /// <summary>
    /// A trail is needed to simulate things like bullets
    /// This method is called to represent those objects
    /// A trail renderer is spawned and then places with a start and end position as needed based on the hit/miss
    /// Over time the renderer will change based on the trail configurator
    /// </summary>
    private System.Collections.IEnumerator PlayTrailRenderer(Vector3 startPosition, Vector3 endPosition, RaycastHit hit)
    {
        TrailRenderer instance = ObjectPooling.Spawn(EffectContainer.Instance._gunTrailEffect, startPosition, Quaternion.identity).GetComponent<TrailRenderer>();
        InitTrailRenderer(instance);
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPosition, endPosition);
        float remainingDistance = distance;

        while (remainingDistance > 0f)
        {
            instance.transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.Clamp01(1 - remainingDistance / distance));
            remainingDistance -= trailConfiguration.simulationSpeed * Time.deltaTime;
            yield return null;
        }

        instance.transform.position = endPosition;

        yield return new WaitForSeconds(trailConfiguration.duration);
        yield return null;

        _particleSystem.Stop();

        instance.emitting = false;
        ObjectPooling.Despawn(instance.gameObject);
    }
}
