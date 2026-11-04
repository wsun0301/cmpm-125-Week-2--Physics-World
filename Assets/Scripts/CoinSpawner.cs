using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;
    public float spawnInterval = 3f;
    public int maxCoins = 8;
    public float spawnRange = 13f;

    private List<GameObject> coinPool = new List<GameObject>();
    private GameManager manager;

    void Start()
    {
        manager = FindFirstObjectByType<GameManager>();

        // Create coins once and store them in the pool
        for (int i = 0; i < maxCoins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform);
            coin.SetActive(false);
            coinPool.Add(coin);
        }

        StartCoroutine(SpawnRoutine());
    }

    
    int GetRandomRarity()
    {
        int roll = Random.Range(0, 100);

        if (roll < 70)
        {
            return 0; // Common: 70%
        }
        else if (roll < 95)
        {
            return 1; // Rare: 25%
        }
        else
        {
            return 2; // Legendary: 5%
        }
    }


    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (manager != null && manager.HasWon)
            {
                yield break;
            }

            SpawnCoin();
        }
    }

    void SpawnCoin()
    {
        foreach (GameObject coin in coinPool)
        {
            if (!coin.activeSelf)
            {
                // Generate a random position
                Vector3 spawnPosition = new Vector3(
                    Random.Range(-spawnRange, spawnRange),
                    1f,
                    Random.Range(-spawnRange, spawnRange)
                );
                coin.transform.position = spawnPosition;

                Coin coinScript = coin.GetComponent<Coin>();

                if (coinScript != null)
                {
                    coinScript.SetRarity(GetRandomRarity());
                }

                coin.SetActive(true);

                return;
            }
        }
    }

    
    public void StopSpawning()
    {
        StopAllCoroutines();

        foreach (GameObject coin in coinPool)
        {
            if (coin != null)
            {
                coin.SetActive(false);
            }
        }
    }

}
