using UnityEngine;

public class BeeCircle : MonoBehaviour
{
    [SerializeField] private Transform startPosition, endPosition;
    [SerializeField] private float travelSpeed;
    [SerializeField] private float rotationspeed;

    private void OnEnable()
    {
        transform.position = startPosition.position;
        transform.Rotate(0, 0, 0);
    }
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, endPosition.position, travelSpeed * Time.deltaTime);

        transform.Rotate(0, 0, rotationspeed);
    }
}
