using UnityEngine;
using TMPro;
using Starfall.Manager;
using Starfall.Entity;
using Starfall.Constants;

public class Spawner : MonoBehaviour
{
    static GameStateManager GameStateManager => GameManager.Instance.GameStateManager;
    static PoolManager PoolManager => GameManager.Instance.PoolManager;
    static Timer Timer => GameManager.Instance.Timer;

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject bossPrefab;
    [SerializeField] float boundary;
    [SerializeField] int enemyTypeNum;
    public TextMeshProUGUI ResourceText;
    public float Enemydelay;
    public int Enemynum = 1;
    public float SpeedCoefficient = 1f;
    public GameObject EnemyList;
    public GameObject MeteorList;
    float maxX, maxY;
    const float Mindelay = 0.005f;
    static readonly float[] SpeedList = {1.25f, 2f, 7f, 1.25f, 1.25f, 1.5f, 2f};
    public float DamageCoefficient = 1f;
    public float MeteorCoefficient = 1f;
    [HideInInspector] public bool Disabled = false;
    [HideInInspector] public bool MakeMeteor = false;
    [HideInInspector] public bool SpawnSmall = false;
    [HideInInspector] public bool SpawnRandom = false;
    public float AddHP = 0f;
    public AudioSource Musicplayer;
    public AudioClip SfxMeteor;
    public Color[] ColorList;

    void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0, Enemydelay);
        maxX = EnemyList.GetComponent<RectTransform>().rect.width/2 * boundary;
        maxY = EnemyList.GetComponent<RectTransform>().rect.height/2;

        SpeedCoefficient = 1f;
    }

    public void ChangeSpawnDelay(float newcooltime)
    {
        if (newcooltime < Mindelay)
        {
            newcooltime = Mindelay;
        }
        Enemydelay = newcooltime;
        CancelInvoke(nameof(SpawnEnemy));
        InvokeRepeating(nameof(SpawnEnemy), 0, Enemydelay);
    }

    public GameObject SpawnMeteor()
    {
        if (!Disabled)
        {
            Musicplayer.PlayOneShot(SfxMeteor);
            var meteor = PoolManager.Get(PoolNumber.Meteor);
            meteor.transform.position = new Vector3(GameManager.Instance.Player.transform.position.x, maxY, 0f);
            meteor.GetComponent<Meteor>().Speed *= MeteorCoefficient;
            return meteor;
        }
        else
        {
            return null;
        }
    }

    public GameObject SpawnFinalBoss()
    {
        foreach (var t in GameManager.GetAllChilds(EnemyList.transform))
        {
            t.gameObject.SetActive(false);
        }
        var enemy = Instantiate(bossPrefab, new Vector3(0f, maxY, 0f), Quaternion.identity);
        enemy.transform.SetParent(EnemyList.transform);
        GameManager.Instance.ActiveEnemyNum += 1;
        return enemy;
    }

    Enemy SpawnEnemyWithType(int type, Vector3 pos)
    {
        var enemy = PoolManager.Get(PoolNumber.Enemy);
        enemy.transform.GetChild(0).GetComponent<SpriteRenderer>().color = ColorList[type];
        enemy.transform.position = pos;
        if (SpawnSmall)
        {
            enemy.transform.localScale = Vector3.one;
        }
        else
        {
            enemy.transform.localScale = Vector3.one * 1.2f;
        }

        var e = enemy.GetComponent<Enemy>();
        e.Maxspeed = SpeedList[type] * SpeedCoefficient;
        e.SetType(type);
        return e;
    }

    public void SpawnItem()
    {
        var item = PoolManager.Get(PoolNumber.Item);
        item.transform.position = new Vector3(Random.Range(-maxX, maxX), maxY, 0f);
        item.GetComponent<DropItem>().SetType((ItemType)Random.Range(0, 4));
    }

    void SpawnEnemy()
    {
        if (!GameStateManager.IsPlaying || Enemynum <= 0)
        {
            return;
        }

        int ran;
        if (!SpawnRandom)
        {
            ran = Random.Range(0, Timer.WaveNum < enemyTypeNum ? Timer.WaveNum : enemyTypeNum);
        }
        else
        {
            ran = Random.Range(0, enemyTypeNum);
        }

        var enemy = SpawnEnemyWithType(ran, new Vector3(Random.Range(-maxX, maxX), maxY, 0f));
        if (Timer.RoundNum % ConstantStore.BossPerWave != 0)
        {
            enemy.IsBoss = false;
            enemy.ExpAmount = 1;
        }
        else
        {
            enemy.MakeBoss();
            enemy.ExpAmount = Timer.WaveNum + 1;
        }
        enemy.MakeMeteor = MakeMeteor;
        enemy.MaxHP = enemy.MaxHP + AddHP > 1 ? enemy.MaxHP + AddHP : 1f;
        enemy.CurrentHP = enemy.MaxHP;
        enemy.SetHPText();
        Enemynum--;
        GameManager.Instance.ActiveEnemyNum++;
    }
}
