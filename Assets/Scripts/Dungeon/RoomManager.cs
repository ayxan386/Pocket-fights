using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera roomCamera;
    [SerializeField] private List<TeleportPad> pads;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private string comparisonName;

    [Header("Floor generation")]
    [SerializeField] private Transform floorGenerationPoint;

    [SerializeField] private Transform capLayer;

    [SerializeField] private Vector2Int dimensions;
    [SerializeField] private Vector3 sizeOfCell;
    [SerializeField] private Vector3 offset;

    [SerializeField] private List<BlockChance> blocks;
    [SerializeField] private List<string> floorBlocks;
    [SerializeField] private List<TopBottomPairs> topBottomPairsList;
    [SerializeField] private LayerMask obstructionLayer;
    [SerializeField] private float detectionRadius;

    [Header("Game of life params")]
    [SerializeField] private string joiningCellName = "water";

    [SerializeField] private int cellNumberThreshold;
    [SerializeField] private int deadCellCount = 1;
    [SerializeField] private string deadCellName;

    [Header("Room decor")]
    [SerializeField] private List<BlockChance> decorPrefabs;

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

    [SerializeField] private List<Transform> specialtyBlockSpawnPoints;

    [Header("Exit condition")]
    [SerializeField] private ExitConditionType exitConditionType;

    [SerializeField] private SpawnerController targetSpawner;
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
        if(!isActive) return;
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
            case ExitConditionType.SpawnerExhaust:
                if (targetSpawner.IsExhausted) CanExit = true;
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

    [ContextMenu("Random bottom floor generation")]
    public void GenerateFloor()
    {
        DeleteAllChildren(floorGenerationPoint);
        floorBlocks.Clear();
        offset.x = Random.Range(0, 10000);
        offset.y = Random.Range(0, 10000);

        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                var pos = floorGenerationPoint.position;
                pos.x += sizeOfCell.x * x;
                pos.z += sizeOfCell.z * y;
                var block = GetRandomBlock(pos.x, pos.z, blocks);

                // var newBlock = PrefabUtility.InstantiatePrefab(block.block) as GameObject;
                // newBlock.transform.position = pos;
                // newBlock.transform.rotation = Quaternion.identity;
                // newBlock.transform.SetParent(floorGenerationPoint);
                //
                floorBlocks.Add(block.type);
            }
        }
    }

    [ContextMenu("Random upper floor generation")]
    public void GenerateUpperFloor()
    {
        DeleteAllChildren(capLayer);
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                var pos = floorGenerationPoint.position;
                pos.x += sizeOfCell.x * x;
                pos.z += sizeOfCell.z * y;
                pos.y += 1;
                var bottomLayer = floorBlocks[GetIndex(y, x)];
                var pair = topBottomPairsList.Find(pair => pair.bottomName == bottomLayer);

                // var newBlock = PrefabUtility.InstantiatePrefab(pair.top) as GameObject;
                // newBlock.transform.position = pos;
                // newBlock.transform.rotation = Quaternion.identity;
                // newBlock.transform.SetParent(capLayer);
            }
        }
    }

    public void GameOfLife()
    {
        var temp = new List<string>();
        for (int y = 0; y < dimensions.y; y++)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                var index = GetIndex(y, x);
                var bottomLayer = floorBlocks[index];
                var numberOfWaterCells =
                    IsWater(y, x + 1) + IsWater(y, x - 1) + IsWater(y + 1, x) + IsWater(y - 1, x)
                    +IsWater(y+1, x + 1) + IsWater(y+1, x - 1) + IsWater(y - 1, x + 1) + IsWater(y - 1, x - 1);
                if (bottomLayer != joiningCellName)
                {
                    temp.Add(numberOfWaterCells >= cellNumberThreshold ? joiningCellName : bottomLayer);
                }
                else
                {
                    temp.Add(numberOfWaterCells <= deadCellCount ? deadCellName : floorBlocks[index]);
                }
            }
        }

        floorBlocks = temp;

        GenerateUpperFloor();
    }

    private int IsWater(int y, int x)
    {
        var index = GetIndex(y, x);

        if (index < 0 || index >= floorBlocks.Count) return 0;

        return floorBlocks[index] == joiningCellName ? 1 : 0;
    }

    private int GetIndex(int y, int x)
    {
        return y + x * dimensions.x;
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

        GenerateDecorPositions();

        Gizmos.color = Color.cyan;
        foreach (var pos in decorPosition)
        {
            Gizmos.DrawSphere(pos, 0.4f);
        }
    }

    private void GenerateDecorPositions()
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
        var numberOfSpecialtyBlocks = Random.Range(0, specialtyBlockSpawnPoints.Count);
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
        if (allSpawners.Length == 1)
        {
            var spawnerData = allSpawners[0].SetSpawnerData();
            targetSpawner = allSpawners[0];
            
            killCounter = spawnerData.numberOfMobsLeft.Aggregate((num,sum) => sum + num);
            exitConditionType = ExitConditionType.SpawnerExhaust;
        }
        else
        {
            var maxAverage = -1;
            foreach (var spawner in allSpawners)
            {
                var spawnerData = spawner.SetSpawnerData();
                var average = (int)spawnerData.numberOfMobsLeft.Average(data => data);
                maxAverage = Mathf.Max(average, maxAverage);
            }

            killCounter = maxAverage;
            exitConditionType = ExitConditionType.KillCounter;
        }
        
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