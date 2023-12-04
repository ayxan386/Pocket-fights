using UnityEngine;

public class ColorChangingAction : MonoBehaviour
{
    [SerializeField] private Renderer target;

    [SerializeField] private Color exhaustedColor;

    public void Use()
    {
        target.material.color = exhaustedColor;
    }
}