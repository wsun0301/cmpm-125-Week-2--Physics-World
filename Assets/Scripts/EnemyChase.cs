using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    private const float DetectionRadius = 14f;
    private const float ChaseSpeed = 4.5f;

    private GameManager manager;
    private Rigidbody body;
    private Rigidbody player;

    public void Initialize(GameManager gameManager, Rigidbody enemyBody, Rigidbody target)
    {
        manager = gameManager;
        body = enemyBody;
        player = target;
    }

    void FixedUpdate()
    {
        Vector3 offset = player.position - body.position;
        if (offset.sqrMagnitude > DetectionRadius * DetectionRadius) return;

        offset.y = 0f;
        body.MovePosition(body.position + Vector3.ClampMagnitude(offset, ChaseSpeed * Time.fixedDeltaTime));
    }

    void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody != player) return;

        Vector3 direction = player.position - body.position;
        direction.y = 0f;
        manager.TakeDamage(direction.normalized);
    }
}
