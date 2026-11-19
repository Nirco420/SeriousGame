using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class BarometerManager : MonoBehaviour
{
    [SerializeField] private float cellSize;
    [SerializeField] private RectTransform circle;

    private Vector2 middlePosition;
    [SerializeField] private float duration = 5f;

    private bool isGameOver;
    private int currentValue;

    public void Awake()
    {
        middlePosition = circle.anchoredPosition;
    }
    
    public void UpdateBarometer()
    {
        StartCoroutine(MoveCircle());
    }

    public IEnumerator MoveCircle() //Kreis per Lerp bewegen 
    {
        float time = 0;
        Vector2 startPos = circle.anchoredPosition;
        Vector2 targetPos = middlePosition + new Vector2(currentValue * cellSize, 0f);


        while (time < duration)
        {
            circle.anchoredPosition = Vector2.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        circle.anchoredPosition = targetPos;
       
    }

    public void ChangeValue(int amount) //Value von den Forderungen erhalten und dementsprechend die Richtung vorgeben
    {
        currentValue = Mathf.Clamp(currentValue + amount, -4, 4);
        UpdateBarometer();

        if (currentValue == -4 || currentValue == 4)
        {
            isGameOver = true;
            SceneManager.LoadScene("LoseScreen");
        }
    }
}
