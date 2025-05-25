using UnityEngine;

[CreateAssetMenu(fileName = "CaterpillarPhase", menuName = "Caterpillar/Phase")]
public class CaterpillarPhaseSO : ScriptableObject
{
    public AttackPattern[] HeadAttacks;
    public AttackPattern[] TailAttacks;

	public float TimeBetweenAttacks = 1f;
    public int TransitionAtHealth = 1;
}
