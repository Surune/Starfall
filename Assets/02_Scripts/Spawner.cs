using UnityEngine;
using TMPro;
using Starfall.Manager;
using Starfall.Entity;
using Starfall.Constants;

public class Spawner : MonoBehaviour {
    #region Manager
    private static GameStateManager gameStateManager => GameManager.Instance.gameStateManager;
    private static PoolManager poolManager => GameManager.Instance.PoolManager;
    #endregion

    public bool spawnEnabled = true;
    [SerializeField] private float boundary;
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private GameObject BossPrefab;
    [SerializeField] private int EnemyTypeNum;
    public TextMeshProUGUI resourceText;
    public float enemydelay;
    public int enemynum = 1;
    public float speedCoefficient = 1f;
    public GameObject EnemyList;
    public GameObject MeteorList;
    private Timer timer;
    private float maxX, maxY;
    private const float mindelay = 0.005f;
    private static float[] speedList = {1.25f, 2f, 7f, 1.25f, 1.25f, 1.5f, 2f};
    public float damageCoefficient = 1f;
    public float meteorCoefficient = 1f;
    [HideInInspector] public bool disabled = false;
    [HideInInspector] public bool makeMeteor = false;
    [HideInInspector] public bool spawnSmall = false;
    [HideInInspector] public bool spawnRandom = false;
    public float addHP = 0f;
    public AudioSource _musicplayer;
    public AudioClip sfxMeteor;

    public Color[] colorList;

    void Start() {
        InvokeRepeating("SpawnEnemy", 0, enemydelay);
        EnemyList = GameObject.Find("Enemies");
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        maxX = EnemyList.GetComponent<RectTransform>().rect.width/2 * boundary;
        maxY = EnemyList.GetComponent<RectTransform>().rect.height/2;

        speedCoefficient = 1f;
        
        GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }
    
    void OnDestroy() {
        GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }

    public void ChangeSpawnDelay(float newcooltime) {
        if (newcooltime < mindelay) newcooltime = mindelay;
        enemydelay = newcooltime;
        CancelInvoke("SpawnEnemy");
        InvokeRepeating("SpawnEnemy", 0, enemydelay);
    }

    public GameObject SpawnMeteor() {
        if (!disabled) {
            _musicplayer.PlayOneShot(sfxMeteor);
            var meteor = poolManager.Get(PoolNumber.Meteor);
            meteor.transform.position = new Vector3(GameManager.Instance.Player.transform.position.x, maxY, 0f);
            meteor.GetComponent<Meteor>().speed *= meteorCoefficient;
            return meteor;
        }
        else
            return null;
    }

    public GameObject SpawnFinalBoss() {
        foreach(Transform t in GameManager.GetAllChilds(EnemyList.transform)){
            t.gameObject.SetActive(false);
        }
        var enemy = Instantiate(BossPrefab, new Vector3(0f, maxY, 0f), Quaternion.identity);
        enemy.transform.SetParent(EnemyList.transform);
        GameManager.Instance.activeEnemyNum += 1;
        return enemy;
    }

    public Enemy SpawnEnemyWithType(int type, Vector3 pos) {
        var enemy = poolManager.Get(PoolNumber.Enemy);
        enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().color = colorList[type];
        enemy.transform.position = pos;
        if(!spawnSmall)
            enemy.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        else
            enemy.transform.localScale = new Vector3(1f, 1f, 1f);
        
        var e = enemy.GetComponent<Enemy>();
        e.maxspeed = speedList[type] * speedCoefficient;
        e.SetType(type);
        return e;
    }

    public void SpawnItem() {
        var item = poolManager.Get(PoolNumber.Item);
        item.transform.position = new Vector3(Random.Range(-maxX, maxX), maxY, 0f);
        item.GetComponent<DropItem>().SetType(Random.Range(0, 4));
    }

    private void SpawnEnemy() {
        if (!gameStateManager.IsPlaying)
            return;

        int ran = 0;
        if (!spawnRandom) {
            ran = Random.Range(0, timer.waveNum < EnemyTypeNum ? timer.waveNum : EnemyTypeNum);
        }
        else {
            ran = Random.Range(0, EnemyTypeNum);
        }
        
        var enemy = SpawnEnemyWithType(ran, new Vector3(Random.Range(-maxX, maxX), maxY, 0f));
        if (timer.roundNum % timer.bossPerWave != 0) {
            enemy.isBoss = false;
            enemy.expAmount = 1;
        }
        else {
            enemy.MakeBoss();
            enemy.expAmount = timer.waveNum + 1;
        }
        enemy.makeMeteor = makeMeteor;
        enemy.maxHP = enemy.maxHP + addHP > 1 ? enemy.maxHP + addHP : 1f;
        enemy.currentHP = enemy.maxHP;
        enemy.SetHPText();
        enemynum -= 1;
        GameManager.Instance.activeEnemyNum++;
    }

    private void OnGameStateChanged(GameState newGameState) {
        spawnEnabled = (newGameState == GameState.Gameplay);
    }
}