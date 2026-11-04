using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpForce = 12f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 velocity = rb.linearVelocity;
                velocity.y = 0f;
                rb.linearVelocity = velocity;

                rb.AddForce(
                    Vector3.up * jumpForce,
                    ForceMode.VelocityChange
                );

                Debug.Log("Jump Pad Activated!");
            }
        }
    }
}

