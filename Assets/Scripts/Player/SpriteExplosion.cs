using UnityEngine;

public class SpriteExplosion : MonoBehaviour
{
	public float directionalForce = 10f;
	public float upForce = 2f;
	public float randomForce = 1f;
	public float partRotationForce = 5f;

	
    void Start()
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody2D>())
        {
            // Berechne die Richtung vom Zentrum zum Teil
            Vector2 direction = (rb.transform.position - transform.position).normalized;
            // Füge zufällige Richtung hinzu
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb.AddForce(direction * directionalForce + Vector2.up * upForce + randomDirection * randomForce, ForceMode2D.Impulse);
            
            // Füge zufällige Rotation hinzu (±50% Streuung)
            float randomRotation = partRotationForce * Random.Range(-1f, 1f);
            rb.AddTorque(Random.value > 0.5f ? randomRotation : -randomRotation, ForceMode2D.Impulse);
        }
    }

}
