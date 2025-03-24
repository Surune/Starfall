using UnityEngine;
using UnityEngine.UI;
using Starfall.Manager;
using Starfall.Entity;
using Starfall.Constants;

public class Ability : MonoBehaviour
{
    static Player Player => GameManager.Instance.Player;
    static PoolManager PoolManager => GameManager.Instance.PoolManager;
    static PlayerManager PlayerManager => GameManager.Instance.PlayerManager;
    static GameStateManager GameStateManager => GameManager.Instance.GameStateManager;
    static HPManager HpManager => GameManager.Instance.HPManager;

    public Image AbilityImage;
    public float SkillCooltimeMax = 20f;
    public int AbilityID = 0;
    public AbilityType AbilityType = AbilityType.Active;
    float skillCooltimeNow = 0f;

    void Update()
    {
        if (AbilityType != 0 && GameStateManager.IsPlaying)
        {
            skillCooltimeNow += Time.deltaTime;
            if (skillCooltimeNow >= SkillCooltimeMax)
            {
                Activate();
            }
            AbilityImage.fillAmount = skillCooltimeNow / SkillCooltimeMax;
        }
    }

    void Activate()
    {
        skillCooltimeNow = 0;
        if (AbilityType == AbilityType.Active)
        {
            GameObject fireball;
            switch (AbilityID)
            {
                case 2:
                    fireball = PoolManager.Get(PoolNumber.Fireball);
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.transform.position = Player.transform.position;
                    fireball.GetComponent<Fireball>().Damage = 1f;
                    break;
                case 3:
                    fireball = PoolManager.Get(PoolNumber.Fireball);
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.transform.position = Player.transform.position;
                    PlayerManager.SetFireInfo(fireball.GetComponent<Fireball>());
                    break;
                case 17:
                    PlayerManager.DamageAllEnemy(PlayerManager.damage * PlayerManager.damageCoefficient + PlayerManager.fixDamage);
                    break;
                case 21:
                    fireball = PoolManager.Get(PoolNumber.Fireball);
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.transform.position = Player.transform.position;
                    PlayerManager.SetFireInfo(fireball.GetComponent<Fireball>());
                    fireball.GetComponent<Fireball>().IsFatal = true;
                    break;
                case 43:
                    PlayerManager.fixDamage += 0.05f;
                    break;
                case 45:
                    HpManager.ChangeHP(+5);
                    break;
                case 47:
                    for (int i = 0; i < 6; i++)
                    {
                        fireball = PoolManager.Get(PoolNumber.Fireball);
                        fireball.transform.position = Player.transform.position;
                        fireball.transform.rotation = Quaternion.Euler(0, 0, 5*i-15);
                        fireball.GetComponent<Fireball>().Damage = 6f;
                    }
                    break;
                case 63:
                    HpManager.GetBarrier(1);
                    break;
                case 66:
                    PlayerManager.criticalProb += 0.025f;
                    break;
                case 70:
                    PlayerManager.criticalCoefficient += 0.025f;
                    break;
                case 77:
                    for (int i = 0; i < 3; i++)
                    {
                        fireball = PoolManager.Get(PoolNumber.Fireball);
                        fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                        fireball.transform.position = Player.transform.position;
                        fireball.GetComponent<Fireball>().Damage = 1f;
                        fireball.GetComponent<Fireball>().Udo = true;
                    }
                    break;
                case 79:
                    fireball = PoolManager.Get(PoolNumber.Fireball);
                    fireball.transform.rotation = Quaternion.Euler(0, 0, 0);
                    fireball.transform.position = Player.transform.position;
                    fireball.GetComponent<Fireball>().Damage = 3f;
                    fireball.GetComponent<Fireball>().Penetrate = true;
                    break;
                case 80:
                    Player.GetComponent<Player>().Reloading = true;
                    Invoke(nameof(ReloadOff), 0.5f);
                    break;
                case 84:
                    PlayerManager.damage += 0.1f;
                    break;
                case 114:
                    Player.GetComponent<Player>().ChangeSkillCool(Player.GetComponent<Player>().SkillCooltimeMax - 0.01f);
                    break;
                case 135:
                    GameManager.Instance.Spawner.SpawnItem();
                    break;
                default:
                    break;
            }
        }
        else if (AbilityType == AbilityType.Area)
        {
            GameObject area = PoolManager.Get(PoolNumber.Area);
            area.GetComponent<Area>().SetIcon(transform.GetChild(1).gameObject.GetComponent<Image>().sprite);
            switch (AbilityID)
            {
                case 19:
                    area.transform.position = Player.transform.position + new Vector3(0f, 2f, 0f);
                    area.GetComponent<Area>().Duration = 1.5f;
                    area.GetComponent<Area>().SetProperty(isFixed : true);
                    break;
                case 24:
                    area.transform.position = Player.transform.position + new Vector3(0f, 7f, 0f);
                    area.GetComponent<Area>().Duration = 1f;
                    area.GetComponent<Area>().SetProperty(isDamage : true);
                    break;
                case 55:
                    area.transform.position = Player.transform.position + new Vector3(0f, 6f, 0f);
                    area.GetComponent<Area>().Duration = 1f;
                    area.GetComponent<Area>().SetProperty(isSlow : true);
                    break;
                case 91:
                    area.transform.position = Player.transform.position + new Vector3(0f, 4f, 0f);
                    area.GetComponent<Area>().Duration = 1f;
                    area.GetComponent<Area>().SetProperty(isSwarm : true);
                    break;
                case 96:
                    area.transform.position = Player.transform.position + new Vector3(0f, 5f, 0f);
                    area.GetComponent<Area>().Duration = 1f;
                    area.GetComponent<Area>().SetProperty(isUnrelenting : true);
                    break;
                case 98:
                    area.transform.position = Player.transform.position + new Vector3(0f, 3f, 0f);
                    area.GetComponent<Area>().Duration = 1.5f;
                    area.GetComponent<Area>().SetProperty(isCrit : true);
                    break;
                default:
                    break;
            }
        }
    }

    void ReloadOff()
    {
        Player.GetComponent<Player>().Reloading = false;
    }
}
