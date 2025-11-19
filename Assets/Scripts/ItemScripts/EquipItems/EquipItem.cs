using UnityEngine;

/// <summary>
/// Some items can be worn by characters
/// Equip and Unquip methods are added to set up the character for using that item
/// </summary>
public abstract class EquipItem : ItemBase
{
    protected MonoBehaviour _user;
    protected GameObject _model;

    public virtual void Equip(Transform parent, MonoBehaviour user)
    {
        _user = user;
        _model = ObjectPooling.Spawn(itemBaseGameObject, parent);
    }

    public virtual void Unequip()
    {
        ObjectPooling.Despawn(_model);
    }
}