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
            while (true)
            {
                var colliders = Physics.OverlapSphere(transform.position, radius, targetMask);
                foreach (var targetCollider in colliders)
                {
                    var tankHealth = targetCollider.GetComponent<TankHealth>();
                    tankHealth.Heal(healingPower);
                }

                if (colliders.Length > 0)
                    burstParticleSystem.Emit(20);

                yield return new WaitForSeconds(1.0f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}