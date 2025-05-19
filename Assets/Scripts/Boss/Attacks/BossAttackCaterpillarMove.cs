using System;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackCaterpillarMove : BossAttackBase
{
    public List<Transform> HeadSlots;
    public List<Transform> TailSlots;
    public float MovementSpeed = 1f;
    public bool IsHead = false;
    [NonSerialized] public int CurrentSlot = 0;

    public override void StartAttack()
    {
        System.Random _random = new();
        var spawnSlot = FindFirstObjectByType<Boss_Caterpillar_SpawnSlot>();

        // Assumes IsHead branch executes before the else branch
        if (IsHead)
        {
            CurrentSlot = _random.Next(HeadSlots.Count);
            transform.position = HeadSlots[CurrentSlot].position;

            if (spawnSlot != null)
            {
                spawnSlot.SpawnSlots = HeadSlots;
                spawnSlot.HeadSlotIndex = CurrentSlot;
            }
        }
        else
        {
            int newSlot;
            do
            {
                newSlot = _random.Next(TailSlots.Count);
            } while (newSlot == CurrentSlot);

            CurrentSlot = newSlot;
            transform.position = TailSlots[CurrentSlot].position;

            if (spawnSlot != null)
            {
                spawnSlot.TailSlotIndex = CurrentSlot;
            }
        }
    }
}
