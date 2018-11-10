using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmedMine : MonoBehaviour
{
    public float triggerRadius = 1.0f;
    public float explosionRadius = 5.0f;
    public float explosionForce = 1000.0f;
    public float explosionDamage = 50.0f;
    public LayerMask triggerMask;
    public bool isActive = false;

    private void Awake()
    {
        GetComponent<SphereCollider>().radius = triggerRadius;

        StartCoroutine(ActivateMine());
    }

    private IEnumerator ActivateMine()
    {
        yield return new WaitForSeconds(2.0f);

        isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;
        
        var tankManager = other.gameObject.GetComponent<TankManager>();
        if (tankManager == null)
            return;
        
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, triggerMask);

        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++)
        {
            // ... and find their rigidbody.
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            // If they don't have a rigidbody, go on to the next collider.
            if (!targetRigidbody)
                continue;

            // Add an explosion force.
            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            // Find the TankHealth script associated with the rigidbody.
            TankHealth targetHealth = targetRigidbody.GetComponent<TankHealth>();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                continue;

            // Deal this damage to the tank.
            targetHealth.TakeDamage(explosionDamage);
        }

        // Destroy the shell.
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}