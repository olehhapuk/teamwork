using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private TextMeshProUGUI ammoText;

    private void Awake()
    {
        instance = this;
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        ammoText.text = "Ammo " + currentAmmo + '/' + maxAmmo;
    }

    public void UpdateEnergy(int max, int current)
    {
        Debug.Log($"Energy: {current / max}");
    }
}
