using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class MobMovementController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Animator animator;
    [SerializeField] private float turnRate;
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private float detectionRadius;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private LayerMask obstructionLayer;
    [SerializeField] private bool drawGizmos;

    [FormerlySerializedAs("currentTargetPoint")] [Header("Debug")] [SerializeField]
    private GridPoint finalTargetPoint;

    [Header("Obstruction avoidance")] [SerializeField]
    private float obstructionAvoidanceDistance;

    [SerializeField] private float obstructionAvoidanceFactor;
    [SerializeField] private float obstructionAvoidanceDistancePower;

    private List<GridPoint> path;

    private Rigidbody rb;

    public bool Navigate { get; set; }

    public bool Move { get; set; }

    public List<GridPoint> grid;
    private Vector3 sumOfForces;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(TargetMovement());
    }

    private IEnumerator TargetMovement()
    {
        SelectRandomTarget();
        while (true)
        {
            rb.velocity = Vector3.zero;
            yield return new WaitUntil(() => Navigate && Move && finalTargetPoint != null);
            // var dir = finalTargetPoint. - transform.position;
            // dir.Normalize();
            // sumOfForces = dir * movementSpeed;
            // sumOfForces.y = 0;
            //
            // animator.SetBool("move", false);
            // while (Vector3.Angle(transform.forward, sumOfForces) > 10)
            // {
            //     rb.velocity = Vector3.zero;
            //     transform.forward = Vector3.Lerp(transform.forward, sumOfForces, turnRate * Time.deltaTime);
            //     yield return null;
            // }
            //
            // rb.AddForce(sumOfForces, ForceMode.Acceleration);
            // animator.SetBool("move", true);

            if (Vector3.Distance(finalTargetPoint.pos, transform.position) < 0.1f) SelectRandomTarget();
            yield return null;
        }
    }

    private void SelectRandomTarget()
    {
        FindAllHits();
        var finalIndex = Random.Range(0, grid.Count);
        var maxDist = -1f;
        for (var index = 0; index < grid.Count; index++)
        {
            var point = grid[index];
            if(point.isObstructed) continue;
            var distance = Vector3.Distance(point.pos, transform.position);
            if (distance > maxDist)
            {
                finalIndex = index;
                maxDist = distance;
            }
        }

        finalTargetPoint = grid[finalIndex];
        finalTargetPoint.parentIndex = -1;
        var order = new Queue<int>();
        order.Enqueue(finalIndex);
        while (order.Count > 0)
        {
            var currentIndex = order.Dequeue();
            print($"Checking {currentIndex}");
            grid[currentIndex].isVisited = true;
            if (Vector3.Distance(transform.position, grid[currentIndex].pos) < 0.1f)
            {
                //Found player pos
                path = new List<GridPoint>();
                while (currentIndex != -1)
                {
                    path.Add(grid[currentIndex]);
                    currentIndex = grid[currentIndex].parentIndex;
                }
        
                break;
            }
        
            var allNeighbours = GetAllNeighbours(currentIndex);
            print($"Found {allNeighbours.Count} nei");
            foreach (var neighbour in allNeighbours)
            {
                if (!neighbour.isObstructed && !neighbour.isVisited)
                {
                    neighbour.parentIndex = currentIndex;
                    order.Enqueue(neighbour.index);
                }
            }
        
        }
    }

    private List<GridPoint> GetAllNeighbours(int currentIndex)
    {
        var x = currentIndex % gridSize.y;
        var y = currentIndex / gridSize.y;
        print($"Index to corr: {currentIndex} {x} , {y}");
        var res = new List<GridPoint>();
        if (x + 1 < gridSize.x)
        {
            res.Add(grid[currentIndex + 1]);
        }

        if (x - 1 >= 0)
        {
            res.Add(grid[currentIndex - 1]);
        }

        if (y + 1 < gridSize.y)
        {
            res.Add(grid[currentIndex + gridSize.y]);
        }

        if (y - 1 >= 0)
        {
            res.Add(grid[currentIndex - gridSize.y]);
        }

        return res;
    }


    private void FindAllHits()
    {
        grid = new List<GridPoint>();
        for (var x = -gridSize.x / 2; x < gridSize.x / 2; x += 1)
        {
            for (var z = -gridSize.y / 2; z < gridSize.y / 2; z += 1)
            {
                var pos = rayOrigin.position;
                pos.x = Mathf.Floor(pos.x + x);
                pos.z += Mathf.Floor(pos.z + z);

                var gridPoint = new GridPoint(pos, x + gridSize.x / 2 + gridSize.y * (z + gridSize.y / 2));
                grid.Add(gridPoint);
                gridPoint.isObstructed = Physics.CheckSphere(pos, detectionRadius, obstructionLayer);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        foreach (var hit in grid)
        {
            if (hit.parentIndex == -1)
            {
                Gizmos.color = Color.blue;
            } else if (hit.isVisited)
            {
                Gizmos.color = Color.green;
            } else if (hit.isObstructed)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }
            
            Gizmos.DrawSphere(hit.pos, detectionRadius);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rayOrigin.position, sumOfForces);
    }
}

[Serializable]
public class GridPoint
{
    public Vector3 pos;
    public bool isObstructed;
    public bool isVisited;
    public int parentIndex;
    public int index;

    public GridPoint(Vector3 pos, int index)
    {
        this.pos = pos;
        this.index = index;
    }
}