using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayer;
    Transform target;
    Rigidbody2D rb;
    float speed;
    float rotateSpeed;
    float detectionRadius;

    public void Setup(float _speed, float _rotateSpeed, float _detectionRadius)
    {
        speed = _speed;
        rotateSpeed = _rotateSpeed;
        detectionRadius = _detectionRadius;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.velocity = transform.right * speed;

        if (rotateSpeed > 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
            if (colliders.Length > 0)
            {
                target = colliders[0].transform;
                Vector2 direction = (Vector2)target.position - rb.position;

                direction.Normalize();

                transform.right = Vector3.Lerp(transform.right, direction, Time.deltaTime * rotateSpeed);
            }
        }
    }
}
