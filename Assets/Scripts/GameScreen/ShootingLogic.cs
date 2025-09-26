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
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private WeaponSO[] _weaponData;
    [SerializeField] private float _spawnOffset = 0.25f;


    private int _currentBullets;
    private int _maxBullets;
    private int _weapon1Bullets;
    private int _weapon2Bullets;
    private float _reloadThreshhold;
    private WeaponSO _selectedWeapon;

    private const float RELOAD_TIME_MULTIPLIER = 0.1f;

    private enum State { Shooting, Reloading }
    private State currentState = State.Shooting;

    private void Start()
    {
        _weapon1Bullets = _weaponData[0].maxAmmo;
        _weapon2Bullets = _weaponData[1].maxAmmo;
        _maxBullets = _weapon1Bullets;
        _currentBullets = _maxBullets;
        _reloadThreshhold = _weaponData[0].reloadThreshhold;
        _selectedWeapon = _weaponData[0];
        _uiManager.RefreshAmmo(_maxBullets, _currentBullets);
        _uiManager.SetWeaponText(_selectedWeapon.weaponName);
        GameInput.Instance.OnWeaponSwitch += Instance_OnWeaponSwitch;
        player.OnShoot += Player_OnShoot;
        player.OnReload += Player_OnReload;
    }

    private void Instance_OnWeaponSwitch(int obj)
    {
        if (obj == 0 && _selectedWeapon != _weaponData[0] && currentState != State.Reloading)
        {
            _weapon2Bullets = _currentBullets;
            _maxBullets = _weaponData[obj].maxAmmo;
            _currentBullets = _weapon1Bullets;
            _reloadThreshhold = _weaponData[0].reloadThreshhold;
            _selectedWeapon = _weaponData[obj];
        }
        else if (obj == 1 && _selectedWeapon != _weaponData[1] && currentState != State.Reloading)
        {
            _weapon1Bullets = _currentBullets;
            _maxBullets = _weaponData[obj].maxAmmo;
            _currentBullets = _weapon2Bullets;
            _reloadThreshhold = _weaponData[1].reloadThreshhold;
            _selectedWeapon = _weaponData[obj];
        }
        _uiManager.RefreshAmmo(_maxBullets, _currentBullets);
        _uiManager.SetWeaponText(_selectedWeapon.weaponName);
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
        if (_currentBullets > 0 && currentState == State.Shooting)
        {
            _currentBullets--;

            AudioManager.Instance.PlaySound(AudioManager.SoundType.Shoot);
            player.PlayMuzzleFlash();

            if (_selectedWeapon == _weaponData[0])
            {
                ShootPistol(spawn);
            }
            else if (_selectedWeapon == _weaponData[1])
            {
                ShootShotgun(spawn);
            }
            OnShootAction?.Invoke(_currentBullets);
        }
        else if (currentState == State.Shooting && _currentBullets <= 0) 
        {
            AudioManager.Instance.PlaySound(AudioManager.SoundType.GunEmpty);
        }
    }

    private void ShootPistol(Transform spawn)
    {
        GameObject newBullet = BulletPool.Instance.GetBullet();
        newBullet.transform.position = spawn.position;
        newBullet.transform.rotation = player.transform.rotation;

        BulletLogic newBulletLogic = newBullet.GetComponent<BulletLogic>();
        GameLogic.Instance.SubScribePistolBullet(newBulletLogic);
    }

    private void ShootShotgun(Transform spawn)
    {
        GameObject newBullet = BulletPool.Instance.GetBullet();
        GameObject newBullet2 = BulletPool.Instance.GetBullet();
        GameObject newBullet3 = BulletPool.Instance.GetBullet();

        newBullet.transform.position = spawn.position;
        newBullet.transform.rotation = player.transform.rotation;

        newBullet2.transform.position = spawn.position - player.transform.right * _spawnOffset;
        newBullet2.transform.rotation = player.transform.rotation * Quaternion.Euler(0, 0, 45);

        newBullet3.transform.position = spawn.position + player.transform.right * _spawnOffset;
        newBullet3.transform.rotation = player.transform.rotation * Quaternion.Euler(0, 0, -45);

        BulletLogic newBulletLogic = newBullet.GetComponent<BulletLogic>();
        BulletLogic newBulletLogic2 = newBullet2.GetComponent<BulletLogic>();
        BulletLogic newBulletLogic3 = newBullet3.GetComponent<BulletLogic>();

        GameLogic.Instance.SubscribeShotgunBullets(newBulletLogic, newBulletLogic2, newBulletLogic3);
    }

    private IEnumerator ReloadLoop()
    {
        player.SetPlayerSpeed(moveSpeedBonus);

        while (_currentBullets != _maxBullets)
        {
            PlayReload();

            currentState = State.Reloading;

            _currentBullets++;

            OnReloadAction?.Invoke();

            reloadTimeInterval = (_currentBullets + _reloadThreshhold) * RELOAD_TIME_MULTIPLIER;

            if (_currentBullets == _maxBullets)
            {
                break;
            }

            yield return new WaitForSeconds(reloadTimeInterval);
        }
        currentState = State.Shooting;
        player.SetPlayerSpeed(3);
    }

    public int GetMaxAmmo()
    {
        return _maxBullets;
    }

    public int GetCurrentAmmo()
    {
        return _currentBullets;
    }

    private void PlayReload()
    {
        if (_currentBullets == _maxBullets - 1)
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
        GameInput.Instance.OnWeaponSwitch -= Instance_OnWeaponSwitch;
    }
}