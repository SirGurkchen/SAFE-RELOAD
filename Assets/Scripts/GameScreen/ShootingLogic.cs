using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the shooting logic of the game.
/// This includes shooting and reloading.
/// </summary>
public class ShootingLogic : MonoBehaviour
{
    public event Action<int> OnShootAction;
    public event Action OnReloadAction;

    [SerializeField] private Player player;
    [SerializeField] private float reloadTimeInterval = 0.5f;
    [SerializeField] private int moveSpeedBonus = 7;

    private int currentBullets = 7;
    private int maxBullets = 7;

    private const float RELOAD_THRESHHOLD = 0.75f;
    private const float RELOAD_TIME_MULTIPLIER = 0.1f;

    private enum State { Shooting, Reloading }
    private State currentState = State.Shooting;

    private void Start()
    {
        player.OnShoot += Player_OnShoot;
        player.OnReload += Player_OnReload;
    }

    private void Player_OnReload()
    {
        if (currentState != State.Reloading)
        {
            StartCoroutine(ReloadLoop());
        }
    }

    /// <summary>
    /// Generates a bullet at the given spawn point.
    /// Plays thee shooting audio and particles.
    /// If player has no bullet, only plays gun empty audio.
    /// </summary>
    /// <param name="spawn">The spawn point</param>
    private void Player_OnShoot(Transform spawn)
    {
        if (currentBullets > 0 && currentState == State.Shooting)
        {
            currentBullets--;

            AudioManager.Instance.PlaySound(AudioManager.SoundType.Shoot);
            player.PlayMuzzleFlash();

            GameObject newBullet = BulletPool.Instance.GetBullet();
            newBullet.transform.position = spawn.position;
            newBullet.transform.rotation = player.transform.rotation;

            BulletLogic newBulletLogic = newBullet.GetComponent<BulletLogic>();
            GameLogic.Instance.SubScribeBullet(newBulletLogic);

            OnShootAction?.Invoke(currentBullets);
        }
        else if (currentState == State.Shooting && currentBullets <= 0) 
        {
            AudioManager.Instance.PlaySound(AudioManager.SoundType.GunEmpty);
        }
    }

    private IEnumerator ReloadLoop()
    {
        player.SetPlayerSpeed(moveSpeedBonus);

        while (currentBullets != maxBullets)
        {
            PlayReload();

            currentState = State.Reloading;

            currentBullets++;

            OnReloadAction?.Invoke();

            reloadTimeInterval = (currentBullets + RELOAD_THRESHHOLD) * RELOAD_TIME_MULTIPLIER;

            yield return new WaitForSeconds(reloadTimeInterval);
        }
        currentState = State.Shooting;
        player.SetPlayerSpeed(3);
    }

    public int GetMaxAmmo()
    {
        return maxBullets;
    }

    public int GetCurrentAmmo()
    {
        return currentBullets;
    }

    private void PlayReload()
    {
        if (currentBullets == maxBullets - 1)
        {
            AudioManager.Instance.PlaySound(AudioManager.SoundType.ReloadFinished);
        }
        else
        {
            AudioManager.Instance.PlaySound(AudioManager.SoundType.Reload);
        }
    }

    public void Unsubscribe()
    {
        player.OnShoot -= Player_OnShoot;
        player.OnReload -= Player_OnReload;
    }
}