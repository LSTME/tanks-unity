using System;
using Pickups;
using Tank;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(TankMovement), typeof(TankShooting))]
    public class TankManager : MonoBehaviour
    {
        public Color playerColor;
        public Transform spawnPoint;
        public int playerNumber;
        public string coloredPlayerText;
        public int wins;
        public int ammo = 5;
        public int mines;

        private TankMovement movement;
        private TankShooting shooting;
        private GameObject canvasGameObject;


        public void Setup()
        {
            movement = GetComponent<TankMovement>();
            shooting = GetComponent<TankShooting>();
            canvasGameObject = GetComponentInChildren<Canvas>().gameObject;

            movement.playerNumber = playerNumber;
            shooting.playerNumber = playerNumber;

            coloredPlayerText = string.Format("<color=#{0}>PLAYER {1}</color>", ColorUtility.ToHtmlStringRGB(playerColor), playerNumber);

            var renderers = GetComponentsInChildren<MeshRenderer>();

            foreach (var tankRenderer in renderers)
            {
                tankRenderer.material.color = playerColor;
            }
        }

        public void DisableControl()
        {
            movement.enabled = false;
            shooting.enabled = false;

            canvasGameObject.SetActive(false);
        }

        public void EnableControl()
        {
            movement.enabled = true;
            shooting.enabled = true;

            canvasGameObject.SetActive(true);
        }

        public void Reset()
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public bool CanShootCanon()
        {
            return ammo > 0;
        }

        public bool CanPlantMine()
        {
            return mines > 0;
        }

        public void OnCanonFired()
        {
            ammo--;
        }

        public void OnMinePlanted()
        {
            mines--;
        }

        public bool Pickup(PickupType pickupType)
        {
            switch (pickupType)
            {
                case PickupType.Ammo:
                    ammo += 5;
                    return true;
                case PickupType.Shield:
                    return GetComponent<TankHealth>().ActivateShield();
                case PickupType.Mines:
                    mines += 3;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException("pickupType", pickupType, null);
            }
        }
    }
}