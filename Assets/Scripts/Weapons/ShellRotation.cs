using UnityEngine;

namespace Weapons
{
	[RequireComponent(typeof(Rigidbody))]
	public class ShellRotation : MonoBehaviour
	{
		private Rigidbody shellRigidbody;

		private void Awake()
		{
			shellRigidbody = GetComponent<Rigidbody>();
		}

		private void Update()
		{
			transform.rotation = Quaternion.LookRotation(shellRigidbody.velocity.normalized);
		}
	}
}
