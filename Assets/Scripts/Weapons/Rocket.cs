using Managers;
using UnityEngine;

namespace Weapons
{
    public class Rocket : MonoBehaviour
    {
        private Rigidbody rocketRigidbody;

        private Vector3 climbTarget;
        private Vector3 attackTarget;
        private Vector3 currentTarget;

        public float nearRadius = 5f;

        private RocketStage stage = RocketStage.IDLE;

        private void Awake()
        {
            rocketRigidbody = GetComponent<Rigidbody>();
        }

        public void Launch(Vector3 target)
        {
            climbTarget = GameObject.FindGameObjectWithTag("RocketClimbTarget").transform.position;
            attackTarget = target;

            stage = RocketStage.CLIMB;
        }

        private void Update()
        {
            if (stage == RocketStage.IDLE)
                return;


            if (stage == RocketStage.CLIMB)
            {
                if (Vector3.Distance(rocketRigidbody.position, climbTarget) < nearRadius)
                {
                    stage = RocketStage.ATTACK;
                    currentTarget = attackTarget;
                }
                else
                    currentTarget = climbTarget;
            }


            var targetRotation = Quaternion.Lerp(rocketRigidbody.rotation, Quaternion.LookRotation(transform.position, currentTarget), 500.0f * Time.deltaTime);
            rocketRigidbody.rotation = targetRotation;
            rocketRigidbody.velocity = transform.up * 20f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(rocketRigidbody.transform.position, rocketRigidbody.transform.position + rocketRigidbody.velocity);
            Gizmos.DrawWireSphere(currentTarget, nearRadius);
        }
    }

    public enum RocketStage
    {
        IDLE,
        CLIMB,
        ATTACK
    }
}