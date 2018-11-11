using System.Collections;
using Managers;
using UnityEngine;

namespace Pickups
{
    public class GenericPickup : MonoBehaviour
    {
        public PickupType pickupType;
        public float pickupRadius = 1.0f;
        public float respawnDelay = 5.0f;

        private void Awake()
        {
            GetComponent<SphereCollider>().radius = pickupRadius;
        }

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 30.0f);
        }

        private void OnDrawGizmos()
        {
            var pickupCollider = GetComponent<SphereCollider>();
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pickupCollider.transform.position, pickupRadius);
        }

        private void OnTriggerStay(Collider other)
        {
            var tankManager = other.gameObject.GetComponent<TankManager>();
            if (tankManager == null) return;

            if (tankManager.Pickup(pickupType))
                StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            ToggleActive(false);
            yield return new WaitForSeconds(respawnDelay);
            ToggleActive(true);
        }

        private void ToggleActive(bool value)
        {
            gameObject.GetComponent<Collider>().enabled = value;
            foreach (Transform childTransform in transform)
                childTransform.gameObject.SetActive(value);
        }
    }

    public enum PickupType
    {
        Ammo,
        Shield,
        Mines
    }
}