using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private BuildingUI ui;
    [SerializeField] private List<SegmentStorage> storages = new List<SegmentStorage>();
    [SerializeField] private TransportManager transport;
    private int currentRound {  get; set; }
    

    private void OnEnable() //Event von der Ui abonnieren
    {
        ui.OnNextRoundPressed += StartNextRound;
    }

    public void StartNextRound(bool isRoundFinished) //Wenn event getriggerd wird, tranport starten und resourcen Produzieren
    {
        if (isRoundFinished == false)
        {
            Debug.Log("Runde wurde noch nicht beendet");
            return;
        }

        Debug.Log("Runde wurde erfolgreich beendet");
        transport.StartTransport();
        foreach (SegmentStorage storage in storages)
        {
            storage.ProducePerRound();
        }
        currentRound++;
    }

    private void OnDisable() //Event wieder deabonnieren
    {
        ui.OnNextRoundPressed -= StartNextRound;
    }

  
}
