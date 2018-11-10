using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIndication : MonoBehaviour
{
    public GameManager gameManager;
    public Text weaponText;

    void Start()
    {
        weaponText.text = string.Empty;
    }

    void Update()
    {
        if (gameManager.RoundStatus != RoundStatus.Playing)
        {
            weaponText.text = string.Empty;
            return;
        }

        var stringBuilder = new StringBuilder();
        foreach (var tankManager in gameManager.m_Tanks)
            stringBuilder.AppendLine(tankManager.m_ColoredPlayerText + " A" + tankManager.ammo + " M" + tankManager.mines);
        weaponText.text = stringBuilder.ToString();
    }
}