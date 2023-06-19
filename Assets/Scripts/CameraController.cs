using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float transition;
    [SerializeField] private Color[] colors;
    [SerializeField] private float transitionTime;

    private Camera cam;
    private float currentX;
    private float currentY;
    private int colorIndex;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        cam = GetComponent<Camera>();
        colorIndex = 0;
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(currentX, currentY, transform.position.z), ref velocity, speed);
        cam.backgroundColor = Color.Lerp(cam.backgroundColor, colors[colorIndex], transitionTime * Time.deltaTime);
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
