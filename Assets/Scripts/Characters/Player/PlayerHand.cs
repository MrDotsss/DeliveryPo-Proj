using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the player's currently equipped item (the item "in hand").
/// Listens to inventory equip and drop events to update the hand accordingly.
/// </summary>
public class PlayerHand : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;  // Reference for forward and right directions used when dropping items

    public InventoryItem OnHand { get; private set; }  // Currently held inventory item

    private GameObject currentObject;  // Instantiated GameObject representing the item visually in hand

    /// <summary>
    /// Subscribe to InventoryManager events and equip the current item on start.
    /// </summary>
    private void Start()
    {
        InventoryManager.Instance.OnEquip += OnEquip;
        InventoryManager.Instance.OnDrop += OnDrop;

        InventoryManager.Instance.EquipItem(InventoryManager.Instance.CurrentItem);
    }

    /// <summary>
    /// Unsubscribe from InventoryManager events on destroy.
    /// </summary>
    private void OnDestroy()
    {
        InventoryManager.Instance.OnEquip -= OnEquip;
        InventoryManager.Instance.OnDrop -= OnDrop;
    }

    /// <summary>
    /// Called when an item is equipped: destroys previous item GameObject and instantiates the new one in hand.
    /// </summary>
    /// <param name="item">Item to equip</param>
    private void OnEquip(InventoryItem item)
    {
        Destroy(currentObject);

        if (item == null) return;

        OnHand = item;

        currentObject = Instantiate(item.data.itemPrefab, transform);
        currentObject.transform.localPosition = Vector3.zero;
        currentObject.transform.localRotation = Quaternion.identity;

        // Set rendering order and layer for hand item
        currentObject.GetComponent<MeshRenderer>().material.renderQueue = 4000;
        currentObject.layer = LayerMask.NameToLayer("Hand");
    }

    /// <summary>
    /// Called when an item is dropped: removes item from hand if held, instantiates dropped object with physics and pickable behavior.
    /// </summary>
    /// <param name="item">Item to drop</param>
    private void OnDrop(InventoryItem item)
    {
        // Clear hand if dropped item is currently held
        if (OnHand == item)
        {
            OnHand = null;
            Destroy(currentObject);
        }

        // Instantiate dropped item in world with physics
        GameObject toDrop = Instantiate(item.data.itemPrefab);
        toDrop.transform.SetPositionAndRotation(transform.position, Quaternion.identity);

        Rigidbody rb = toDrop.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Apply random force and torque to simulate throw/drop
        Vector3 randomForce = orientation.transform.forward * Random.Range(3, 8) + orientation.transform.right * Random.Range(-2, 2);
        rb.AddForce(randomForce + (orientation.transform.up * 5), ForceMode.Impulse);
        rb.angularVelocity = randomForce * Random.Range(-5, 5);
        rb.drag = randomForce.magnitude / 3f;
        rb.angularDrag = randomForce.magnitude / 3f;

        // Add PickableObject component and assign item data for pickup functionality
        PickableObject pickable = toDrop.AddComponent<PickableObject>();
        pickable.SetItemData(item);
    }
}
