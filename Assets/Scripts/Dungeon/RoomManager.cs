using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera roomCamera;
    [SerializeField] private List<TeleportPad> pads;

    private void Start()
    {
        pads.ForEach(pad => pad.LinkedRoom = this);
    }

    public void Activate()
    {
        roomCamera.Priority = 15;
    }

    public void Deactivate()
    {
        roomCamera.Priority = 5;
    }
}