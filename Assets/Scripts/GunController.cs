using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject gunShotparticles;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private LayerMask damagableLayer;


    [Header("Parameters")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private int maxMagazines = 1;
    [SerializeField] private float shootDelay = 0.3f;
    [SerializeField] private float reloadDelay = 0.7f;
    [SerializeField] private int damage = 1;

    private PlayerController _player;
    private bool _canInteract;

    private int _currentAmmo;
    private int _currentMagazines;
    private float _shootTimer;
    private float _reloadTimer;
    private bool _isReloading;

    private void Start()
    {
        _player = GameManager.Instance.PlayerController;
        _currentAmmo = maxAmmo;
        _currentMagazines = maxMagazines;
        _shootTimer = 0;
        _reloadTimer = 0;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Interact") && _canInteract)
            Interact();

        _shootTimer -= Time.deltaTime;
        if (_isReloading)
        {
            _reloadTimer -= Time.deltaTime;
            if (_reloadTimer <= 0)
            {
                Debug.Log("Stopped reloading");
                _isReloading = false;
                _currentMagazines--;
                _currentAmmo = maxAmmo;
                UpdateAmmoUI();
            }
        }
    }

    private void OnDestroy()
    {
        ResetAmmoUI();
    }

    private void ResetAmmoUI()
    {
        UIManager.instance.UpdateAmmo(0, 0);
    }

    private void UpdateAmmoUI()
    {
        UIManager.instance.UpdateAmmo(_currentAmmo, _currentMagazines * maxAmmo);
    }

    public void Shoot()
    {
        if (_currentAmmo > 0)
        {
            if (_shootTimer <= 0)
            {
                _shootTimer = shootDelay;
                _currentAmmo--;
                UpdateAmmoUI();
                Instantiate(gunShotparticles, muzzle.position, muzzle.rotation);
                var hit = Physics2D.Raycast(muzzle.position, muzzle.right, 100, damagableLayer);

                if (hit.collider != null)
                {
                    var damagable = hit.collider.GetComponent<IDamagable>();
                    if (damagable != null)
                        damagable.TakeDamage(damage);

                    var bullet = Instantiate(bulletPrefab, new Vector2(0, 0), Quaternion.Euler(0, 0, 0));
                    bullet.GetComponent<LineRenderer>().SetPosition(0, muzzle.position);
                    bullet.GetComponent<LineRenderer>().SetPosition(1, hit.point);
                    Destroy(bullet, 0.05f);
                    Instantiate(gunShotparticles, hit.point, Quaternion.FromToRotation(Vector2.right, hit.normal) * Quaternion.identity);
                }
                else
                {
                    var bullet = Instantiate(bulletPrefab, new Vector2(0, 0), Quaternion.Euler(0, 0, 0));
                    bullet.GetComponent<LineRenderer>().SetPosition(0, muzzle.position);
                    bullet.GetComponent<LineRenderer>().SetPosition(1, muzzle.right * 30 + muzzle.position);
                    Destroy(bullet, 0.05f);
                }
            }
        }
        else
        {
            if (_currentMagazines > 0 && !_isReloading)
            {
                Debug.Log("Started reloading");
                _isReloading = true;
                _reloadTimer = reloadDelay;
            }
            else if (_currentMagazines <= 0)
            {
                Debug.Log("Error: No ammo left");
            }
        }
    }

    public void ReplenishAmmo(int amount)
    {
        _currentMagazines += amount / maxAmmo;
        UpdateAmmoUI();
    }

    #region Interaction

    public void Interact()
    {
        _player.TakeGun(this);
        UpdateAmmoUI();
    }


    public void ShowHelp()
    {
        Debug.Log("Press E to interact");
    }

    public void HideHelp()
    {
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ShowHelp();
            _canInteract = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HideHelp();
            _canInteract = false;
        }
    }

    #endregion
}
