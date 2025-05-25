using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Trompeter : MonoBehaviour
{
	public float speed = 1;
	[Tooltip("Zeit in Sekunden, die in eine Richtung gelaufen wird")]
	public float moveTime = 5;

	public string saveProperty;

 	public UnityEvent onReachedTarget;

	private Vector3 startPosition;
	private float timer;

    void Start()
    {
		if(PlayerPrefs.GetInt(saveProperty) == 1)
		{
            StartEvent();
            Destroy(gameObject);
            return;
        }
		else
		{
			PlayerPrefs.SetInt(saveProperty, 1);
            startPosition = transform.position;
            timer = moveTime;

            if (speed > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }

			StartCoroutine(ChangeToIntroMusic());
        }

        //if (skipInEditor || (!Application.isEditor && saveProperty != ""))
        //{
        //	var played = PlayerPrefs.GetInt(saveProperty, 0) != 0;
        //	if (skipInEditor || played)
        //	{
        //		StartEvent();
        //		Destroy(gameObject);
        //		return;
        //	}
        //}
        //startPosition = transform.position;
        //timer = moveTime;

        //if (speed > 0)
        //{
        //    transform.localScale = new Vector3(-1, 1, 1);
        //}
    }

    void Update()
    {
		transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
		if (timer > 0)
		{
			timer -= Time.deltaTime;
			if (timer <= 0)
			{
				speed = -speed;
				transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
			}
		}
		else
		{
			if (speed < 0)
			{
				if (transform.position.x < startPosition.x)
				{
					StartEvent();
					Destroy(gameObject, 1.5f);
				}
			}
			else
			{
				if (transform.position.x > startPosition.x)
				{
					StartEvent();
					Destroy(gameObject, 1.5f);
				}
			}
		}
    }
	IEnumerator ChangeToIntroMusic()
	{
		yield return null;
        AudioManager.Instance.StartMusicFadeOut((int)AudioManager.MusicSongs.ArenaIntro, false, 0.1f, 1);
    }

	void StartEvent()
	{
		if (saveProperty != "")
		{
			PlayerPrefs.SetInt(saveProperty, 1);
		}
		onReachedTarget?.Invoke();
		enabled = false;
	}
}
