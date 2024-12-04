using UnityEngine;
using UnityEngine.UI;
using Starfall.Manager;
using Starfall.Entity;
using Starfall.Constants;

public class Ability : MonoBehaviour {
    # region Managers
    public Image abilityImage;
    private static GameObject player;
    private static GameObject enemyList;
    private static PoolManager poolManager => GameManager.Instance.PoolManager;
    private static PlayerManager playerManager => GameManager.Instance.PlayerManager;
    private static GameStateManager gameStateManager => GameManager.Instance.gameStateManager;
    private static HPManager hpManager => GameManager.Instance.HPManager;
    # endregion

    # region Property
    public float skillCooltimeMax = 20f;
    private float skillCooltimeNow = 0f;
    public int abilityID = 0;
    public int abilityType = 1;
    # endregion

    void Start() {
        if (player == null)     player = GameObject.Find("Player");
        if (enemyList == null)  enemyList = GameObject.Find("Enemies");
    }

    void Update() {
        if (abilityType != 0 && gameStateManager.IsPlaying) {
            skillCooltimeNow += Time.deltaTime;
            if (skillCooltimeNow >= skillCooltimeMax) 
                Activate();
            abilityImage.fillAmount = skillCooltimeNow / skillCooltimeMax;
        }
    }

    public void Activate() {
        skillCooltimeNow = 0;
        if (abilityType == 1) {
            GameObject fireball;
            switch (abilityID) {
                case 2:
                    fireball = poolManager.Get(PoolNumber.Fireball);
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.transform.position = player.transform.position;
                    fireball.GetComponent<Fireball>().damage = 1f;
                    break;
                case 3:
                    fireball = poolManager.Get(PoolNumber.Fireball);
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.transform.position = player.transform.position;
                    playerManager.SetFireInfo(fireball.GetComponent<Fireball>());
                    break;
                case 17:
                    playerManager.DamageAllEnemy(playerManager.damage * playerManager.damageCoefficient + playerManager.fixDamage);
                    break;
                case 21:
                    fireball = poolManager.Get(PoolNumber.Fireball);
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.transform.position = player.transform.position;
                    playerManager.SetFireInfo(fireball.GetComponent<Fireball>());
                    fireball.GetComponent<Fireball>().isFatal = true;
                    break;
                case 43:
                    playerManager.fixDamage += 0.05f;
                    break;
                case 45:
                    hpManager.ChangeHP(+5);
                    break;
                case 47:
                    for (int i = 0; i < 6; i++) {
                        fireball = poolManager.Get(PoolNumber.Fireball);
                        fireball.transform.position = player.transform.position;
                        fireball.transform.rotation = Quaternion.Euler(0, 0, 5*i-15);
                        fireball.GetComponent<Fireball>().damage = 6f;
                    }
                    break;
                case 63:
                    hpManager.GetBarrier(1);
                    break;
                case 66:
                    playerManager.criticalProb += 0.025f;
                    break;
                case 70:
                    playerManager.criticalCoefficient += 0.025f;
                    break;
                case 77:
                    for (int i = 0; i < 3; i++) {
                        fireball = poolManager.Get(PoolNumber.Fireball);
                        fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                        fireball.transform.position = player.transform.position;
                        fireball.GetComponent<Fireball>().damage = 1f;
                        fireball.GetComponent<Fireball>().udo = true;
                    }
                    break;
                case 79:
                    fireball = poolManager.Get(PoolNumber.Fireball);
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.transform.position = player.transform.position;
                    fireball.GetComponent<Fireball>().damage = 3f;
                    fireball.GetComponent<Fireball>().penetrate = true;
                    break;
                case 80:
                    player.GetComponent<Player>().reloading = true;
                    Invoke("ReloadOff", 0.5f);
                    break;
                case 84:
                    playerManager.damage += 0.1f;
                    break;
                case 114:
                    player.GetComponent<Player>().ChangeSkillCool(player.GetComponent<Player>().skillCooltimeMax - 0.01f);
                    break;
                case 135:
                    GameManager.Instance.spawner.SpawnItem();
                    break;
                default:
                    break;
            }
        }
        else if (abilityType == 2) {
            GameObject area = poolManager.Get(PoolNumber.Area);
            area.GetComponent<Area>().SetIcon(transform.GetChild(1).gameObject.GetComponent<Image>().sprite);
            switch (abilityID) {
                case 19:
                    area.transform.position = player.transform.position + new Vector3(0f, 2f, 0f);
                    area.GetComponent<Area>().duration = 1.5f;
                    area.GetComponent<Area>().SetProperty(isFixed : true);
                    break;
                case 24:
                    area.transform.position = player.transform.position + new Vector3(0f, 7f, 0f);
                    area.GetComponent<Area>().duration = 1f;
                    area.GetComponent<Area>().SetProperty(isDamage : true);
                    break;
                case 55:
                    area.transform.position = player.transform.position + new Vector3(0f, 6f, 0f);
                    area.GetComponent<Area>().duration = 1f;
                    area.GetComponent<Area>().SetProperty(isSlow : true);
                    break;
                case 91:
                    area.transform.position = player.transform.position + new Vector3(0f, 4f, 0f);
                    area.GetComponent<Area>().duration = 1f;
                    area.GetComponent<Area>().SetProperty(isSwarm : true);
                    break;
                case 96:
                    area.transform.position = player.transform.position + new Vector3(0f, 5f, 0f);
                    area.GetComponent<Area>().duration = 1f;
                    area.GetComponent<Area>().SetProperty(isUnrelenting : true);
                    break;
                case 98:
                    area.transform.position = player.transform.position + new Vector3(0f, 3f, 0f);
                    area.GetComponent<Area>().duration = 1.5f;
                    area.GetComponent<Area>().SetProperty(isCrit : true);
                    break;
                default:
                    break;
            }
        }
    }

    private void ReloadOff() {
        player.GetComponent<Player>().reloading = false;
    }
}
