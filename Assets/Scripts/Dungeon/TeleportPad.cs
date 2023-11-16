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

    private bool OnCooldown { get; set; }

    public RoomManager LinkedRoom { get; set; }

    public bool IsLinked => linkedPad != null;

    public TelepadColor Color => color;

    private void FixedUpdate()
    {
        if (linkedPad != null
            && Physics.CheckSphere(transform.position, distance, playerLayer))
        {
            StartCoroutine(TeleportPlayer());
        }
    }

    private IEnumerator TeleportPlayer()
    {
        if (linkedPad.OnCooldown || OnCooldown) yield break;
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

    private IEnumerator CooldownPeriod()
    {
        yield return new WaitForSeconds(cooldownPeriod);
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
    Red
}