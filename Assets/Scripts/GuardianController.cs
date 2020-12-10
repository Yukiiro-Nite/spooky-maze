using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardianController : MonoBehaviour
{
    public float lookDistance = 10f;
    public float lookMultiplier = 1.5f;
    public float movementEpsillon = 0.1f;
    public float speed = 1f;
    public float speedMultiplier = 1.5f;
    public int stunLimit = 3;
    public float stunTimeout = 1f;
    enum Mode {
        Wonder,
        Chase,
        Search
    }
    private Mode currentMode = Mode.Wonder;
    private Vector3? lastPlayerPosition = null;
    private Vector3 targetPosition;
    private CapsuleCollider playerCollider;
    private SphereCollider selfCollider;
    private Rigidbody selfBody;
    private SphereCollider torchCollider;
    private MapManager mapManager;
    private Pathfinder pathfinder;
    private List<NavNode> path = new List<NavNode>();
    private int stunCount;
    private bool stunned = false;
    void Start()
    {
        gameObject.SetActive(Settings.HasGuardian);
        selfCollider = gameObject.GetComponent<SphereCollider>();
        selfBody = gameObject.GetComponent<Rigidbody>();
        playerCollider = GameObject.Find("XR Rig").GetComponent<CapsuleCollider>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        torchCollider = GameObject.Find("TorchTip").GetComponent<SphereCollider>();
        pathfinder = GameObject.Find("Navigation").GetComponent<Pathfinder>();
        Spawn();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!stunned) {
            Look();
            Move();
        }
    }

    void Spawn()
    {
        currentMode = Mode.Wonder;
        Vector2 mazePos = new Vector2(
            Random.Range(0, mapManager.Width),
            Random.Range(0, mapManager.Height)
        );
        Vector2 worldPos = mapManager.GetWorldPosition(mazePos);
        mapManager.PlaceObject(gameObject, mazePos, mapManager.CeilingHeight / 2f);
        targetPosition = new Vector3(worldPos.x, mapManager.CeilingHeight / 2f, worldPos.y);
        stunCount = 0;
    }

    void Look()
    {
        Vector3 guardianToPlayer = (Camera.main.transform.position - transform.position).normalized;
        bool guardianHit = Physics.Raycast(transform.position, guardianToPlayer, out RaycastHit guardianHitInfo, lookDistance);

        Vector3 playerToGuardian = (transform.position - Camera.main.transform.position).normalized;
        bool playerHit = Physics.Raycast(Camera.main.transform.position, playerToGuardian, out RaycastHit playerHitInfo, lookDistance);

        bool hasHit = guardianHit
            && playerHit
            && guardianHitInfo.collider == playerCollider
            && playerHitInfo.collider == selfCollider;

        if (hasHit) {
            if(path.Count > 0) {
                path.Clear();
            }
            currentMode = Mode.Chase;
            lastPlayerPosition = Camera.main.transform.position;
        } else if (lastPlayerPosition.HasValue && currentMode == Mode.Chase) {
            currentMode = Mode.Search;
        }
    }

    void Move()
    {
        switch(currentMode) {
            case Mode.Wonder:
                DoWonder();
                break;
            case Mode.Chase:
                DoChase();
                break;
            case Mode.Search:
                DoSearch();
                break;
        }
    }

    void DoWonder()
    {
        float distance = Vector3.Distance(transform.position, targetPosition);
        if(distance > movementEpsillon) {
            Vector3 dir = (targetPosition - transform.position).normalized;
            transform.position += dir * Time.fixedDeltaTime * speed;
        } else {
            targetPosition = NextWonderPosition();
        }
    }

    void DoChase()
    {
        Vector3 nextPos = lastPlayerPosition.Value;
        float distance = Vector3.Distance(transform.position, nextPos);
        if(distance > movementEpsillon) {
            Vector3 dir = (nextPos - transform.position).normalized;
            transform.position += dir * Time.fixedDeltaTime * speed;
        } else {
            Debug.Log("Switching from chase to Wonder");
            targetPosition = transform.position;
            currentMode = Mode.Wonder;
        }
    }

    void DoSearch()
    {
        Vector3 nextPos = lastPlayerPosition.Value;
        float distance = Vector3.Distance(transform.position, nextPos);
        if(distance > movementEpsillon) {
            Vector3 dir = (nextPos - transform.position).normalized;
            transform.position += dir * Time.fixedDeltaTime * speed;
        } else {
            Debug.Log("Switching from search to Wonder");
            targetPosition = transform.position;
            currentMode = Mode.Wonder;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == playerCollider) {
            ResetPlayer();
        } else if (collision.collider == torchCollider) {
            TorchHit();
        }
    }

    void ResetPlayer()
    {
        if(Settings.IsHardcore) {
            SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
        } else {
            mapManager.PlaceObject(
                mapManager.Player,
                mapManager.maze.start,
                0f
            );
        }
    }

    Vector3 NextWonderPosition()
    {
        if(path.Count == 0) {
            Vector2 currentPos = mapManager.GetGridPosition(gameObject);
            Cell currentCell = mapManager.maze.getCell(currentPos);
            List<Cell> neighbors = mapManager.maze.getConnectedNeighbors(currentCell);
            Utils.Shuffle(neighbors);
            Cell randomNeighbor = neighbors[Random.Range(0, neighbors.Count)];
            Vector2 newPos = mapManager.GetWorldPosition(new Vector2(randomNeighbor.x, randomNeighbor.y));
            Vector3 targetPos = new Vector3(newPos.x, 0, newPos.y);

            path = pathfinder.FindPath(gameObject.transform.position, targetPos);
        }

        NavNode nextNode = path[0];
        path.Remove(nextNode);

        return nextNode.worldPos + Vector3.up * mapManager.CeilingHeight / 2.0f;
    }

    void TorchHit()
    {
        if(!stunned) {
            stunCount++;
            if(stunCount < stunLimit) {
                StartCoroutine(Stun());
            } else {
                speed *= speedMultiplier;
                lookDistance *= lookMultiplier;
                Spawn();
            }
        }
    }

    private IEnumerator Stun()
    {
        stunned = true;
        selfBody.useGravity = true;

        yield return new WaitForSeconds(stunTimeout);

        stunned = false;
        selfBody.useGravity = false;
    }
}
