using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianController : MonoBehaviour
{
    public float lookDistance = 10f;
    public float movementEpsillon = 0.01f;
    public float speed = 1f;
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
    private MapManager mapManager;
    void Start()
    {
        selfCollider = gameObject.GetComponent<SphereCollider>();
        playerCollider = GameObject.Find("XR Rig").GetComponent<CapsuleCollider>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        Spawn();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Look();
        Move();
    }

    void Spawn()
    {
        currentMode = Mode.Wonder;
        Vector2 mazePos = new Vector2(
            Random.Range(0, mapManager.Width),
            Random.Range(0, mapManager.Height)
        );
        mapManager.PlaceObject(gameObject, mazePos, mapManager.CeilingHeight / 2f);
        targetPosition = transform.position;
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
            currentMode = Mode.Chase;
            lastPlayerPosition = Camera.main.transform.position;
        } else if (lastPlayerPosition.HasValue) {
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

    }

    void DoChase()
    {
        Vector3 nextPos = lastPlayerPosition.Value;
        float distance = Vector3.Distance(transform.position, nextPos);
        if(distance > movementEpsillon) {
            Vector3 dir = (nextPos - transform.position).normalized;
            transform.position += dir * Time.fixedDeltaTime * speed;
        }
    }

    void DoSearch()
    {
        Vector3 nextPos = lastPlayerPosition.Value;
        float distance = Vector3.Distance(transform.position, nextPos);
        if(distance > movementEpsillon) {
            Vector3 dir = (nextPos - transform.position).normalized;
            transform.position += dir * Time.fixedDeltaTime * speed;
        } else if(distance <= movementEpsillon) {
            targetPosition = transform.position;
            currentMode = Mode.Wonder;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.collider == playerCollider) {
            mapManager.PlaceObject(
                mapManager.Player,
                mapManager.maze.start,
                0f
            );
        }
    }
}
