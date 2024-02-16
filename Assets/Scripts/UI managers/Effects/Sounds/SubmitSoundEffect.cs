using UnityEngine;
using UnityEngine.EventSystems;

public class SubmitSoundEffect : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] private UiSoundEffect soundEffect;

    public void OnPointerClick(PointerEventData eventData)
    {
        soundEffect.Play();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        soundEffect.Play();
    }
}