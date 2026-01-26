using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Bullet : MonoBehaviour
{
    [SerializeField] private string playerLyaerName = "Player";
    [SerializeField] private string groundLyaerName = "Ground";
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    public void FlipSprite() => sr.flipX = !sr.flipX;
    public void SetVelocity(Vector2 velocity) => rb.velocity = velocity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLyaerName))
        {
            collision.GetComponent<Player>().Knockback(transform.position.x);
            Destroy(gameObject);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer(groundLyaerName))
        {
            Destroy(gameObject);
        }
    }
}
