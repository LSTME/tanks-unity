using System.Text;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WeaponIndication : MonoBehaviour
    {
        public GameManager gameManager;
        public Text weaponText;

        private void Start()
        {
            weaponText.text = string.Empty;
        }

        private void Update()
        {
            if (gameManager.RoundStatus != RoundStatus.Playing)
            {
                weaponText.text = string.Empty;
                return;
            }

            // TODO: print tank status
        }
    }
}