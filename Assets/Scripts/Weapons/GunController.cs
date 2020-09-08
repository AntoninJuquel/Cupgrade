using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunController : MonoBehaviour
{
    [Header("Gun Stats")]
    [SerializeField] string projectileName;
    [SerializeField] string soundFx;
    [SerializeField] int damage;
    [SerializeField] float timeBtwShooting, timeBtwShots, spreadAngle, reloadTime;
    [SerializeField] int magazineSize, shotsPerTap, projectilePerShot = 1;
    [SerializeField] float knockBackForce;
    [SerializeField] bool allowButtonHold;
    [SerializeField] int projectilesLeft, projectilesToShoot;

    [Header("Projectile Stats")]
    [SerializeField] float range;
    [SerializeField] float projectileSpeed;

    [Header("Explosion")]
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionForce;
    [Header("Reflect")]
    [SerializeField] int reflections;
    [Header("Smart bullet")]
    [SerializeField] float detectionRadius;
    [SerializeField] float rotateSpeed;
    [Header("Pierce")]
    [SerializeField] bool piercing;

    [Header("Laser")]
    [SerializeField] LineRenderer lr;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] LayerMask obstacleLayer;

    [Header("Laser Sight")]
    [SerializeField] bool hasLaser;
    [SerializeField] LineRenderer laserLr;

    [Header("Status")]
    [SerializeField] bool readyToShoot;
    [SerializeField] bool shooting;
    [SerializeField] bool reloading;

    [Header("References")]
    [SerializeField] Transform firePoint;
    [SerializeField] Rigidbody2D playerRb;

    [Header("Pickup")]
    [SerializeField] bool held;

    [Header("Camera Shake")]
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeMagnitude;
    CameraController cameraController;

    List<Quaternion> projectilesAngle;

    private void Awake()
    {
        projectilesAngle = new List<Quaternion>(new Quaternion[projectilePerShot]);
        projectilesLeft = magazineSize;
        readyToShoot = true;
        if (!held)
        {
            DroppedDown();
        }
        laserLr.positionCount = 0;
        lr.positionCount = 0;
    }
    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        laserLr.positionCount = 0;
    }
    private void LateUpdate()
    {
        if (hasLaser)
        {
            laserLr.positionCount = 2;
            laserLr.SetPosition(0, firePoint.position);
            laserLr.SetPosition(1, Physics2D.Raycast(firePoint.position, transform.right, Mathf.Infinity, enemyLayer | obstacleLayer).point);
        }
    }
    public void MyInput(bool holding, bool tap, bool reload)
    {
        if (projectilesLeft == 0 && !reloading)
        {
            StartCoroutine(Reload());
        }

        if (allowButtonHold)
        {
            shooting = holding;
        }
        else
        {
            shooting = tap;
        }


        // Shoot
        if (readyToShoot && shooting && !reloading && projectilesLeft > 0)
        {
            projectilesToShoot = shotsPerTap;
            StartCoroutine(Shoot());
        }

        // Reload
        if (reload && projectilesLeft < magazineSize && !reloading)
            StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        projectilesLeft = magazineSize;
        reloading = false;
        if (playerRb.CompareTag("Player"))
        {
            UIManager.Instance.SetBulletsCount(projectilesLeft, magazineSize);
            AudioManager.Instance.Play("Reload_" + Random.Range(0, 4));
        }
    }

    IEnumerator Shoot()
    {
        readyToShoot = false;

        // KnockBack

        playerRb.AddForce(-(Vector2)transform.right.normalized * knockBackForce);

        // Laser
        if (projectileName == "Laser")
        {
            lr.positionCount = 2;
            lr.SetPosition(0, firePoint.position);
            Vector2 firstWallPosition = Physics2D.Raycast(firePoint.position, transform.right, Mathf.Infinity, obstacleLayer).point;
            if (piercing)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(firePoint.position, transform.right, Vector2.Distance(firstWallPosition, firePoint.position), enemyLayer);

                lr.SetPosition(1, firstWallPosition);

                foreach (RaycastHit2D hit in hits)
                {
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    if (damageable != null)
                        damageable.DecreaseHealth(damage);
                }
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, transform.right, Mathf.Infinity, enemyLayer | obstacleLayer);
                if (hit)
                {
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    if (damageable != null)
                        damageable.DecreaseHealth(damage);

                    lr.SetPosition(1, hit.point);
                }
            }

            yield return new WaitForSeconds(.05f);
            lr.positionCount = 0;
        }
        // Projectile
        else
        {
            GameObject projectile;
            for (int i = 0; i < projectilePerShot; i++)
            {
                AudioManager.Instance.Play(soundFx + "_" + Random.Range(0, 4));

                StartCoroutine(cameraController.Shake(shakeDuration, shakeMagnitude));

                projectilesAngle[i] = Random.rotation;
                projectile = ProjectileManager.Instance.Create(projectileName, firePoint.position, firePoint.rotation, damage, range, projectileSpeed, explosionRadius, explosionForce, rotateSpeed, detectionRadius, reflections);
                projectile.transform.rotation = Quaternion.RotateTowards(projectile.transform.rotation, projectilesAngle[i], spreadAngle);
            }
        }

        projectilesLeft--;
        projectilesToShoot--;

        if (playerRb.CompareTag("Player"))
            UIManager.Instance.SetBulletsCount(projectilesLeft, magazineSize);
        if (projectilesToShoot > 0 && projectilesLeft > 0)
        {
            yield return new WaitForSeconds(timeBtwShots);
            StartCoroutine(Shoot());
        }
        else
        {
            yield return new WaitForSeconds(timeBtwShooting);
            readyToShoot = true;
        }
    }
    public void PickedUp(Rigidbody2D rb)
    {
        Destroy(GetComponent<BoxCollider2D>());
        held = true;
        playerRb = rb;
    }

    public void DroppedDown()
    {
        StopAllCoroutines();
        reloading = false;
        if (gameObject.GetComponent<BoxCollider2D>() == null)
            gameObject.AddComponent<BoxCollider2D>();
        GetComponent<BoxCollider2D>().isTrigger = true;
        held = false;
        playerRb = null;
    }

    public void Switched()
    {
        StopAllCoroutines();
        reloading = false;
        readyToShoot = true;
    }
    public void Upgrade()
    {
        print("Up");
    }
    public int GetProjectileLeft()
    {
        return projectilesLeft;
    }
    public int GetMagazineSize()
    {
        return magazineSize;
    }
    public Sprite GetWeaponImage()
    {
        return transform.GetComponentInChildren<SpriteRenderer>().sprite;
    }
    public float GetReloadTime()
    {
        return reloadTime;
    }
    public bool GetReloading()
    {
        return reloading;
    }
    public Vector3 GetFirepoint()
    {
        return firePoint.position;
    }
    public IEnumerator StartSmart(float time,float _detectionRadius, float _rotateSpeed)
    {
        detectionRadius = _detectionRadius;
        rotateSpeed = _rotateSpeed;
        yield return new WaitForSeconds(time);
        detectionRadius = 0f;
        rotateSpeed = 0f;
    }
}
