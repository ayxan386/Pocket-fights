using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<RoomManager> roomPrefabs;
    [SerializeField] private List<RoomManager> roomInstances;
    [SerializeField] private Vector2 distanceBetweenRooms;
    [SerializeField] private Vector2Int numberOfRooms;

    private void Start()
    {
        roomInstances[0].PlacePlayer();
        roomInstances[0].Activate();
    }

    [ContextMenu("Generate floor")]
    public void GenerateFloor()
    {
        roomInstances = new List<RoomManager>();
        var randomRoomIndex = Random.Range(0, roomPrefabs.Count);
        var randomRoom = roomPrefabs[randomRoomIndex];
        randomRoom = Instantiate(randomRoom, transform);

        roomInstances.Add(randomRoom);

        var iteration = 0;
        var maxRooms = Random.Range(numberOfRooms.x, numberOfRooms.y);
        while (roomInstances.Count < maxRooms && roomInstances.Exists(room => room.HasUnlinkedTelepad())
                                              && iteration < 100)
        {
            CreateRoomForEachPad(roomInstances.Find(room => room.HasUnlinkedTelepad()));
            iteration++;
        }
    }

    private void CreateRoomForEachPad(RoomManager currentRoom)
    {
        foreach (var telepad in currentRoom.Telepads)
        {
            if (telepad.IsLinked) continue;

            var potentialRooms =
                roomPrefabs.FindAll(room =>
                    room.ComparisonName != currentRoom.ComparisonName
                    && room.CanConnectToDirection(telepad.Color));

            if (potentialRooms.Count == 0) continue;
            var potentialRoom = potentialRooms[Random.Range(0, potentialRooms.Count)];

            if (potentialRoom != null)
            {
                var pos = transform.position;
                if (roomInstances.Count % 2 == 0)
                {
                    pos.x += distanceBetweenRooms.x * roomInstances.Count * (roomInstances.Count % 2 == 0 ? 1 : -1);
                }
                else
                {
                    pos.z += distanceBetweenRooms.y * roomInstances.Count;
                }

                potentialRoom = Instantiate(potentialRoom, pos, Quaternion.identity, transform);
                potentialRoom.Telepads.Find(pad => pad.Color == telepad.Color && !pad.IsLinked).Link(telepad);
                roomInstances.Add(potentialRoom);
            }
        }
    }
}