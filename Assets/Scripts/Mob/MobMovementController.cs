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
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private bool drawGizmos;

    [SerializeField] [Range(0, 1f)] private float sfxDensity = 0.1f;
    [SerializeField] private int sfxRateLimiter;
    [SerializeField] private UiSoundEffect motionSfx;

    [FormerlySerializedAs("currentTargetPoint")] [Header("Debug")] [SerializeField]
    private GridPoint finalTargetPoint;

    public bool Navigate { get; set; }

    private List<GridPoint> grid;
    public List<GridPoint> path;
    public int currentIndex;
    private Vector3 movementVector;
    private RoomManager room;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        StartCoroutine(TargetMovement());
    }

    public void ToggleMovement(bool shouldMove)
    {
        rb.isKinematic = !shouldMove;
        Navigate = shouldMove;
    }

    private IEnumerator TargetMovement()
    {
        yield return new WaitForSeconds(1);
        FindClosestRoom();
        yield return new WaitForSeconds(0.2f);
        while (path.Count == 0)
        {
            SelectRandomTarget();
            yield return new WaitForSeconds(0.2f);
        }

        while (true)
        {
            rb.velocity = Vector3.zero;
            yield return new WaitUntil(() => Navigate && gameObject.activeSelf && currentIndex < path.Count);
            var dir = path[currentIndex].pos - transform.position;
            movementVector = dir.normalized * movementSpeed;
            movementVector.y = 0;

            while (Navigate && Vector3.Angle(transform.forward, movementVector) > 3)
            {
                transform.forward = Vector3.Lerp(transform.forward, movementVector, turnRate * Time.deltaTime);
                yield return null;
            }


            animator.SetBool("move", Navigate);
            var rateCounter = 0;
            while (Navigate && gameObject.activeSelf)
            {
                dir = path[currentIndex].pos - transform.position;
                movementVector = dir.normalized * movementSpeed;
                movementVector.y = 0;
                rb.velocity = movementVector;
                var distance = Vector3.Distance(path[currentIndex].pos, transform.position);

                if(rateCounter == 0)
                    TryAndMakeSound();

                rateCounter = (rateCounter + 1) % sfxRateLimiter;

                if (currentIndex + 1 >= path.Count)
                {
                    currentIndex = 0;
                    SelectRandomTarget();
                    break;
                }

                if (distance < 0.6f)
                {
                    currentIndex++;
                    break;
                }

                yield return null;
            }
        }
    }

    private void TryAndMakeSound()
    {
        if (room.IsActive && Random.value <= sfxDensity)
        {
            motionSfx.PlayWithoutCheck();
        }
    }

    [ContextMenu("Select random target")]
    public void SelectRandomTarget()
    {
        var finalIndex = Random.Range(0, grid.Count);
        while (grid[finalIndex].isObstructed)
        {
            finalIndex = Random.Range(0, grid.Count);
        }

        foreach (var point in grid)
        {
            point.visitIndex = 0;
            point.parentIndex = -1;
        }

        currentIndex = 0;

        finalTargetPoint = grid[finalIndex];
        finalTargetPoint.parentIndex = -1;

        var order = new Queue<int>();
        order.Enqueue(finalIndex);

        while (order.Count > 0)
        {
            var currentIndex = order.Dequeue();
            grid[currentIndex].visitIndex = 2;
            var distToMob = Vector3.Distance(transform.position, grid[currentIndex].pos);
            if (distToMob <= 0.9f)
            {
                //Found player pos
                path = new List<GridPoint>();
                while (currentIndex != -1)
                {
                    path.Add(grid[currentIndex]);
                    currentIndex = grid[currentIndex].parentIndex;
                }

                return;
            }

            var allNeighbours = GetAllNeighbours(currentIndex);
            foreach (var neighbour in allNeighbours)
            {
                if (!neighbour.isObstructed && neighbour.visitIndex == 0)
                {
                    neighbour.visitIndex = 1;
                    neighbour.parentIndex = currentIndex;
                    order.Enqueue(neighbour.index);
                }
            }
        }
    }

    private List<GridPoint> GetAllNeighbours(int currentIndex)
    {
        var y = currentIndex / gridSize.y;
        var x = currentIndex % gridSize.y;
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

    private void FindClosestRoom()
    {
        var minDistance = 1000000f;
        foreach (var room in FloorManager.Instance.Rooms)
        {
            var distance = Vector3.Distance(transform.position, room.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                this.room = room;
                gridSize = room.GridSize;
            }
        }

        grid = room.Grid.ConvertAll(point => new GridPoint(point));
    }


    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        foreach (var hit in grid)
        {
            if (hit == finalTargetPoint)
            {
                Gizmos.color = Color.blue;
            }
            else if (path != null && path.Contains(hit))
            {
                Gizmos.color = Color.green;
            }
            else if (hit.isObstructed)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.yellow;
            }

            Gizmos.DrawSphere(hit.pos, 0.3f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(rayOrigin.position, movementVector);
    }
}

[Serializable]
public class GridPoint
{
    public Vector3 pos;
    public bool isObstructed;
    public int visitIndex;
    public int parentIndex;
    public int index;

    public GridPoint(Vector3 pos, int index)
    {
        this.pos = pos;
        this.index = index;
    }

    public GridPoint(GridPoint input) : this(input.pos, input.index)
    {
        isObstructed = input.isObstructed;
    }
}