using UnityEngine;

/// <summary>
/// The player can handle interactions unlike other characters
/// This script simply does a raycast when requested to interact
/// </summary>
public class PlayerInteractionHandler : MonoBehaviour, IInteractor
{
    [SerializeField] private float _maxInteractionDistance;
    [SerializeField] private GameObject _inventoryContainer;

    private Transform _cameraTransform;

    public IInventory GetInventory { get { return _inventoryContainer.GetComponent<IInventory>(); } }

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// When requested to interact
    /// Raycast to see if any Interactions are found, if so, call that interaction passing in this object
    /// </summary>
    public void InteractRequest()
    {
        if(Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, _maxInteractionDistance))
        {
            hit.collider.GetComponent<IInteract>()?.Interact(this);
        }
    }
}
