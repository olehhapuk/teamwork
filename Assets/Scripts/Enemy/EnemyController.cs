using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour, IDamagable
{
    enum State
    {
        IDLE,
        FOLLOWING,
        ATTACKING,
        TAKING_DAMAGE
    }

    [SerializeField] private int maxHealth;

    [Header("Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float friction;
    [SerializeField] private float acceleration;
    [SerializeField] private float airFriction;
    [SerializeField] private float airControl;
    [SerializeField] private float jumpHeight;

    [Space]
    [SerializeField] private float searchRange;
    [SerializeField] private float megaSearchRange;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask playerLayer;

    private int _currentHealth;
    private int _moveDir;
    private int _lookDir;
    private Vector2 _velocity;
    private bool _playerFound;
    private bool _inAttackRange;
    private float _currentSearchRange;

    private Rigidbody2D _rb;
    private PlayerController _player;
    private State _state;

    public void Die()
    {
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _state = State.TAKING_DAMAGE;
        StopCoroutine(StopTakingDamage());
        StartCoroutine(StopTakingDamage());
        _velocity.x = -10 * _lookDir;
        _rb.velocity = _velocity;
        if (_currentHealth <= 0)
            Die();
    }

    private IEnumerator StopTakingDamage()
    {
        yield return new WaitForSeconds(1);
        _state = State.IDLE;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _player = GameManager.Instance.PlayerController;
        _currentHealth = maxHealth;
        _state = State.IDLE;
        _currentSearchRange = searchRange;
    }

    private void Update()
    {
        CheckCollisions();
        switch (_state)
        {
            case State.IDLE:
                if (_playerFound)
                    _state = State.FOLLOWING;
                break;
            case State.FOLLOWING:
                if (_player.transform.position.x < transform.position.x)
                    _moveDir = -1;
                else if (_player.transform.position.x > transform.position.x)
                    _moveDir = 1;
                else
                    _moveDir = 0;

                if (_inAttackRange)
                    _state = State.ATTACKING;
                else if (!_playerFound)
                    _state = State.IDLE;

                break;
            case State.ATTACKING:
                if (!_inAttackRange)
                    _state = State.IDLE;
                break;
            case State.TAKING_DAMAGE:
                _currentSearchRange = megaSearchRange;
                StopCoroutine(ResetSearchRange());
                StartCoroutine(ResetSearchRange());
                break;
        }

        if (_state != State.FOLLOWING)
            _moveDir = 0;

        if (_player.transform.position.x < transform.position.x)
            _lookDir = -1;
        else if (_player.transform.position.x > transform.position.x)
            _lookDir = 1;
    }

    private IEnumerator ResetSearchRange()
    {
        yield return new WaitForSeconds(7);
        _currentSearchRange = searchRange;
    }

    private void CheckCollisions()
    {
        _playerFound = Physics2D.OverlapCircle(transform.position, _currentSearchRange, playerLayer);
        _inAttackRange = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _playerFound ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, _currentSearchRange);

        Gizmos.color = _inAttackRange ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void Move()
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

        //if (_moveDir != 0)
        //{
        //    _velocity.x += _moveDir * airControl;
        //    _velocity.x = Mathf.Clamp(_velocity.x, -maxSpeed, maxSpeed);
        //}
        //else
        //{
        //    _velocity.x = Mathf.Lerp(_velocity.x, 0, airFriction);
        //}

        _velocity.y = _rb.velocity.y;

        //if (_velocity.y < 0)
        //{
        //    _velocity.y += Physics2D.gravity.y * 3.5f * Time.deltaTime;
        //}
        //else if (_velocity.y > 0 && !Input.GetButton("Jump"))
        //{
        //    _velocity.y += Physics2D.gravity.y * 3 * Time.deltaTime;
        //}
        _rb.velocity = _velocity;
    }
}
