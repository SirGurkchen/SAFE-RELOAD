using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player logic of the game
/// </summary>
public class Player : MonoBehaviour
{
    public event Action<Transform> OnShoot;
    public event Action OnReload;
    public event Action OnPlayerDeath;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private GameObject bulletSpawnPoint;
    [SerializeField] private int maxHealth = 6;

    private int currentHealth;

    private void Start()
    {
        gameObject.SetActive(true);
        currentHealth = maxHealth;
    }

    private void FixedUpdate()
    {
        Vector2 vel = GameInput.Instance.PlayerMovementNormalized() * moveSpeed;
        rb.linearVelocity = vel;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (worldPos - transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
    }

    private void Update()
    {
        bool isShooting = GameInput.Instance.PlayerShoot();
        bool isReloading = GameInput.Instance.PlayerReload();

        AudioManager.Instance.PlayWalkAudio(GameInput.Instance.PlayerMovementNormalized() != Vector2.zero);

        if (isShooting)
        {
            OnShoot?.Invoke(bulletSpawnPoint.transform);
        }
        else if (isReloading)
        {
            OnReload?.Invoke();
        }
    }

    /// <summary>
    /// Reduces the player's health by the given damage.
    /// </summary>
    /// <param name="damage">Number of damage taken</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0 )
        {
            OnPlayerDeath?.Invoke();
        }
    }

    /// <summary>
    /// Sets the play's speed by the given speed
    /// </summary>
    /// <param name="speed">New player speed</param>
    public void SetPlayerSpeed(float speed)
    {
        moveSpeed = speed;
    }

    /// <summary>
    /// Resets the player Game Object
    /// </summary>
    public void ResetPlayer()
    {
        this.gameObject.SetActive(false);
        this.gameObject.transform.position = Vector2.zero;
        moveSpeed = 3;
        maxHealth = 6;
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Gives the current player health amount.
    /// </summary>
    /// <returns>Current player health</returns>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Gives the current player max health amount.
    /// </summary>
    /// <returns>Current player max health</returns>
    public int GetMaxHealth()
    {
        return maxHealth;
    }

    /// <summary>
    /// Shows the shooting Particles.
    /// </summary>
    public void PlayMuzzleFlash()
    {
        bulletSpawnPoint.GetComponent<ParticleSystem>().Play();
    }
}