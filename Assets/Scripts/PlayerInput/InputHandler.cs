using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// When the input listener detects input it sends a message to this script the handle that input
/// For example, being able to move, reload, etc
/// </summary>
public class InputHandler : MonoBehaviour
{
    [SerializeField] private float _movementSpeed;                  //How fast can the player move
    [SerializeField] private float _mouseSensitivity;               //How sensitive is the camera movement
    [SerializeField] private float _verticalMouseRange = 60f;       //How far up and down can the player look
    [SerializeField] private UnityEvent<List<ItemToken>> _onReload; //What happens when the player tries to reload

    [SerializeField] private PlayerData _playerData;    //A reference to the player's data, needed for their inventory when reloading

    private float _currentVerticalRotation = 0;         //Stores the current vertical rotation the player is using

    private Transform _mainCameraTransform;             //Local reference to the main camera's transform

    /// <summary>
    /// At the start of the game, find and store the camera's transform
    /// </summary>
    private void Awake()
    {
        _mainCameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// When trying to reload, the character needs to search their inventory for all items of the Ammo type
    /// Each that is found is placed into a list, that list is then sent to the method stored in the reload event to use
    /// </summary>
    public void OnReload()
    {
        _onReload?.Invoke(_playerData.GetInventory.GetAllItemsOfType("ammo"));
    }

    /// <summary>
    /// Moves the character forward and horizontally as needed
    /// </summary>
    public void Movement(Vector2 offset)
    {
        transform.position += transform.forward * offset.y * _movementSpeed * Time.deltaTime;
        transform.position += transform.right * offset.x * _movementSpeed * Time.deltaTime;
    }

    /// <summary>
    /// A passed in offset is broken into 2 values
    /// When using the X the whole character is rotated
    /// When using the Y only the main camera is rotated
    /// This allows the player to have full control of their vision
    /// </summary>
    public void MouseMovement(Vector2 offset)
    {
        float x = offset.x * _mouseSensitivity;
        transform.Rotate(0f, x, 0f);


        _currentVerticalRotation -= offset.y * _mouseSensitivity;
        _currentVerticalRotation = Mathf.Clamp(_currentVerticalRotation, -_verticalMouseRange, _verticalMouseRange);
        _mainCameraTransform.localRotation = Quaternion.Euler(_currentVerticalRotation, 0f, 0f);
    }
}