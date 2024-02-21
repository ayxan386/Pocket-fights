using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GroundGenerator : MonoBehaviour
{
    [Header("Floor generation")] [SerializeField]
    private Transform floorGenerationPoint;

    [SerializeField] private Transform capLayer;

    [SerializeField] private Vector2Int dimensions;
    [SerializeField] private Vector3 sizeOfCell;
    [SerializeField] private Vector3 offset;

    [SerializeField] private List<BlockChance> blocks;
    [SerializeField] private List<string> floorBlocks;
    [SerializeField] private List<TopBottomPairs> topBottomPairsList;
    [SerializeField] private LayerMask obstructionLayer;
    [SerializeField] private float detectionRadius;

    [Header("Game of life params")] [SerializeField]
    private string joiningCellName = "water";

    [SerializeField] private int cellNumberThreshold;
    [SerializeField] private int deadCellCount = 1;
    [SerializeField] private string deadCellName;

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
                    + IsWater(y + 1, x + 1) + IsWater(y + 1, x - 1) + IsWater(y - 1, x + 1) + IsWater(y - 1, x - 1);
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
        var chance = Mathf.Clamp01(Mathf.PerlinNoise(x + offset.x, y + offset.y));
        foreach (var tuple in blockCollection)
        {
            if (chance >= tuple.weight.x && chance <= tuple.weight.y)
            {
                return tuple;
            }
        }

        print($"Chance {chance}");
        return null;
    }
}