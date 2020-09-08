using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovements : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void MoveTo(Vector3 targetedPosition)
    {
        Vector2 direction = (targetedPosition - transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        spriteRenderer.flipX = direction.x < 0;
    }

    public IEnumerator BumpBody()
    {
        rb.AddForce((transform.position-PlayerController.Instance.GetPosition()) * UnityEngine.Random.Range(2f,10f),ForceMode2D.Impulse);
        transform.Rotate(0, 0, UnityEngine.Random.Range(-360f,360f));
        yield return new WaitForSeconds(.2f);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
