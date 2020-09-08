using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movements : MonoBehaviour
{
    float moveSpeed;
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashTime = .1f;

    [SerializeField] int playerLayerIndex;
    [SerializeField] int enemyLayerIndex;
    [SerializeField] int projectileLayerIndex;

    Rigidbody2D rb;
    Vector3 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = baseSpeed;
    }
    public void SetMovements(float horizontal, float vertical)
    {
        movement.x = horizontal;
        movement.y = vertical;
    }

    public IEnumerator Dash()
    {
        moveSpeed = dashSpeed;
        Physics2D.IgnoreLayerCollision(playerLayerIndex, enemyLayerIndex, true);
        Physics2D.IgnoreLayerCollision(playerLayerIndex, projectileLayerIndex, true);
        yield return new WaitForSeconds(dashTime);

        moveSpeed = baseSpeed;
        Physics2D.IgnoreLayerCollision(playerLayerIndex, enemyLayerIndex, false);
        Physics2D.IgnoreLayerCollision(playerLayerIndex, projectileLayerIndex, false);
    }

    public void Move()
    {
        rb.MovePosition(rb.position + (Vector2)movement * moveSpeed * Time.deltaTime);
    }
}
