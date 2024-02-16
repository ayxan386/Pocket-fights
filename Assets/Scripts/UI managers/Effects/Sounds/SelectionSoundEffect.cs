using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionSoundEffect : MonoBehaviour, ISelectHandler
{
    [SerializeField] private UiSoundEffect selectionEffect;
        
    public void OnSelect(BaseEventData eventData)
    {
       selectionEffect.Play(); 
    }

}