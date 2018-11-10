using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public PickupType pickupType;
    public float PickupRadius = 1.0f;
    public float respawnDelay = 5.0f;

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = PickupRadius;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 30.0f);
    }

    private void OnDrawGizmos()
    {
        var pickupCollider = GetComponent<SphereCollider>();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pickupCollider.transform.position, PickupRadius);
    }

    private void OnTriggerStay(Collider other)
    {
        var tankManager = other.gameObject.GetComponent<TankManager>();
        if (tankManager != null)
        {
            if (tankManager.pickup(pickupType))
                StartCoroutine(Cooldown());
        }
    }

    private IEnumerator Cooldown()
    {
        toggleActive(false);
        yield return new WaitForSeconds(respawnDelay);
        toggleActive(true);
    }

    private void toggleActive(bool value)
    {
        gameObject.GetComponent<Collider>().enabled = value;
        foreach (Transform childTransform in transform)
            childTransform.gameObject.SetActive(value);
    }
}

public enum PickupType
{
    AMMO,
    SHIELD
}