using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private Vector2 spawnPos;
    private GameObject sprite;
    private Vector2 lowerBound;
    private Rigidbody2D moveObj;

    private void Awake()
    {
        spawnPos = transform.position;
        lowerBound = new Vector2(transform.position.x, -8.7f);
        sprite = GameObject.Find("Sprite");
        moveObj = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        spawnPos = transform.position;
    }
    private void Update()
    {
        if (checkIfLower())
            Die();
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
        sprite.SetActive(false);
        moveObj.constraints = RigidbodyConstraints2D.FreezePosition;
        yield return new WaitForSeconds(duration);
        transform.position = spawnPos;
        moveObj.constraints = RigidbodyConstraints2D.FreezeRotation;
        sprite.SetActive(true);
    }
    public bool checkIfLower()
    {
        return lowerBound.y > transform.position.y;
    }
    public Vector2 getLower() { return lowerBound; }
    public void setLower(float lower) {  lowerBound.y = lower; }
}
