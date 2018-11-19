using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Tank
{
    [RequireComponent(typeof(TankManager))]
    public class TankShooting : MonoBehaviour
    {
        public int playerNumber = 1; // Used to identify the different players.

        public GameObject shellPrefab; // Prefab of the shell.
        public GameObject armedMinePrefab;

        public Transform fireTransform; // A child of the tank where the shells are spawned.
        public Slider aimSlider; // A child of the tank that displays the current launch force.

        public AudioSource shootingAudio; // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.

        public AudioClip chargingClip; // Audio that plays when each shot is charging up.
        public AudioClip fireClip; // Audio that plays when each shot is fired.

        public float minLaunchForce = 5.0f; // The force given to the shell if the fire button is not held.
        public float maxLaunchForce = 30.0f; // The force given to the shell if the fire button is held for the max charge time.

        public float maxChargeTime = 0.75f; // How long the shell can charge for before it is fired at max force.

        private string fireButton; // The input axis that is used for launching shells.
        private string altFireButton; // The input axis that is used for launching shells.
        private float currentLaunchForce; // The force that will be given to the shell when the fire button is released.
        private float chargeSpeed; // How fast the launch force increases, based on the max charge time.
        private bool fired; // Whether or not the shell has been launched with this button press.

        private TankManager tankManager;

        private void Awake()
        {
            tankManager = GetComponent<TankManager>();
        }

        private void OnEnable()
        {
            // When the tank is turned on, reset the launch force and the UI
            currentLaunchForce = minLaunchForce;
            aimSlider.value = minLaunchForce;
        }

        private void Start()
        {
            // The fire axis is based on the player number.
            fireButton = string.Format("Fire{0}", playerNumber);
            altFireButton = string.Format("AltFire{0}", playerNumber);

            // The rate that the launch force charges up is the range of possible forces by the max charge time.
            chargeSpeed = (maxLaunchForce - minLaunchForce) / maxChargeTime;
        }

        private void Update()
        {
            // The slider should have a default value of the minimum launch force.
            aimSlider.value = minLaunchForce;

            HandleFireCanon();
            HandlePlantMine();
        }

        private void HandleFireCanon()
        {
            // TODO: can shoot?

            // If the max force has been exceeded and the shell hasn't yet been launched...
            if (currentLaunchForce >= maxLaunchForce && !fired)
            {
                // ... use the max force and launch the shell.
                currentLaunchForce = maxLaunchForce;
                FireCanon();
            }
            // Otherwise, if the fire button has just started being pressed...
            else if (Input.GetButtonDown(fireButton))
            {
                // ... reset the fired flag and reset the launch force.
                fired = false;
                currentLaunchForce = minLaunchForce;

                // Change the clip to the charging clip and start it playing.
                shootingAudio.clip = chargingClip;
                shootingAudio.Play();
            }
            // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
            else if (Input.GetButton(fireButton) && !fired)
            {
                // Increment the launch force and update the slider.
                currentLaunchForce += chargeSpeed * Time.deltaTime;

                aimSlider.value = currentLaunchForce;
            }
            // Otherwise, if the fire button is released and the shell hasn't been launched yet...
            else if (Input.GetButtonUp(fireButton) && !fired)
            {
                // ... launch the shell.
                FireCanon();
            }
        }

        private void HandlePlantMine()
        {
            // TODO: can plant mine?

            if (Input.GetButtonDown(altFireButton))
                PlantMine();
        }

        private void FireCanon()
        {
            // Set the fired flag so only Fire is only called once.
            fired = true;

            // Create an instance of the shell and store a reference to it's rigidbody.
            var shellInstance = Instantiate(shellPrefab, fireTransform.position, fireTransform.rotation);

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.GetComponent<Rigidbody>().AddForce(fireTransform.forward * currentLaunchForce, ForceMode.Impulse);

            // Change the clip to the firing clip and play it.
            shootingAudio.clip = fireClip;
            shootingAudio.Play();

            // Reset the launch force.  This is a precaution in case of missing button events.
            currentLaunchForce = minLaunchForce;

            // TODO: tell tank manager that we shot canon
        }

        private void PlantMine()
        {
            Instantiate(armedMinePrefab, transform.position, transform.rotation);

            // TODO: tell tank manager that we have planted mine
        }
    }
}