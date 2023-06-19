using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform prevRoom;
    [SerializeField] private Transform nextRoom;
    [SerializeField] private CameraController cam;
    [SerializeField] private Transform prevSpawn;
    [SerializeField] private Transform nextSpawn;

    GameController playerObj;
    private bool reverseRoom = false; // false if going into new Room

    private void Awake()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player").GetComponent<GameController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if (collision.tag == "Player")
        {
            
            if (prevRoom.position.y != nextRoom.position.y)
            {

                if (reverseRoom)
                {
                    playerObj.Checkpoint(prevSpawn.position);
                    cam.MoveToNewRoom(prevRoom, !reverseRoom);
                }
                else
                {
                    playerObj.Checkpoint(nextSpawn.position);
                    cam.MoveToNewRoom(nextRoom, !reverseRoom);
                }
                reverseRoom = !reverseRoom;
            }
            else
            {
                if (collision.transform.position.x < transform.position.x)
                {
                    playerObj.Checkpoint(nextSpawn.position);
                    cam.MoveToNewRoom(nextRoom, true);
                }
                else
                {
                    playerObj.Checkpoint(prevSpawn.position);
                    cam.MoveToNewRoom(prevRoom, false);
                }
            }
        }
    }
}
