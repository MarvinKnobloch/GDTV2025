using System.Collections;
using UnityEngine;

public class Boss3ChargeBee : MonoBehaviour
{
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    private float chargeSpeed;
    [SerializeField] private float lifeTime = 3;

    private void OnEnable()
    {
        transform.position = startPosition.position;
        StartCoroutine(Deactivate());
    }
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, endPosition.position, chargeSpeed * Time.deltaTime);
    }
    public void SetValues(float speed)
    {
        chargeSpeed = speed;
    }
    IEnumerator Deactivate()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.Instance.health.PlayerTakeDamage(1, false);
        }
    }
}
