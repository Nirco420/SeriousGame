using UnityEngine;

[CreateAssetMenu(fileName = "DemandData", menuName = "Scriptable Objects/DemandData")]
public class DemandData : ScriptableObject
{
    public string name;
    public string description;
    public ResourceData resource;
    public int quantity;
    public int triggerRound;
    public int deadline;
    public int changeInValuePositive;
    public int changeInValueHalf;
    public int changeInValueNegative;
    public int changeInDeferalInValue;
    public int deferralPeriod;


}
