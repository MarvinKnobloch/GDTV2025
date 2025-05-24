using System.Collections;
using UnityEngine;

public class OpenGate : MonoBehaviour
{
    private Vector3 endPosi;

    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private string playerPrefString;

    private void Awake()
    {
        if(PlayerPrefs.GetInt(playerPrefString) == 1) gameObject.SetActive(false);

        endPosi = transform.GetChild(0).gameObject.transform.position;
    }
    public void MovementStart()
    {
        StartCoroutine(MoveGate());
    }
    IEnumerator MoveGate()
    {
        while (Vector2.Distance(transform.position, endPosi) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, endPosi, moveSpeed * Time.deltaTime);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
