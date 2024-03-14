using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private List<TeleportPad> pads;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private string comparisonName;

    [Header("Obstruction detection")] [SerializeField]
    private Transform capLayer;

    [SerializeField] private Vector2Int dimensions;
    [SerializeField] private Vector3 offset;

    [SerializeField] private LayerMask obstructionLayer;
    [SerializeField] private float detectionRadius;

    [Header("Room decor")] [SerializeField]
    private List<BlockChance> decorPrefabs;

    [SerializeField] [Range(0, 1f)] private float density;
    [SerializeField] private Transform decorHolder;
    [SerializeField] private bool showGizmos;
    [SerializeField] private LayerMask forbiddenLayers;
    [SerializeField] private Transform layerToPlaceDecorOnTop;
    [SerializeField] private List<Vector3> decorPosition;
    [SerializeField] private bool randomSeed;
    [SerializeField] private int currentSeed;

    [Header("Room specialty")]
    [SerializeField] private List<BlockChance> specialtyBlockPrefabs;

    [SerializeField] private int maxNumberOfSpecialtyBlocks = 2;

    [Range(1f, 2f)]
    [SerializeField] private float difficulty;

    [SerializeField] private List<Transform> specialtyBlockSpawnPoints;

    [Header("Exit condition")] [SerializeField]
    private ExitConditionType exitConditionType;

    [SerializeField] private int killCounter;
    [SerializeField] private ExitConditionDisplayManager exitConditionDisplayManager;

    private List<GridPoint> grid;
    private bool isActive;

    public List<TeleportPad> Telepads => pads;
    public string ComparisonName => comparisonName;
    public Vector2Int GridSize => dimensions;
    public List<GridPoint> Grid => grid;

    public bool CanExit { get; private set; } = true;

    private void Start()
    {
        pads.ForEach(pad => pad.LinkedRoom = this);
        CanExit = false;
        CheckExitConditions();
        FindAllHits();
    }

    private void OnMobDeath(MobController obj)
    {
        if (!isActive) return;
        killCounter--;
        CheckExitConditions();

        exitConditionDisplayManager.UpdateDisplay(killCounter, CanExit);
    }

    private void CheckExitConditions()
    {
        switch (exitConditionType)
        {
            case ExitConditionType.None:
                CanExit = true;
                return;
            case ExitConditionType.KillCounter:
                if (killCounter <= 0) CanExit = true;
                break;
            default:
                print("Unknown exit condition");
                break;
        }
    }

    public void Activate()
    {
        isActive = true;
        EventManager.OnMobDeath += OnMobDeath;
        exitConditionDisplayManager.gameObject.SetActive(exitConditionType != ExitConditionType.None);
    }

    public void Deactivate()
    {
        isActive = false;
        EventManager.OnMobDeath -= OnMobDeath;
        exitConditionDisplayManager.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.OnMobDeath -= OnMobDeath;
    }

    public void PlacePlayer()
    {
        PlayerInputController.Instance.PlacePlayer(playerSpawnPoint);
    }

    public bool CanConnectToDirection(TelepadColor desiredColor)
    {
        return pads.Exists(pad => pad.Color == desiredColor);
    }

    public bool HasUnlinkedTelepad()
    {
        return pads.Exists(pad => !pad.IsLinked);
    }

    [ContextMenu("Delete all children")]
    public void DeleteAllChildren(Transform parent)
    {
        for (int i = 0; i < 10; i++)
        {
            for (int index = 0; index < parent.childCount; index++)
            {
                DestroyImmediate(parent.GetChild(index).gameObject);
            }
        }
    }

    private BlockChance GetRandomBlock(float x, float y, List<BlockChance> blockCollection)
    {
        var chance = Mathf.PerlinNoise(x + offset.x, y + offset.y);
        foreach (var tuple in blockCollection)
        {
            if (chance >= tuple.weight.x && chance <= tuple.weight.y)
            {
                return tuple;
            }
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        foreach (var gridPoint in grid)
        {
            Gizmos.color = gridPoint.isObstructed ? Color.red : Color.cyan;
            Gizmos.DrawSphere(gridPoint.pos, 0.3f);
        }
    }

    [ContextMenu("Generate decor positions")]
    public void GenerateDecorPositions()
    {
        decorPosition = new List<Vector3>();
        if (randomSeed)
        {
            currentSeed = (int)(Random.value * Random.Range(1000, 123456));
        }

        Random.InitState(currentSeed);
        for (int childIndex = 0; childIndex < layerToPlaceDecorOnTop.childCount; childIndex++)
        {
            if (Random.value <= density)
            {
                var capBlock = layerToPlaceDecorOnTop.GetChild(childIndex);
                if ((forbiddenLayers & (1 << capBlock.gameObject.layer)) != 0) continue;
                decorPosition.Add(capBlock.position);
            }
        }
    }

    private void FindAllHits()
    {
        grid = new List<GridPoint>();
        for (var x = 0; x < dimensions.x; x += 1)
        {
            for (var z = 0; z < dimensions.y; z += 1)
            {
                var pos = capLayer.GetChild(x * dimensions.y + z).transform.position;
                pos.y += .3f;

                var gridPoint = new GridPoint(pos, z + x * dimensions.y);
                grid.Add(gridPoint);
                gridPoint.isObstructed = Physics.CheckSphere(pos, detectionRadius, obstructionLayer);
            }
        }
    }

    [ContextMenu("Place decors")]
    public void PlaceDecors()
    {
        DeleteAllChildren(decorHolder);
        foreach (var decorPos in decorPosition)
        {
            var randomDecor = GetRandomBlock(decorPos.x, decorPos.z, decorPrefabs);
            // var newBlock = PrefabUtility.InstantiatePrefab(randomDecor.block) as GameObject;
            // newBlock.transform.position = decorPos + randomDecor.placementOffset;
            // newBlock.transform.rotation = Quaternion.identity;
            // newBlock.transform.SetParent(decorHolder);
        }
    }

    public void PlaceSpecialtyBlocks(float difficulty)
    {
        var numberOfSpecialtyBlocks = specialtyBlockSpawnPoints.Count * this.difficulty * Random.Range(0.3f, 0.8f);
        numberOfSpecialtyBlocks = Mathf.Min(numberOfSpecialtyBlocks, maxNumberOfSpecialtyBlocks);
        var usedPoints = new HashSet<int>();
        for (var i = 0; i < numberOfSpecialtyBlocks; i++)
        {
            var roll = Mathf.Clamp01(Random.value + difficulty);
            var block =
                specialtyBlockPrefabs.Find(blockChance =>
                    blockChance.weight.x <= roll && blockChance.weight.y >= roll);

            if (block == null) continue;

            var randomPoint = Random.Range(0, specialtyBlockSpawnPoints.Count);
            var it = 0;
            while (usedPoints.Contains(randomPoint) && it < 100)
            {
                it++;
                randomPoint = Random.Range(0, specialtyBlockSpawnPoints.Count);
            }

            usedPoints.Add(randomPoint);
            var position = specialtyBlockSpawnPoints[randomPoint].position + block.placementOffset;
            Instantiate(block.block, position, Quaternion.identity, transform);
        }

        if (usedPoints.Count == 0)
        {
            var block = specialtyBlockPrefabs[0];
            var position = specialtyBlockSpawnPoints[Random.Range(0, specialtyBlockSpawnPoints.Count)].position
                           + block.placementOffset;
            Instantiate(block.block, position, Quaternion.identity, transform);
        }
    }

    public void SetExitConditions()
    {
        var allSpawners = GetComponentsInChildren<SpawnerController>();
        if (allSpawners.Length == 0)
        {
            exitConditionDisplayManager.gameObject.SetActive(false);
            return;
        }

        var maxAverage = -1;
        foreach (var spawner in allSpawners)
        {
            var spawnerData = spawner.SetSpawnerData();
            spawnerData.numberOfMobsLeft = spawnerData.numberOfMobsLeft.ConvertAll(number => Mathf.RoundToInt(number * difficulty));
            maxAverage += spawnerData.numberOfMobsLeft.Sum(data => data);
        }

        killCounter = Mathf.FloorToInt(maxAverage * Random.Range(0.3f, 0.6f));
        exitConditionType = ExitConditionType.KillCounter;

        CanExit = false;
        exitConditionDisplayManager.UpdateDisplay(killCounter, CanExit);
    }
}

[Serializable]
public class BlockChance
{
    public Vector3 placementOffset;
    public string type;
    public Vector2 weight;
    public GameObject block;
}

[Serializable]
public class TopBottomPairs
{
    public string bottomName;
    public GameObject top;
}

[Serializable]
public enum ExitConditionType
{
    None,
    KillCounter,
    SpawnerExhaust
}