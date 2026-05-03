using UnityEngine;
using System.Collections;

public class MenuSlide : MonoBehaviour
{
    [Header("Target to Move")]
    [SerializeField] private RectTransform target;

    [Header("Slide Settings")]
    [SerializeField] private float slideAmount = 488f;
    [SerializeField] private float duration = 0.3f;

    private static bool hasMoved = false;

    private void OnEnable()
    {
        hasMoved = false; // Reset every time scene loads
    }

    public void Slide()
    {
        if (!hasMoved)
        {
            StartCoroutine(SlideRoutine());
        }
    }

    private IEnumerator SlideRoutine()
    {
        hasMoved = true;

        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(slideAmount, 0);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, time / duration);
            yield return null;
        }

        target.anchoredPosition = endPos;
    }
}