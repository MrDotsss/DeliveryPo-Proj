using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;

    public InventoryItem OnHand { get; private set; }

    private GameObject currentObject;
    private void Start()
    {
        InventoryManager.Instance.OnEquip += OnEquip;
        InventoryManager.Instance.OnDrop += OnDrop;

        /* Test only
        //InputManager.Instance.lmbAction.performed += _ =>
        //{
        //    InventoryManager.Instance.DropItem(CurrentItem);
        //};

        //InputManager.Instance.rmbAction.performed += _ =>   
        //{
        //    InventoryManager.Instance.EquipItem(
        //        InventoryManager.Instance.FindInventory("Black Ball"));
        //};
        */

        InventoryManager.Instance.EquipItem(InventoryManager.Instance.CurrentItem);
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnEquip -= OnEquip;
        InventoryManager.Instance.OnDrop -= OnDrop;
    }

    private void OnEquip(InventoryItem item)
    {
        Destroy(currentObject);

        if (item == null) return;

        OnHand = item;

        currentObject = Instantiate(item.data.itemPrefab, transform);
        currentObject.transform.localPosition = Vector3.zero;
        currentObject.transform.localRotation = Quaternion.identity;
        currentObject.GetComponent<MeshRenderer>().material.renderQueue = 4000;
        currentObject.layer = LayerMask.NameToLayer("Hand");

    }

    private void OnDrop(InventoryItem item)
    {
        if (OnHand == item)
        {
            OnHand = null;
            Destroy(currentObject);
        }

        GameObject toDrop = Instantiate(item.data.itemPrefab);

        toDrop.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        Rigidbody rb = toDrop.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Vector3 randomForce = orientation.transform.forward * Random.Range(3, 8) + orientation.transform.right * Random.Range(-2, 2);
        rb.AddForce(randomForce + (orientation.transform.up * 5), ForceMode.Impulse);
        rb.angularVelocity = randomForce * Random.Range(-5, 5);
        rb.drag = randomForce.magnitude / 3f;
        rb.angularDrag = randomForce.magnitude / 3f;
        PickableObject pickable = toDrop.AddComponent<PickableObject>();
        pickable.SetItemData(item);
    }
}
