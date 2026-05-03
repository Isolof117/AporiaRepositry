using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SettingsFontFade : MonoBehaviour
{
    [Header("Texts To Fade In")]
    [SerializeField] private List<TextMeshProUGUI> textsToFadeIn;

    [Header("Texts To Fade Out")]
    [SerializeField] private List<TextMeshProUGUI> textsToFadeOut;

    [Header("Fade Settings")]
    [SerializeField] private float delay = 1f;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    public void FadeInTexts()
    {
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        // --- FADE OUT OLD TEXTS ---
        float time = 0f;

        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            float alpha = 1f - (time / fadeOutDuration);

            foreach (var text in textsToFadeOut)
            {
                if (text != null)
                {
                    Color c = text.color;
                    c.a = alpha;
                    text.color = c;
                }
            }

            yield return null;
        }

        // Ensure fully invisible + disable
        foreach (var text in textsToFadeOut)
        {
            if (text != null)
            {
                Color c = text.color;
                c.a = 0f;
                text.color = c;
                text.gameObject.SetActive(false);
            }
        }

        // --- WAIT BEFORE FADE IN ---
        yield return new WaitForSeconds(delay);

        // --- PREP NEW TEXTS ---
        foreach (var text in textsToFadeIn)
        {
            if (text != null)
            {
                text.gameObject.SetActive(true);

                Color c = text.color;
                c.a = 0f;
                text.color = c;
            }
        }

        // --- FADE IN NEW TEXTS ---
        time = 0f;

        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            float alpha = time / fadeInDuration;

            foreach (var text in textsToFadeIn)
            {
                if (text != null)
                {
                    Color c = text.color;
                    c.a = alpha;
                    text.color = c;
                }
            }

            yield return null;
        }

        // Ensure fully visible
        foreach (var text in textsToFadeIn)
        {
            if (text != null)
            {
                Color c = text.color;
                c.a = 1f;
                text.color = c;
            }
        }
    }
}