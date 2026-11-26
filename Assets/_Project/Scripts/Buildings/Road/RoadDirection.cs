using UnityEngine;

public class RoadDirection : MonoBehaviour
{
    [SerializeField] private GameObject rightArrow;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject upperArrow;
    [SerializeField] private GameObject downArrow;

    public void GetDirection (Vector2Int direction, bool isFree)
    {
        if (direction.x > 0)
        {
            rightArrow.SetActive(isFree);
            
        }
        else if ( direction.x < 0)
        {
            leftArrow.SetActive(isFree);
        }
        else if (direction.y > 0)
        {
            upperArrow.SetActive(isFree);
        }
        else if ( direction.y < 0)
        {
            downArrow.SetActive(isFree);
        }

    }

    public void DeactivateArrows()
    {
        rightArrow.SetActive(false);
        leftArrow.SetActive(false);
        upperArrow.SetActive(false);
        downArrow.SetActive(false);
    }
}
