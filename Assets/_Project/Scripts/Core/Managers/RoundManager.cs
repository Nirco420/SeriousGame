using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private BuildingUI ui;
    [SerializeField] private List<SegmentStorage> storages = new List<SegmentStorage>();
    private int currentRound {  get; set; }
    

    private void OnEnable()
    {
        ui.OnNextRoundPressed += StartNextRound;
    }

    public void StartNextRound(bool isRoundFinished)
    {
        if (isRoundFinished == false)
        {
            Debug.Log("Runde wurde noch nicht beendet");
            return;
        }

        Debug.Log("Runde wurde erfolgreich beendet"); 
        foreach (SegmentStorage storage in storages)
        {
            storage.ProducePerRound();
        }
        currentRound++;
    }

    private void OnDisable()
    {
        ui.OnNextRoundPressed -= StartNextRound;
    }

  
}
