using NUnit.Framework.Constraints;
using UnityEngine;

public class MainStorrage : MonoBehaviour
{
    public int wood {  get; private set; }
    public int wheed { get; private set; }
    public int flachs { get; private set; }

    public int gold {  get; private set; }


    public void AddResources(int amount, ResourceData resource) //Prüft, welche Ressource akommt und verändert dementsprechend den Lagerwert
    {
        switch (resource.resourceType)
        {
            case ResourceData.ResourceType.WHEED:
                wheed += amount;
                break;

            case ResourceData.ResourceType.FLACHS:
                flachs += amount; 
                break;

            case ResourceData.ResourceType.WOOD:
                wood += amount;
                break;
        }
    }

}
