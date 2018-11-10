using System;
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

        weaponText.text = DateTime.Now.ToLongTimeString();
    }
}