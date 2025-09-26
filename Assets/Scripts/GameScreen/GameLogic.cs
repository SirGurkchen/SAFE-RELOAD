using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// Handles the general Game Logic of the game.
/// </summary>
public class GameLogic : MonoBehaviour
{
    private static GameLogic instance;

    [SerializeField] private EnemySpawn spawner;
    [SerializeField] private ShootingLogic shootLogic;
    [SerializeField] private UIManager gameUI;
    [SerializeField] private Player player;
    [SerializeField] private int bulletDamage = 1;

    private int playerScore = 0;
    private int highScore = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            spawner = FindAnyObjectByType<EnemySpawn>();
            shootLogic = FindAnyObjectByType<ShootingLogic>();
            gameUI = FindAnyObjectByType<UIManager>();
            player = FindAnyObjectByType<Player>();
            GameInput.Instance = FindAnyObjectByType<GameInput>();

            shootLogic.OnReloadAction += ShootLogic_OnReloadAction;
            shootLogic.OnShootAction += ShootLogic_OnShootAction;
            spawner.OnEnemySpawn += Spawner_OnEnemySpawn;
            player.OnPlayerDeath += Player_OnPlayerDeath;
        }
    }

    public static GameLogic Instance => instance;

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        shootLogic.OnReloadAction += ShootLogic_OnReloadAction;
        shootLogic.OnShootAction += ShootLogic_OnShootAction;
        spawner.OnEnemySpawn += Spawner_OnEnemySpawn;
        player.OnPlayerDeath += Player_OnPlayerDeath;
    }

    private void Player_OnPlayerDeath()
    {
        if (playerScore > highScore)
        {
            highScore = playerScore;
        }

        playerScore = 0;

        player.ResetPlayer();
        shootLogic.Unsubscribe();
        GameInput.Instance.DisableInput();

        shootLogic.OnReloadAction -= ShootLogic_OnReloadAction;
        shootLogic.OnShootAction -= ShootLogic_OnShootAction;
        spawner.OnEnemySpawn -= Spawner_OnEnemySpawn;
        player.OnPlayerDeath -= Player_OnPlayerDeath;

        AudioManager.Instance.DisableAudio();
        SceneManager.LoadScene("StartMenuScene");
    }

    private void Spawner_OnEnemySpawn(EnemyLogic logic)
    {
        logic.ResetEnemy();

        logic.OnDeath -= Logic_OnDeath;
        logic.OnPlayerHit -= Logic_OnPlayerHit;

        logic.OnDeath += Logic_OnDeath;
        logic.OnPlayerHit += Logic_OnPlayerHit;
    }

    private void Logic_OnPlayerHit(int damage)
    {
        player.TakeDamage(damage);
        gameUI.UpdateHealthBar((float)player.GetCurrentHealth() / (float)player.GetMaxHealth());
    }

    private void Logic_OnDeath(EnemyLogic obj, int score)
    {
        obj.OnDeath -= Logic_OnDeath;
        obj.OnPlayerHit -= Logic_OnPlayerHit;
        EnemyPool.Instance.ReturnEnemy(obj.GetData(), obj.gameObject);
        EnemyPool.Instance.RemoveEnemyFromList(obj.gameObject);
        playerScore += score;
        gameUI.UpdateScore(playerScore);
    }

    private void ShootLogic_OnShootAction(int currentAmmo)
    {
        gameUI.RefreshAmmo(shootLogic.GetMaxAmmo(), currentAmmo);
    }

    private void ShootLogic_OnReloadAction()
    {
        gameUI.RefreshAmmo(shootLogic.GetMaxAmmo(), shootLogic.GetCurrentAmmo());
    }

    public void SubScribePistolBullet(BulletLogic bulletLogic)
    {
        bulletLogic.moveBullet(player.transform.up);

        bulletLogic.OnObjectHit += NewBulletLogic_OnObjectHit;
    }

    public void SubscribeShotgunBullets(BulletLogic bulletLogic, BulletLogic bulletLogic2, BulletLogic bulletLogic3)
    {
        Vector3 leftOffset = player.transform.rotation * new Vector3(-1, 1, 0).normalized;
        Vector3 rightOffset = player.transform.rotation * new Vector3(1, 1, 0).normalized;

        bulletLogic.moveBullet(player.transform.up);
        bulletLogic2.moveBullet(leftOffset);
        bulletLogic3.moveBullet(rightOffset);

        bulletLogic.OnObjectHit += NewBulletLogic_OnObjectHit;
        bulletLogic2.OnObjectHit += NewBulletLogic_OnObjectHit;
        bulletLogic3.OnObjectHit += NewBulletLogic_OnObjectHit;
    }

    public void SubscribeEnemyBullet(BulletLogic bulletLogic, GameObject enemy)
    {
        bulletLogic.moveBullet(enemy.transform.up);

        bulletLogic.OnObjectHit += NewBulletLogic_OnObjectHit;
    }

    private void NewBulletLogic_OnObjectHit(GameObject obj, GameObject bullet)
    {
        bullet.GetComponent<BulletLogic>().OnObjectHit -= NewBulletLogic_OnObjectHit;
        BulletPool.Instance.ReturnBullet(bullet);

        if (obj.tag.Contains("Enemy"))
        {
            obj.GetComponent<EnemyLogic>().RemoveHealth();
        }
        else if (obj.tag.Contains("Player"))
        {
            player.TakeDamage(bulletDamage);
            gameUI.UpdateHealthBar((float)player.GetCurrentHealth() / (float)player.GetMaxHealth());
        }
    }

    public int GetHighScore()
    {
        return highScore;
    }
}