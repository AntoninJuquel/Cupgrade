using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    enum Type
    {
        Kamikaze,
        Gunner,
        Boss
    }
    [SerializeField] Type type;
    [SerializeField] float attackRange = 2.5f;
    [Header("Gunner")]
    [SerializeField] Transform weaponHolder;
    [SerializeField] GunController gun;
    [Header("Kamikaze")]
    [SerializeField] float explosionTime;
    float explosionTimer;
    [SerializeField] float radius;
    [SerializeField] int explosionDamage;

    Vector3 baseScale;
    bool exploded = false;
    private void Awake()
    {
        if (weaponHolder == null)
            return;
        baseScale = weaponHolder.localScale;
    }
    public void Aim()
    {
        if (weaponHolder == null)
            return;
        // Aiming calculs
        Vector3 lookPos = PlayerController.Instance.GetPosition() - weaponHolder.position;
        float angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

        weaponHolder.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (Mathf.Abs(angle) > 100)
        {
            weaponHolder.transform.localPosition = new Vector3(-.2f, -.16f, 0);
            weaponHolder.transform.localScale = new Vector3(baseScale.x, -baseScale.y, baseScale.z);
        }
        else if (Mathf.Abs(angle) < 80)
        {
            weaponHolder.transform.localPosition = new Vector3(.2f, -.16f, 0);
            weaponHolder.transform.localScale = new Vector3(baseScale.x, baseScale.y, baseScale.z);
        }

    }

    public void Attack()
    {
        switch (type)
        {
            case Type.Kamikaze:
                explosionTimer += Time.deltaTime;
                if (explosionTimer > explosionTime && !exploded)
                {
                    exploded = true;
                    GetComponent<IDamageable>().DecreaseHealth(1000);
                }
                break;
            case Type.Gunner:
                if (Physics2D.Raycast(gun.GetFirepoint(), PlayerController.Instance.GetPosition() - gun.GetFirepoint(), Mathf.Infinity).transform.CompareTag("Player"))
                    gun.MyInput(true, true, false);
                break;
            case Type.Boss:
                if (Physics2D.Raycast(gun.GetFirepoint(), PlayerController.Instance.GetPosition() - gun.GetFirepoint(), Mathf.Infinity).transform.CompareTag("Player"))
                    gun.MyInput(true, true, false);
                if (GetComponent<EnemyAI>().GetHealth() < GetComponent<EnemyAI>().GetMaxHealth() * 0.25f)
                    SwitchWeapon(3);
                else if (GetComponent<EnemyAI>().GetHealth() < GetComponent<EnemyAI>().GetMaxHealth() * 0.5f)
                    SwitchWeapon(2);
                else if (GetComponent<EnemyAI>().GetHealth() < GetComponent<EnemyAI>().GetMaxHealth() * 0.75f)
                    SwitchWeapon(1);
                break;
            default:
                return;
        }
    }
    void SwitchWeapon(int i)
    {
        for (int j = 0; j < i; j++)
        {
            weaponHolder.GetChild(j).gameObject.SetActive(false);
        }
        weaponHolder.GetChild(i).gameObject.SetActive(true);
        gun = weaponHolder.GetChild(i).GetComponent<GunController>();
    }
    public void ResetAttack()
    {
        explosionTimer = 0;
    }
    public void LastAttack()
    {
        switch (type)
        {
            case Type.Kamikaze:
                if (Vector2.Distance(PlayerController.Instance.GetPosition(), transform.position) <= radius)
                    PlayerController.Instance.GetComponent<IDamageable>().DecreaseHealth(explosionDamage);
                ParticleManager.Instance.SpawnParticle("Explosion", transform.position, Quaternion.identity);
                AudioManager.Instance.Play("Explosion_0");
                GameManager.Instance.RemoveEnemy(gameObject, false);
                break;
            case Type.Gunner:
                break;
            default:
                break;
        }
    }
    public float GetAttackRange()
    {
        return attackRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
