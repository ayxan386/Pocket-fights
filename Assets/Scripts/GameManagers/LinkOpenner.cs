using UnityEngine;

public class LinkOpenner : MonoBehaviour
{
    [SerializeField] private string url;

    public void OpenUrl()
    {
        Application.OpenURL(url);
    }
}