using UnityEngine;

public class Coin : MonoBehaviour
{
    public float rotationSpeed = 120f;
    public int pointValue = 1;

    private GameManager manager;
    private Renderer coinRenderer;

    void Awake()
    {
        manager = FindFirstObjectByType<GameManager>();
        coinRenderer = GetComponent<Renderer>();
    }

    public void SetRarity(int rarity)
    {
        if (rarity == 0)
        {
            pointValue = 1;
            SetColor(Color.yellow);
        }
        else if (rarity == 1)
        {
            pointValue = 3;
            SetColor(Color.cyan);
        }
        else
        {
            pointValue = 10;
            SetColor(Color.magenta);
        }
    }

    void SetColor(Color color)
    {
        if (coinRenderer != null)
        {
            coinRenderer.material.color = color;
        }
    }

    void Update()
    {
        transform.Rotate(
            0f,
            rotationSpeed * Time.deltaTime,
            0f,
            Space.World
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (manager != null && !manager.HasWon)
            {
                manager.AddScore(pointValue);
                gameObject.SetActive(false);
            }
        }
    }
}
