using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private float newLower = -8.7f;
    private bool notReached = true;
    GameController playerObj;

    private void Awake()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player").GetComponent<GameController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && notReached)
        {
            playerObj.Checkpoint(transform.position);
            playerObj.setLower(newLower);
            notReached = false;
        }
    }


}
