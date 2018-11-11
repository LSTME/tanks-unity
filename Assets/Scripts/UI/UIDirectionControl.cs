using UnityEngine;

namespace UI
{
    public class UIDirectionControl : MonoBehaviour
    {
        // This class is used to make sure world space UI
        // elements such as the health bar face the correct direction.

        public bool useRelativeRotation = true; // Use relative rotation should be used for this gameobject?
        private Quaternion relativeRotation; // The local rotatation at the start of the scene.

        private void Start()
        {
            relativeRotation = transform.parent.localRotation;
        }

        private void Update()
        {
            if (useRelativeRotation)
                transform.rotation = relativeRotation;
        }
    }
}