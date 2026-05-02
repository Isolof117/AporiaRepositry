using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSoundTrigger : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (UIAudioManager.Instance != null)
            UIAudioManager.Instance.PlayClick();
    }
}