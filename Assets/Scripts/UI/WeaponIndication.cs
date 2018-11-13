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

            var stringBuilder = new StringBuilder();
            foreach (var tankManager in gameManager.tankManagers)
            {
                stringBuilder.AppendFormat("{0} A{1} M{2}", tankManager.coloredPlayerText, tankManager.ammo, tankManager.mines);
                stringBuilder.AppendLine();
            }

            weaponText.text = stringBuilder.ToString();
        }
    }
}