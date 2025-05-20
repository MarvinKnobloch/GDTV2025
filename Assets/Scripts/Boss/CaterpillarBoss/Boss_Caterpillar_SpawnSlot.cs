using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Caterpillar_SpawnSlot : MonoBehaviour
{
    [NonSerialized] public List<Transform> SpawnSlots;
    [NonSerialized] public int HeadSlotIndex = 0;
    [NonSerialized] public int TailSlotIndex = 0;

    public Transform GetSpawnSlot()
    {
        for (int i = 0; i < SpawnSlots.Count; i++)
        {
            if (i == HeadSlotIndex || i == TailSlotIndex)
            {
                continue;
            }

            return SpawnSlots[i];
        }

        return null;
    }
}
