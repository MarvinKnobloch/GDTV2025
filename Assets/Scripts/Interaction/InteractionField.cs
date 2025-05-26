using UnityEngine;

public class InteractionField : MonoBehaviour
{
    [SerializeField] private float yOffset;
    private void LateUpdate()
    {
        transform.position = Player.Instance.transform.position + transform.up * yOffset;
    }
}
