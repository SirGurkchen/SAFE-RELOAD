using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the logic of an enemy.
/// This includes doing damage, hit registration, and getting damage/death.
/// </summary>
public class EnemyLogic : MonoBehaviour
{
    public event Action<EnemyLogic, int> OnDeath;
    public event Action<int> OnPlayerHit;
    
    private int _health;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _scoreToGive = 50;
    [SerializeField] private int _damageToGive = 1;
    [SerializeField] private int _maxHealth = 2;
    [SerializeField] private float _damageIndicatorTime = 0.1f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource hitSoundSource;
    [SerializeField] private AudioClip hitSound;

    private Color _originalColor;
    private Transform playerPos;

    private void Awake()
    {
        _originalColor = spriteRenderer.color;
    }

    private void FixedUpdate()
    {
        if (playerPos == null)
        {
            return;
        }

        Vector2 dir = (playerPos.position - transform.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        _rb.linearVelocity = dir * _moveSpeed;
    }

    public void ResetEnemy()
    {
        _health = _maxHealth;
        _rb.linearVelocity = Vector2.zero;
    }

    public void RemoveHealth()
    {
        _health--;
        PlayHitSound();
        StartCoroutine(TakeDamageVisual());

        if (_health <= 0)
        {
            AudioManager.Instance.PlayEnemyDieAudio();
            OnDeath?.Invoke(this, _scoreToGive);
        }
    }

    private IEnumerator TakeDamageVisual()
    {
        spriteRenderer.color = new Color(_originalColor.r + 0.2f, _originalColor.g + 0.2f, _originalColor.b + 0.2f, 1f);
        yield return new WaitForSeconds(_damageIndicatorTime);
        spriteRenderer.color = _originalColor;
    }

    public void ActivateEnemy(Transform playerTransform)
    {
        playerPos = playerTransform;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerHit?.Invoke(_damageToGive);
        }
    }

    private void PlayHitSound()
    {
        if (hitSoundSource != null && hitSound != null)
        {
            hitSoundSource.PlayOneShot(hitSound);
        }
    }
}