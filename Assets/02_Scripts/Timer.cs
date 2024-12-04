using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Starfall.Manager;

public class Timer : MonoBehaviour {
    [SerializeField] private float timeMax;
    private float remainTime;
    [SerializeField] private TextMeshProUGUI text;
    public int waveNum;
    public int roundNum;
    public int bossPerWave = 7;
    public bool waiting = false;
    public float waitTimeMax = 0f;
    private float waitTimeNow = 0f;
    public int addition = 0;
    [SerializeField] private Spawner spawner;
    [SerializeField] private GameObject EnemyList;
    [SerializeField] private Color[] colors;
    [SerializeField] private Image fillarea;
    private AudioSource musicPlayer;
    [SerializeField] private AudioClip sfxWave;
    [SerializeField] private AudioClip sfxBoss;
    [SerializeField] private AudioClip sfxFinalBoss;

    private GameStateManager gameState => GameStateManager.Instance;

    private void Start() {
        remainTime = timeMax;
        waveNum = 1;
        roundNum = 0;
        addition = 0;
        musicPlayer = GameObject.Find("Effects").GetComponent<AudioSource>();
        NextWave();
    }

    private void NextWave() {
        roundNum++;
        if (roundNum >= bossPerWave + 1) {
            roundNum -= bossPerWave;
            waveNum++;
        }

        if (waveNum == 8) {
            if (roundNum == 1) {
                spawner.SpawnFinalBoss();
                SetText("SOMETHING BIG IS COMING...!", colors[2]);
                musicPlayer.PlayOneShot(sfxFinalBoss);
                roundNum = 2;
            }
        }
        else {
            waiting = false;
            if (roundNum % bossPerWave != 0) {  
                // Normal
                SetText("Wave " + waveNum + "-" + roundNum, colors[0]);
                remainTime = timeMax;
                spawner.enemynum = waveNum * (waveNum - 1) + roundNum + addition;
                musicPlayer.PlayOneShot(sfxWave);
            }
            else {    
                // Boss
                SetText("Wave " + waveNum + "-Boss", colors[1]);
                remainTime = timeMax + waveNum;
                spawner.enemynum = waveNum * waveNum + 1;
                musicPlayer.PlayOneShot(sfxBoss);
            }
        }
    }

    private void SetText(string t, Color c) {
        text.text = t;
        text.color = c;
    }

    private void Update() {
        if (!gameState.IsPlaying)
            return;

        if (waiting) {
            waitTimeNow -= Time.deltaTime;
            if (waitTimeNow <= 0 && waveNum != 8)
                NextWave();
            return;
        }

        if (remainTime <= 0) {
            waiting = true;
            SetText("Wait...", colors[2]);
            waitTimeNow = waitTimeMax;
        }
        else if(spawner.enemynum == 0 && GameManager.Instance.activeEnemyNum == 0) { 
            // if time is left and all enemy killed
            waiting = true;
            SetText("Wait...", colors[2]);
            waitTimeNow = 0.25f;
        }
        else if(spawner.enemynum == 0) {
            remainTime -= Time.deltaTime;
        }
    }
}
