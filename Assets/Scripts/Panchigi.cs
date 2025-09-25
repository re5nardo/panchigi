using System.Collections.Generic;
using UnityEngine;

public class Panchigi : MonoBehaviour
{
    [SerializeField] private OutOfBoundsDetector outOfBoundsDetector;
    [SerializeField] private ForceElement forceElement;
    [SerializeField] private GameObject coin_1_Source;
    [SerializeField] private GameObject coin_2_Source;
    [SerializeField] private GameObject coin_3_Source;
    [SerializeField] private GameObject coin_4_Source;

    private CoinFormation coinFormation = CoinFormation.FourInLine;

    private List<GameObject> coins = new List<GameObject>();

    private bool reservedOutOfBounds = false;

    private void Awake()
    {
        outOfBoundsDetector.onOutOfBoundsDetected += HandleOutOfBounds;
    }

    private void OnDestroy()
    {
        outOfBoundsDetector.onOutOfBoundsDetected -= HandleOutOfBounds;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;

        Physics.gravity = new Vector3(0, -9.81f * 10, 0);
        Physics.defaultSolverIterations = 12;
        Physics.defaultSolverVelocityIterations = 6;

        SpawnCoins(coinFormation);
    }

    private void LateUpdate()
    {
        if (reservedOutOfBounds)
        {
            reservedOutOfBounds = false;

            ResetGame();
            SpawnCoins(coinFormation);
        }
    }

    private void HandleOutOfBounds(Collider collider)
    {
        TextUI.ShowText("Out Of Bounds!", Camera.main.WorldToScreenPoint(new Vector3(9f, 0f, 18f)), 2f);

        reservedOutOfBounds = true;
    }

    public void ResetGame()
    {
        foreach (var coin in coins)
        {
            Destroy(coin);
        }
        coins.Clear();
        forceElement.ResetForceElement();
    }

    public void SpawnCoins(CoinFormation coinFormation)
    {
        switch (coinFormation)
        {
            case CoinFormation.FourInLine:
                SpawnCoinsInFourInLine();
                break;
            case CoinFormation.SixInLine:
                SpawnCoinsInSixInLine();
                break;
            case CoinFormation.FourByTwo:
                SpawnCoinsInFourByTwo();
                break;
            case CoinFormation.SixByTwo:
                SpawnCoinInSixByTwo();
                break;
        }

        this.coinFormation = coinFormation;
    }

    private GameObject CreateCoin(GameObject source, Vector3 position)
    {
        GameObject coin = Instantiate(source, position, Quaternion.identity);

        Rigidbody rigidbody = coin.GetComponentInChildren<Rigidbody>();
        rigidbody.mass = 0.08f;
        rigidbody.linearDamping = 0.0f;
        rigidbody.angularDamping = 1f;
        rigidbody.maxAngularVelocity = 15f;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        coins.Add(coin);

        return coin;
    }

    private void SpawnCoinsInFourInLine()
    {
        CreateCoin(coin_1_Source, new Vector3(6f, 5f, 19f));
        CreateCoin(coin_2_Source, new Vector3(6f, 5f, 15f));
        CreateCoin(coin_3_Source, new Vector3(6f, 5f, 11f));
        CreateCoin(coin_4_Source, new Vector3(6f, 5f, 7f));
    }

    private void SpawnCoinsInSixInLine()
    {
        CreateCoin(coin_1_Source, new Vector3(6f, 5f, 20f));
        CreateCoin(coin_2_Source, new Vector3(6f, 5f, 17f));
        CreateCoin(coin_3_Source, new Vector3(6f, 5f, 14f));
        CreateCoin(coin_4_Source, new Vector3(6f, 5f, 11f));
        CreateCoin(coin_1_Source, new Vector3(6f, 5f, 8f));
        CreateCoin(coin_2_Source, new Vector3(6f, 5f, 5f));
    }

    private void SpawnCoinsInFourByTwo()
    {
        CreateCoin(coin_1_Source, new Vector3(6f, 5f, 19f));
        CreateCoin(coin_2_Source, new Vector3(6f, 5f, 15f));
        CreateCoin(coin_3_Source, new Vector3(6f, 5f, 11f));
        CreateCoin(coin_4_Source, new Vector3(6f, 5f, 7f));

        CreateCoin(coin_1_Source, new Vector3(9f, 5f, 19f));
        CreateCoin(coin_2_Source, new Vector3(9f, 5f, 15f));
        CreateCoin(coin_3_Source, new Vector3(9f, 5f, 11f));
        CreateCoin(coin_4_Source, new Vector3(9f, 5f, 7f));
    }

    private void SpawnCoinInSixByTwo()
    {
        CreateCoin(coin_1_Source, new Vector3(6f, 5f, 20f));
        CreateCoin(coin_2_Source, new Vector3(6f, 5f, 17f));
        CreateCoin(coin_3_Source, new Vector3(6f, 5f, 14f));
        CreateCoin(coin_4_Source, new Vector3(6f, 5f, 11f));
        CreateCoin(coin_1_Source, new Vector3(6f, 5f, 8f));
        CreateCoin(coin_2_Source, new Vector3(6f, 5f, 5f));

        CreateCoin(coin_1_Source, new Vector3(9f, 5f, 20f));
        CreateCoin(coin_2_Source, new Vector3(9f, 5f, 17f));
        CreateCoin(coin_3_Source, new Vector3(9f, 5f, 14f));
        CreateCoin(coin_4_Source, new Vector3(9f, 5f, 11f));
        CreateCoin(coin_1_Source, new Vector3(9f, 5f, 8f));
        CreateCoin(coin_2_Source, new Vector3(9f, 5f, 5f));
    }
}
