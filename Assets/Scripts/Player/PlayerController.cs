using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float acceleration;
    [SerializeField] private float airFriction;
    [SerializeField] private float airControl;
    [SerializeField] private float jumpHeight;

    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;

    [Header("Shooting")]
    [SerializeField] private Transform gunHolder;
    [SerializeField] private Transform gunPivot;

    private Rigidbody2D _rb;

    private Vector2 _velocity;
    private int _moveDir;
    private bool _isGrounded;
    private Vector2 _groundCheckPosition;

    public GunController CurrentGun { get; private set; } = null;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
        CheckCollisions();
        RotateGun();
    }

    private void GetInput()
    {
        _moveDir = (int)Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            Jump();

        if (Input.GetButton("Shoot"))
            Shoot();

        if (Input.GetButtonDown("Drop"))
            DropGun();
    }

    private void Shoot()
    {
        if (CurrentGun != null)
            CurrentGun.Shoot();
    }

    public void ReplenishAmmo(int amount, GameObject ammoObject)
    {
        if (CurrentGun != null)
        {
            CurrentGun.ReplenishAmmo(amount);
            Destroy(ammoObject);
        }
    }

    public void TakeGun(GunController gun)
    {
        if (CurrentGun == null)
        {
            gun.GetComponent<BoxCollider2D>().enabled = false;
            CurrentGun = gun;
            gun.transform.parent = gunHolder;
            gun.transform.localPosition = Vector2.zero;
            gun.transform.localRotation = Quaternion.identity;
        }
        else
        {
            DropGun();
            gun.GetComponent<BoxCollider2D>().enabled = false;
            CurrentGun = gun;
            gun.transform.parent = gunHolder;
            gun.transform.localPosition = Vector2.zero;
            gun.transform.localRotation = Quaternion.identity;
        }
    }

    public void DropGun()
    {
        if (CurrentGun != null)
        {
            Destroy(CurrentGun.gameObject);
            CurrentGun = null;
        }
    }

    private void RotateGun()
    {
        var mousePosition = Input.mousePosition;
        var myPosition = Camera.main.WorldToScreenPoint(gunPivot.position);
        var angle = Mathf.Atan2(mousePosition.y - myPosition.y, mousePosition.x - myPosition.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void CheckCollisions()
    {
        _groundCheckPosition = new Vector2(transform.position.x, transform.position.y - GetComponent<Collider2D>().bounds.size.y / 2);
        _isGrounded = Physics2D.OverlapBox(_groundCheckPosition, groundCheckSize, 0, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _isGrounded ? Color.red : Color.green;
        Gizmos.DrawWireCube(_groundCheckPosition, groundCheckSize);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (_isGrounded)
        {
            if (_moveDir != 0)
            {
                _velocity.x += _moveDir * acceleration;
                _velocity.x = Mathf.Clamp(_velocity.x, -maxSpeed, maxSpeed);
            }
            else
            {
                _velocity.x = Mathf.Lerp(_velocity.x, 0, friction);
            }
        }
        else
        {
            if (_moveDir != 0)
            {
                _velocity.x += _moveDir * airControl;
                _velocity.x = Mathf.Clamp(_velocity.x, -maxSpeed, maxSpeed);
            }
            else
            {
                _velocity.x = Mathf.Lerp(_velocity.x, 0, airFriction);
            }
        }

        _velocity.y = _rb.velocity.y;

        if (_velocity.y < 0)
        {
            _velocity.y += Physics2D.gravity.y * 3.5f * Time.deltaTime;
        }
        else if (_velocity.y > 0 && !Input.GetButton("Jump"))
        {
            _velocity.y += Physics2D.gravity.y * 3 * Time.deltaTime;
        }
        _rb.velocity = _velocity;
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _velocity.y += jumpHeight;
            _rb.velocity = _velocity;
        }
    }
}
