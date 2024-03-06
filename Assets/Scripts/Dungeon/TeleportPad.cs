using System.Collections;
using UnityEngine;

public class TeleportPad : MonoBehaviour
{
    [Header("Player check")] [SerializeField]
    private float distance;

    [SerializeField] private LayerMask playerLayer;

    [Header("Teleportation")] [SerializeField]
    private TeleportPad linkedPad;

    [SerializeField] private float duration;
    [SerializeField] private float cooldownPeriod;
    [SerializeField] private Transform teleportPoint;
    [SerializeField] private TelepadColor color;

    [Header("FX")] [SerializeField] private ParticleSystem teleportParticles;
    private Coroutine teleportRoutine;

    private bool OnCooldown { get; set; }

    public RoomManager LinkedRoom { get; set; }

    public bool IsLinked => linkedPad != null;

    public TelepadColor Color => color;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(5f);
        if (linkedPad == null) Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        if (linkedPad != null && !LinkedRoom.CanExit || linkedPad.OnCooldown || OnCooldown) return;

        if (Physics.CheckSphere(transform.position, distance, playerLayer))
        {
            teleportRoutine = StartCoroutine(TeleportPlayer());
        }
        else if (teleportRoutine != null)
        {
            StopCoroutine(teleportRoutine);
        }
    }

    private IEnumerator TeleportPlayer()
    {
        OnCooldown = true;

        teleportParticles.Play();
        yield return new WaitForSeconds(duration);

        linkedPad.LinkedRoom.Activate();
        LinkedRoom.Deactivate();
        teleportParticles.Stop();
        PlayerInputController.Instance.PlacePlayer(linkedPad.teleportPoint);
        StartCoroutine(CooldownPeriod());
        linkedPad.StartCoroutine(linkedPad.CooldownPeriod());
    }

    [ContextMenu("Teleport player")]
    public void Debug_TeleportPlayer()
    {
        StartCoroutine(TeleportPlayer());
    }

    private IEnumerator CooldownPeriod()
    {
        yield return new WaitForSeconds(cooldownPeriod);
        OnCooldown = false;
    }

    private void OnDisable()
    {
        OnCooldown = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = UnityEngine.Color.blue;
        Gizmos.DrawWireSphere(transform.position, distance);
    }

    public void Link(TeleportPad otherPad)
    {
        linkedPad = otherPad;
        otherPad.linkedPad = this;
    }
}

public enum TelepadColor
{
    Blue,
    Red,
    Green,
    Purple
}