using System.Collections;
using Managers;
using Tank;
using UnityEngine;

namespace Weapons
{
    public class Mine : MonoBehaviour
    {
        public float triggerRadius = 1.0f;

        public float explosionRadius = 5.0f;
        public float explosionForce = 1000.0f;
        public float explosionDamage = 50.0f;
        public ParticleSystem explosionParticleSystem;
        private AudioSource explosionAudio;

        public AudioSource beepAudio;
        public Light armedLight;
        public Color safeColor = Color.green;
        public Color armedColor = Color.red;

        public LayerMask triggerMask;
        public bool isArmed;

        private void Awake()
        {
            GetComponent<SphereCollider>().radius = triggerRadius;
            explosionAudio = explosionParticleSystem.GetComponent<AudioSource>();

            StartCoroutine(ArmMine());
        }

        private IEnumerator ArmMine()
        {
            armedLight.color = safeColor;

            yield return new WaitForSeconds(1.0f);

            armedLight.color = armedColor;
            isArmed = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isArmed)
                return;

            var tankManager = other.gameObject.GetComponent<TankManager>();
            if (tankManager == null)
                return;

            StartCoroutine(Explode());
        }

        private IEnumerator Explode()
        {
            beepAudio.Play();
            yield return new WaitForSeconds(0.2f);

            // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius, triggerMask);

            // Go through all the colliders...
            foreach (var explosionCollider in colliders)
            {
                // ... and find their rigidbody.
                var targetRigidbody = explosionCollider.GetComponent<Rigidbody>();

                // If they don't have a rigidbody, go on to the next collider.
                if (!targetRigidbody)
                    continue;

                // Add an explosion force.
                targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);

                // Find the TankHealth script associated with the rigidbody.
                var targetHealth = targetRigidbody.GetComponent<TankHealth>();

                // If there is no TankHealth script attached to the gameobject, go on to the next collider.
                if (!targetHealth)
                    continue;

                // Deal this damage to the tank.
                targetHealth.TakeDamage(explosionDamage);
            }

            // Unparent the particles from the shell.
            explosionParticleSystem.transform.parent = null;

            // Play the particle system.
            explosionParticleSystem.Play();

            // Play the explosion sound effect.
            explosionAudio.Play();

            // Once the particles have finished, destroy the gameobject they are on.
            var mainModule = explosionParticleSystem.main;
            Destroy(explosionParticleSystem.gameObject, mainModule.duration);

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
}