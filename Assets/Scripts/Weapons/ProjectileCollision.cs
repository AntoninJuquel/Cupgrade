using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.Universal;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField] Light2D muzzleFlashLight;
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite muzzleFlashSprite;

    [SerializeField] float millisecondToFlash = 3;

    [SerializeField] SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    Collider2D col;
    Vector3 lastVelocity;
    float maxDuration;
    float duration;
    int damage;
    float explosionRadius;
    float explosionForce;
    int reflections;

    //[System.Serializable]
    //public class HitEvent : UnityEvent<Collider2D> { }
    //[SerializeField] HitEvent OnHitEvent;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }
    public void Setup(int _damage, float _maxDuration, float _explosionRadius, float _explosionForce, int _reflections)
    {
        damage = _damage;
        maxDuration = _maxDuration;
        explosionRadius = _explosionRadius;
        explosionForce = _explosionForce;
        reflections = _reflections;
        col.isTrigger = reflections == 0;
        spriteRenderer.sprite = muzzleFlashSprite;
        muzzleFlashLight.gameObject.SetActive(true);
    }
    private void Update()
    {
        duration += Time.deltaTime;
        lastVelocity = rb.velocity;
        if (duration >= millisecondToFlash / 100f)
        {
            spriteRenderer.sprite = defaultSprite;
            muzzleFlashLight.gameObject.SetActive(false);
        }
        if (duration >= maxDuration)
            Destroy();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable damageable = collision.collider.GetComponent<IDamageable>();
        if (damageable == null && reflections > 0)
            Ricochet(collision);
        else
            Hit(damageable);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
            Hit(damageable);
        ParticleManager.Instance.SpawnParticle("Hit", transform.position, Quaternion.identity);
    }
    public void Ricochet(Collision2D collision)
    {
        reflections--;

        Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

        transform.right = direction;
    }
    public void Hit(IDamageable damageable)
    {
        AudioManager.Instance.Play("Hit_0");

        if (damageable != null)
        {
            damageable.DecreaseHealth(damage);
        }
        Destroy();
    }
    public void Destroy()
    {
        // Explosion
        if (explosionRadius > 0)
        {
            print("Audio explosion");
            AudioManager.Instance.Play("Explosion_0");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (Collider2D collider in colliders)
            {
                IDamageable damageable = collider.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.DecreaseHealth(damage);
            }
        }
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        duration = 0;
        gameObject.SetActive(false);
    }
}
