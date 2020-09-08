using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] Transform weaponHolder;
    [SerializeField] LayerMask cupgradeLayer;
    [SerializeField] LayerMask gunLayer;
    [SerializeField] float radius;
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer GFX;
    [SerializeField] Transform crosshair;
    [SerializeField] GameObject mouseRight;

    [SerializeField] int maxHealth;
    int health;

    Vector3 mousePos;
    Vector3 lookPos;
    float angle;
    float reloadingProgress = 0f;
    Camera cam;

    Inventory inventory;
    Shooting shooting;
    Movements movements;

    public static PlayerController Instance;

    private void Awake()
    {
        Instance = this;
        inventory = GetComponent<Inventory>();
        shooting = GetComponent<Shooting>();
        movements = GetComponent<Movements>();
        cam = Camera.main;

        health = maxHealth;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        UIManager.Instance.SetHealthBar(health, maxHealth);
    }
    private void Update()
    {
        if (GameManager.gamePaused)
        {
            return;
        }
        // Aiming calculs
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        crosshair.position = (Vector2)mousePos;
        lookPos = mousePos - weaponHolder.position;
        angle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(angle) > 100)
            GFX.flipX = true;
        else if (Mathf.Abs(angle) < 80)
            GFX.flipX = false;

        weaponHolder.transform.localPosition = new Vector3(GFX.flipX ? -.2f : .2f, -.16f, 0);
        weaponHolder.transform.localScale = new Vector3(1, GFX.flipX ? -1 : 1, 1);

        // Shooting
        GunController weaponHeld = shooting.GetWeaponHeld();
        if (weaponHeld != null)
        {
            weaponHeld.MyInput(Input.GetMouseButton(0), Input.GetMouseButtonDown(0), Input.GetKey(KeyCode.R));
            if (weaponHeld.GetReloading())
                reloadingProgress += Time.deltaTime / weaponHeld.GetReloadTime();
            else
                reloadingProgress = 0;

            UIManager.Instance.SetReloadingBar(reloadingProgress);
        }

        // Inventory
        Collider2D collider = Physics2D.Raycast(cam.transform.position, crosshair.position - cam.transform.position, Vector2.Distance(cam.transform.position, crosshair.position), cupgradeLayer | gunLayer).collider;
        if (collider)
        {
            if (Vector2.Distance(transform.position, collider.transform.position) < radius)
            {
                mouseRight.SetActive(true);
                if (Input.GetMouseButtonDown(1))
                {
                    Cupgrade isCupgrade = collider.GetComponent<Cupgrade>();
                    GunController isGun = collider.GetComponent<GunController>();
                    if (isCupgrade != null)
                        inventory.Interact(collider);
                    else if (isGun)
                        inventory.PickUpGun(isGun);
                }
            }
            else
                mouseRight.SetActive(false);
        }
        else
            mouseRight.SetActive(false);
        

        // Switching
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
            inventory.SwitchUp();
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
            inventory.SwitchDown();

        // Moving
        movements.SetMovements(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        anim.SetBool("isRunning", Math.Abs(Input.GetAxisRaw("Horizontal")) > 0 || Math.Abs(Input.GetAxisRaw("Vertical")) > 0);

        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(movements.Dash());
    }
    private void FixedUpdate()
    {
        movements.Move();
    }
    private void LateUpdate()
    {
        // Aiming
        shooting.SetRotation(angle);
    }
    public Collider2D[] CheckCupgrade()
    {
        return Physics2D.OverlapCircleAll(transform.position, radius, cupgradeLayer);
    }

    public Collider2D[] CheckGuns()
    {
        return Physics2D.OverlapCircleAll(transform.position, radius, gunLayer);
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void DecreaseHealth(int damage)
    {
        health -= damage;
        if (health <= 0)
            Die();

        UIManager.Instance.SetHealthBar(health, maxHealth);
    }
    public void IncreaseHealth(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        UIManager.Instance.SetHealthBar(health, maxHealth);
    }
    public void Die()
    {
        GameManager.Instance.EndScreen();
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Finish"))
            GameManager.Instance.FinishLevel();
        else if (collision.CompareTag("Chest"))
        {
            LootManager.Instance.LootWeapon(collision.transform);
            GameManager.Instance.AddChestOpened();
        }
        else if (collision.CompareTag("HealthBox") && health < maxHealth)
        {
            health++;
            UIManager.Instance.SetHealthBar(health, maxHealth);
            GameManager.Instance.AddKitUsed();
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("SmartBullet") && shooting.GetWeaponHeld() != null)
        {
            StartCoroutine(shooting.GetWeaponHeld().StartSmart(10f, 2f, 20f));
            Destroy(collision.gameObject);
        }
    }
}