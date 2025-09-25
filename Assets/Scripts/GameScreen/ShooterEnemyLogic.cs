using System;
using System.Collections;
using UnityEngine;

public class ShooterEnemyLogic : EnemyLogic
{
    [SerializeField] private Transform _muzzlePoint;
    [SerializeField] private float _shootingInterval;

    private void Start()
    {
        StartCoroutine(ShootingAction());
    }

    private IEnumerator ShootingAction()
    {
        while (true)
        {
            yield return new WaitForSeconds(_shootingInterval);

            AudioManager.Instance.PlayEnemyShoot(AudioManager.SoundType.Shoot);
            PlayMuzzleFlash();

            GameObject newBullet = BulletPool.Instance.GetBullet();
            newBullet.transform.position = _muzzlePoint.position;
            newBullet.transform.rotation = this.transform.rotation;

            BulletLogic newBulletLogic = newBullet.GetComponent<BulletLogic>();
            GameLogic.Instance.SubscribeEnemyBullet(newBulletLogic, this.gameObject);
        }
    }

    public void PlayMuzzleFlash()
    {
        _muzzlePoint.GetComponent<ParticleSystem>().Play();
    }
}