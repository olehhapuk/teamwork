using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour, IAutoInteractable
{
    public int AmmoAmount;

    private PlayerController _player;

    private void Start()
    {
        _player = GameManager.Instance.PlayerController;
    }

    public void Interact()
    {
        _player.ReplenishAmmo(AmmoAmount, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            Interact();
    }
}
