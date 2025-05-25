using UnityEngine;

public class DropShadow : MonoBehaviour
{
    public float maxDistance = 2f;  // Maximaler Abstand für Ausfaden
    public float minAlpha = 0f;     // Minimale Transparenz
    public float maxAlpha = 1f;     // Maximale Transparenz

    private Transform parent;
    private new SpriteRenderer renderer;
    private float relativeY;
	private float absoluteY; // Ground Level
    private float relativeX;

    void Start()
    {
        parent = transform.parent;
        renderer = GetComponent<SpriteRenderer>();
        if (parent == null)
        {
            Debug.LogWarning("DropShadow benötigt ein Parent-Objekt!");
        }
        if (renderer == null)
        {
            Debug.LogWarning("DropShadow benötigt einen SpriteRenderer!");
        }
        if (parent != null)
        {
			relativeY = transform.position.y - parent.position.y;
            absoluteY = parent.position.y + relativeY;
            relativeX = transform.position.x - parent.position.x;
        }
    }

    void Update()
    {
        if (parent == null || renderer == null) return;

        UpdatePosition();
        UpdateAlpha();
    }

	void UpdatePosition()
	{
		Vector3 pos = transform.position;
		pos.x = parent.position.x + relativeX * parent.localScale.x;
		pos.y = absoluteY;
		transform.position = pos;
	}

	void UpdateAlpha()
	{
		float distance = Mathf.Abs(absoluteY - (parent.transform.position.y + relativeY));
		float alpha = Mathf.Lerp(maxAlpha, minAlpha, distance / maxDistance);
		alpha = Mathf.Clamp(alpha, minAlpha, maxAlpha);
		Color c = renderer.color;
		c.a = alpha;
		renderer.color = c;
	}
}
