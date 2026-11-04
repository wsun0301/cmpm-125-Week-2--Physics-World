using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    private const float DetectDistance = 7f;
    private const float LoseDistance = 10f;
    private const float RoamSpeedMultiplier = 0.5f;

    private GameManager manager;
    private Rigidbody body;
    private Rigidbody player;
    private float speed;
    private Vector3 waypoint;
    private Vector3 roamTarget;
    private bool chasing;

    public void Initialize(GameManager gameManager, Rigidbody enemyBody, Rigidbody target, float chaseSpeed)
    {
        manager = gameManager;
        body = enemyBody;
        player = target;
        speed = chaseSpeed;
        waypoint = body.position;
        roamTarget = body.position;
    }

    void FixedUpdate()
    {
        if (manager == null || manager.GameEnded || player == null) return;
        float detectionRange = chasing ? LoseDistance : DetectDistance;
        bool playerInRange = (player.position - body.position).sqrMagnitude <= detectionRange * detectionRange;
        if (chasing && !playerInRange) roamTarget = body.position;
        chasing = playerInRange;

        if ((waypoint - body.position).sqrMagnitude < 0.01f)
        {
            if (!chasing && (roamTarget - body.position).sqrMagnitude < 0.04f)
                PickRoamTarget();

            Vector3 destination = chasing ? player.position : roamTarget;
            waypoint = manager.Arena.NextStep(body.position, destination);
            if (!chasing && (waypoint - body.position).sqrMagnitude < 0.01f)
                roamTarget = body.position;
        }

        float movementSpeed = chasing ? speed : speed * RoamSpeedMultiplier;
        Vector3 next = Vector3.MoveTowards(body.position, waypoint, movementSpeed * Time.fixedDeltaTime);
        if (manager.Arena.IsClear(next, 0.65f)) body.MovePosition(next);
        else waypoint = body.position;
    }

    private void PickRoamTarget()
    {
        for (int attempt = 0; attempt < 12; attempt++)
        {
            Vector3 candidate = new Vector3(Random.Range(-12, 13), body.position.y, Random.Range(-12, 13));
            if ((candidate - body.position).sqrMagnitude < 9f || !manager.Arena.IsClear(candidate, 0.7f))
                continue;

            roamTarget = candidate;
            return;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != player) return;
        Vector3 direction = player.position - body.position;
        direction.y = 0f;
        manager.TakeDamage(direction.normalized);
    }
}
