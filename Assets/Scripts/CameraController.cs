using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private Vector3 velocity = Vector3.zero;
    private GameController playerObj;
    [SerializeField] private bool followPlayer;

    // for slide camera
    private float currentX;
    private float currentY;
    private int colorIndex;

    [SerializeField] private float speed;
    [SerializeField] private float transition;
    [SerializeField] private Color[] colors;
    [SerializeField] private float transitionTime;

    // for follow camera
    private Transform target;
    [SerializeField] private Vector3 positionOffset;
 
    [Range(0f, 1f)]
    public float smoothTime;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        playerObj = GameObject.FindGameObjectWithTag("Player").GetComponent<GameController>();
    }
    private void Start()
    {
        cam = GetComponent<Camera>();
        colorIndex = 0;
    }

    private void Update()
    {
        if (!followPlayer)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentX, currentY, transform.position.z), ref velocity, speed);
            cam.backgroundColor = Color.Lerp(cam.backgroundColor, colors[colorIndex], transitionTime * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        if (followPlayer)
        {
            Vector3 targetPosition = target.position+positionOffset;
            float yPos = playerObj.getLower().y;
            if (targetPosition.y < yPos)
                targetPosition.y = yPos;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }

    public void MoveToNewRoom(Transform _newRoom, bool _nextRoom)
    {
        currentX = _newRoom.position.x;
        currentY = _newRoom.position.y;
        if (_nextRoom)
            colorIndex++;
        else
            colorIndex--;
        CheckIndex();   
    }

    public void MoveToVerticalRoom(Transform _newRoom, bool _nextRoom)
    {
        currentX = _newRoom.position.x;
        currentY = _newRoom.position.y;
        if (_nextRoom)
            colorIndex++;
        else
            colorIndex--;
        CheckIndex();
    }

    private void CheckIndex()
    {
        if (colorIndex >= colors.Length || colorIndex < 0)
            colorIndex = 0;
    }
}
