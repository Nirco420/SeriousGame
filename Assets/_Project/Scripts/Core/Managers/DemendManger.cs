using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DemendManger : MonoBehaviour
{
    public event Action<DemandData, int> OnDemandDisplayRequested;

    [SerializeField]private List<DemandData> demands = new List<DemandData>();
    private DemandData currentDemend;
    private int nextDemandIndex;
    private int dueRound;

    public void TriggerDemand(DemandData demand, int currentRound) //Erhält die aktuelle Forderung, berechnet die Deadline und setzt dann den Index hoch.
    {
        if (currentDemend != null)
        {
            return;
        }

        currentDemend = demand;
        dueRound = currentRound + demand.deadline;
        nextDemandIndex++;

        OnDemandDisplayRequested?.Invoke(demand, currentRound);
    }

    public void CheckDemandsForRound(int currentRound) //Prüfen ob schoon eine Forderung aktiv und ob in der Liste noch eine ist. Wenn ja Demand Triggern
    {
        if(currentDemend != null)
        {
            return;
        }

        if (nextDemandIndex >= demands.Count)
        {
            return;
        }

        DemandData nextDemand = demands[nextDemandIndex];

        if (currentRound < nextDemand.triggerRound)
        {
            return;
        }

        TriggerDemand(nextDemand, currentRound);
    }
}
