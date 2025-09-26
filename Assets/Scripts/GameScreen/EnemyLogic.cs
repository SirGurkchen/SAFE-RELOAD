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

    private EnemySO data;
    private int _health;
    private Rigidbody2D _rb;
    private int _score;

    [SerializeField] private float _damageIndicatorTime = 0.1f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource hitSoundSource;
    [SerializeField] private AudioClip hitSound;

    private Color _originalColor;
    private Transform playerPos;

    public void Initialize(EnemySO enemyData, Transform playerTransform)
    {
        data = enemyData;
        playerPos = playerTransform;
        _rb = GetComponent<Rigidbody2D>();
        _health = data.maxHealth;
        _originalColor = spriteRenderer.color;
        _score = enemyData.scoreToGive;
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

        _rb.linearVelocity = dir * data.moveSpeed;
    }


    public void ResetEnemy()
    {
        _health = data.maxHealth;
        _rb.linearVelocity = Vector2.zero;
    }

    public void RemoveHealth()
    {
        _health--;
        PlayHitSound();
        if (_health > 0)
        {
            StartCoroutine(TakeDamageVisual());
        }

        if (_health <= 0)
        {
            AudioManager.Instance.PlayEnemyDieAudio();
            OnDeath?.Invoke(this, _score);
        }
    }

    private IEnumerator TakeDamageVisual()
    {
        spriteRenderer.color = new Color(_originalColor.r + 0.2f, _originalColor.g + 0.2f, _originalColor.b + 0.2f, 1f);
        yield return new WaitForSeconds(_damageIndicatorTime);
        spriteRenderer.color = _originalColor;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPlayerHit?.Invoke(data.damageToGive);
        }
    }

    private void PlayHitSound()
    {
        if (hitSoundSource != null && hitSound != null && _health > 0)
        {
            hitSoundSource.PlayOneShot(hitSound);
        }
    }

    public EnemySO GetData()
    {
        return data;
    }
}