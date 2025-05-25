using System;
using UnityEngine;

public class CrateHoney : MonoBehaviour
{

    [SerializeField] private GameObject honeyDrop;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private SpriteRenderer defaultSpriteRenderer;
    //Animations
    [NonSerialized] public Animator currentAnimator;
    [NonSerialized] public string currentstate;
    const string idleState = "Idle";
    const string crateState = "Crate";



    private void Awake()
    {
        currentAnimator = GetComponent<Animator>();
    }

    public void CrateDrop()
    {
        ChangeAnimationState(crateState);
    }
    public void SpawnDrop()
    {
        GameObject prefab = Instantiate(honeyDrop, spawnPosition.position, Quaternion.identity);

        prefab.transform.Rotate(0, 0, 0);
        //prefab.transform.right = transform.right;
        ChangeAnimationState(idleState);
    }
    public void ChangeAnimationState(string newstate)
    {
        if (currentstate == newstate) return;
        currentstate = newstate;
        if (currentAnimator == null) return;

        currentAnimator.CrossFadeInFixedTime(newstate, 0.1f);
    }
    public void ChangeSprite()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        defaultSpriteRenderer.enabled = false;
    }
}
