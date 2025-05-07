using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    [SerializeField] private Transform neck;
    private GameObject currentObject;
    private InventoryItem currentItem;
    [Header("Collision")]
    public float checkDistance = 0.5f;
    public Vector3 halfExtents = Vector3.one * 0.1f;

    public Camera cam;

    private void Start()
    {
        PlayerInventory.Instance.OnEquipItem += EquipInstance;
        PlayerInventory.Instance.OnDropItem += DropInstance;

        cam = Camera.main;
    }

    private void OnDestroy()
    {
        PlayerInventory.Instance.OnEquipItem -= EquipInstance;
        PlayerInventory.Instance.OnDropItem -= DropInstance;
    }

    private void Update()
    {
        PushBack();
    }

    private void EquipInstance(InventoryItem item)
    {
        Destroy(currentObject);

        if (item == null) return;

        currentItem = item;
        currentObject = Instantiate(item.itemData.modelPrefab, transform);
        currentObject.layer = LayerMask.NameToLayer("Hand");

        foreach (Transform child in currentObject.GetComponentInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer("Hand");
        }
    }

    private void DropInstance(InventoryItem item)
    {
        if (currentItem == item)
        {
            currentItem = null;
            Destroy(currentObject);
        }

        GameObject toDrop = Instantiate(item.itemData.modelPrefab);
        toDrop.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        Rigidbody rb = toDrop.AddComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Vector3 randomForce = neck.forward * Random.Range(3, 8) + neck.right * Random.Range(-2, 2);
        rb.AddForce(randomForce + (neck.up * 5), ForceMode.Impulse);
        rb.angularVelocity = randomForce * Random.Range(-5, 5);
        rb.drag = randomForce.magnitude / 3f;
        rb.angularDrag = randomForce.magnitude / 3f;
        PickableObject pickable = toDrop.AddComponent<PickableObject>();
        pickable.inventoryItem = item;
    }

    private void PushBack()
    {
        bool collided = Physics.BoxCast(cam.transform.position, halfExtents, cam.transform.forward, transform.rotation, checkDistance);

        float targetZ = collided ? 0.5f : 0.8f;

        Vector3 localPos = transform.localPosition;

        localPos.z = Mathf.MoveTowards(localPos.z, targetZ, Time.deltaTime);
        transform.localPosition = localPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(cam.transform.position, cam.transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.forward * checkDistance, halfExtents * 2);
    }
}
