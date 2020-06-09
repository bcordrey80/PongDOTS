using System;
using System.Collections;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private float xBound;
    [SerializeField] private float yBound;
    [SerializeField] private float ballSpeed;
    [SerializeField] private float respawnDelay;
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI[] playerScoreTexts;

    public float XBound => xBound;
    public float YBound => yBound;

    private int[] _playerScores;

    private Entity _ballEntity;
    private EntityManager _manager;
    private BlobAssetStore _blobAssetStore;

    private WaitForSeconds _oneSecond;
    private WaitForSeconds _delay;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        _playerScores = new int[2];

        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _blobAssetStore = new BlobAssetStore();

        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, _blobAssetStore);
        _ballEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(ballPrefab, settings);

        _oneSecond = new WaitForSeconds(1.0f);
        _delay = new WaitForSeconds(respawnDelay);

        StartCoroutine(CountdownAndSpawnBall());
    }

    public void PlayerScored(int playerId)
    {
        _playerScores[playerId]++;
        for (int i = 0; i < _playerScores.Length && i < playerScoreTexts.Length; i++)
        {
            playerScoreTexts[i].text = $"{_playerScores[i]}";
        }

        StartCoroutine(CountdownAndSpawnBall());
    }

    IEnumerator CountdownAndSpawnBall()
    {
        mainText.text = "Get Ready";
        yield return _delay;
        
        mainText.text = "3";
        yield return _oneSecond;

        mainText.text = "2";
        yield return _oneSecond;

        mainText.text = "1";
        yield return _oneSecond;

        mainText.text = "";

        SpawnBall();
        
    }

    void SpawnBall()
    {
        StopCoroutine(CountdownAndSpawnBall());

        Entity ball = _manager.Instantiate(_ballEntity);

        Vector3 direction = new Vector3(UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1, UnityEngine.Random.Range(-.5f, .5f), 0f).normalized;

        Vector3 speed = direction * ballSpeed;

        PhysicsVelocity velocity = new PhysicsVelocity()
        {
            Linear = speed,
            Angular = float3.zero
        };

        _manager.AddComponentData(ball, velocity);
    }

    private void OnDestroy()
    {
        _blobAssetStore.Dispose();
    }
}
