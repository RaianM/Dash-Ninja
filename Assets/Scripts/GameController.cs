using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Vector2 spawnPos;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        spawnPos = transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Death"))
        {
            Die();
        }
    }

    public void Checkpoint(Vector2 position)
    {
        spawnPos = position;
    }

    private void Die()
    {
        StartCoroutine(Respawn(0.5f));
    }

    private IEnumerator Respawn(float duration)
    {
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(duration);
        transform.position = spawnPos;
        spriteRenderer.enabled = true;
    }
}
