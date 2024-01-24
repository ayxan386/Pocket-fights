using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField] private List<RoomManager> roomPrefabs;
    [SerializeField] private RoomManager endRoom;
    [SerializeField] private List<RoomManager> roomInstances;
    [SerializeField] private Vector2 distanceBetweenRooms;
    [SerializeField] private Vector2Int numberOfRooms;
    [SerializeField] private bool autoGenerate = true;
    private int maxRooms;

    public static FloorManager Instance { get; private set; }

    public List<RoomManager> Rooms => roomInstances;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        if (autoGenerate)
        {
            yield return new WaitForSeconds(0.1f);
            GenerateFloor();
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitUntil(() => PlayerInputController.Instance != null);
        roomInstances[0].PlacePlayer();
        roomInstances[0].Activate();
    }

    [ContextMenu("Generate floor")]
    public void GenerateFloor()
    {
        roomInstances = new List<RoomManager>();
        var randomRoomIndex = Random.Range(0, roomPrefabs.Count);
        var randomRoom = roomPrefabs[randomRoomIndex];
        randomRoom = Instantiate(randomRoom, Vector3.zero, Quaternion.identity, transform);
        randomRoom.PlaceSpecialtyBlocks(0);
        randomRoom.SetExitConditions();

        roomInstances.Add(randomRoom);

        var iteration = 0;
        maxRooms = Random.Range(numberOfRooms.x, numberOfRooms.y);
        while (roomInstances.Count < maxRooms - 1 && roomInstances.Exists(room => room.HasUnlinkedTelepad())
                                                  && iteration < 100)
        {
            CreateRoomForEachPad(FindRoomWithUnlinkedTelepad());
            iteration++;
        }

        AddSpecificRoom(endRoom, FindRoomWithUnlinkedTelepad());
    }

    private RoomManager FindRoomWithUnlinkedTelepad()
    {
        return roomInstances.Find(room => room.HasUnlinkedTelepad());
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
                var transform1 = transform;
                var pos = transform1.position;
                pos.x += distanceBetweenRooms.x * roomInstances.Count;

                potentialRoom = Instantiate(potentialRoom, pos, Quaternion.identity, transform1);
                potentialRoom.PlaceSpecialtyBlocks(1f * roomInstances.Count / maxRooms / 10f);
                potentialRoom.SetExitConditions();
                roomInstances.Add(potentialRoom);

                potentialRoom.Telepads.Find(pad => pad.Color == telepad.Color && !pad.IsLinked).Link(telepad);
            }
            
        }
    }

    private void AddSpecificRoom(RoomManager roomToAdd, RoomManager currentRoom)
    {
        foreach (var telepad in currentRoom.Telepads)
        {
            if (telepad.IsLinked) continue;

            var potentialRoom = roomToAdd;

            if (potentialRoom != null)
            {
                var pos = transform.position;
                pos.x += distanceBetweenRooms.x * roomInstances.Count;

                potentialRoom = Instantiate(potentialRoom, pos, Quaternion.identity, transform);
                potentialRoom.PlaceSpecialtyBlocks(1f * roomInstances.Count / maxRooms / 10f);
                potentialRoom.SetExitConditions();
                roomInstances.Add(potentialRoom);

                potentialRoom.Telepads.Find(pad => pad.Color == telepad.Color && !pad.IsLinked).Link(telepad);
            }
            break;
        }
    }
}