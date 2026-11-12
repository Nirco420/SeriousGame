using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private BuildingUI ui;
    [SerializeField] private List<SegmentStorage> storages = new List<SegmentStorage>();
    [SerializeField] private TransportManager transport;
    [SerializeField] private DemendManger demendManger;
    public int currentRound {  get; private set; }
    

    private void OnEnable() //Event von der Ui abonnieren
    {
        ui.OnNextRoundPressed += StartNextRound;
    }

    public void StartNextRound(bool isRoundFinished) //Wenn event getriggerd wird, tranport starten und resourcen Produzieren
    {
        if (isRoundFinished == false)
        {
            return;
        }

        transport.StartTransport();
        foreach (SegmentStorage storage in storages)
        {
            storage.ProducePerRound();
        }
        currentRound++;
        demendManger.CheckDemandsForRound(currentRound);
    }

    private void OnDisable() //Event wieder deabonnieren
    {
        ui.OnNextRoundPressed -= StartNextRound;
    }

  
}
