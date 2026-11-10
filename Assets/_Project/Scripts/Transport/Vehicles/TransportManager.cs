using Mono.Cecil;
using System.Collections;
using UnityEngine;

public class TransportManager : MonoBehaviour
{
    [SerializeField] private SegmentStorage segmentStorage;
    [SerializeField] private MainStorrage mainStorage;
    [SerializeField] private Transform startStoragPos;
    [SerializeField] private Transform mainStoragePos;
    [SerializeField] private GameObject cart;

    [SerializeField] private int transportCapacity;
    public int currentLoad {  get; private set; }
    public float moveSpeed;

    private bool isDriving = false;

    public ResourceData resourceData {  get; private set; }

    public void StartTransport() //Startet den Transport, 
    {
        if(isDriving == true)
        {
            return;
        }

        currentLoad = segmentStorage.RemoveResources(transportCapacity); //Aus dem Segmentlager soviele Ressourcen holen wie der wagen platz hat

        if (currentLoad == 0)
        {
            return;
        }

        resourceData = segmentStorage.storedResource; //Resource Data übergeben
        isDriving = true;
        StartCoroutine(TranportLifeCycle(startStoragPos, mainStoragePos, moveSpeed)); //Coroutine für Wagen Animation starten
        
    }

    public IEnumerator MoveToMainStorage(Transform startPos, Transform targetPos, float kmh ) //Aanimation von Startposition zu Endpostion
    {
        while(Vector3.Distance(cart.transform.position, targetPos.transform.position) > 0.05f)
        {
            cart.transform.position = Vector3.MoveTowards(cart.transform.position, targetPos.transform.position, kmh * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator MoveBackToSegmentStorage(Transform targetPos, Transform startPos, float kmh) //Animation von EndPosition wieder zurück
    {
        while(Vector3.Distance(cart.transform.position, startPos.transform.position) > 0.05f)
        {
            cart.transform.position = Vector3.MoveTowards(cart.transform.position, startPos.transform.position, kmh * Time.deltaTime);
            yield return null;
        }
    }

    public IEnumerator TranportLifeCycle(Transform startPos, Transform targetPos, float kmh) //Corountineablauf
    {
        if (isDriving == true)
        {
            yield return StartCoroutine(MoveToMainStorage(startPos, targetPos, kmh));

            cart.SetActive(false);
            mainStorage.AddResources(currentLoad, resourceData);
            currentLoad = 0;

            yield return new WaitForSeconds(2f);

            cart.SetActive(true);
           
            yield return StartCoroutine(MoveBackToSegmentStorage(targetPos, startPos, kmh));
            isDriving = false;
        }
    }
    }
