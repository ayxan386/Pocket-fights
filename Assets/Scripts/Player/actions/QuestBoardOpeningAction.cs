using UnityEngine;

public class QuestBoardOpeningAction : MonoBehaviour
{
    [SerializeField] private UiAnimation questBoardAnim;
    [SerializeField] private Transform questHolder;
    private bool canOpen = true;

    private void Start()
    {
        EventManager.OnPauseMenuToggled += OnPauseMenuToggled;
    }

    private void OnDestroy()
    {
        EventManager.OnPauseMenuToggled -= OnPauseMenuToggled;
    }

    private void OnPauseMenuToggled(bool obj)
    {
        canOpen = !obj;
        if (!canOpen) questBoardAnim.Disappear();
    }

    public void OpenBoard()
    {
        if (!canOpen) return;
        questBoardAnim.Appear();
        QuestManager.Instance.OpenUi(questHolder);
        PlayerInputController.Instance.State.isLookingAtQuests = true;
    }
}