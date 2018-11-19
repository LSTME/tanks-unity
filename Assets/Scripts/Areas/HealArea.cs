using System.Collections;
using Tank;
using UnityEngine;

namespace Areas
{
    public class HealArea : MonoBehaviour
    {
        public float radius = 10.0f;
        public LayerMask targetMask;

        public float healingPower = 5.0f;

        public ParticleSystem burstParticleSystem;

        private void Awake()
        {
            StartCoroutine(Heal());
        }

        private IEnumerator Heal()
        {
            // TODO: heal players around
            return null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}