using System;
using UnityEngine;

/// <summary>
/// Handles the Bullet Logic.
/// This includes movement and hit registration.
/// </summary>
public class BulletLogic : MonoBehaviour
{
    public event Action<GameObject, GameObject> OnObjectHit;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5;

    public void moveBullet(Vector2 dir)
    {
        rb.linearVelocity = dir * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnObjectHit?.Invoke(collision.gameObject, this.gameObject);
    }
}