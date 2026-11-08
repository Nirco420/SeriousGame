using UnityEngine;

[CreateAssetMenu(fileName = "ResourceData", menuName = "Scriptable Objects/ResourceData")]
public class ResourceData : ScriptableObject
{
    public enum ResourceType
    {
        None,
        WOOD,
        WHEED,
        FLAX,
    }

    public string name;
    public Sprite icon;
}
