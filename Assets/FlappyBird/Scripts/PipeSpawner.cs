using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float heightOffset = 0.75f;
    [SerializeField] private int poolSize = 10;

    private float timer;
    private Camera mainCamera;
    private Queue<PipeController> pipePool = new Queue<PipeController>();
    private List<PipeController> allPipes = new List<PipeController>();

    private void Awake()
    {
        mainCamera = Camera.main;
        SetSpawnPosition();
        CreatePool();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
            return;

        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    private void SetSpawnPosition()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        float screenRight = mainCamera.orthographicSize * mainCamera.aspect;
        float spawnX = screenRight + 2f;

        // Position spawner at fixed Y = 0 for consistent vertical pipe pairing
        transform.position = new Vector3(spawnX, 0f, 0f);
    }

    private void CreatePool()
    {
        if (pipePrefab == null)
        {
            Debug.LogError("PipePrefab is not assigned in PipeSpawner!");
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject pipeObj = Instantiate(pipePrefab);
            pipeObj.SetActive(false);

            PipeController pipe = pipeObj.GetComponent<PipeController>();
            if (pipe != null)
            {
                pipe.Initialize(this);
                pipePool.Enqueue(pipe);
                allPipes.Add(pipe);
            }
        }
    }

    private void SpawnPipe()
    {
        if (pipePool.Count == 0)
        {
            Debug.LogWarning("No available pipes in pool!");
            return;
        }

        PipeController pipe = pipePool.Dequeue();
        float randomY = Random.Range(-heightOffset, heightOffset);
        pipe.transform.position = transform.position + new Vector3(0, randomY, 0);
        pipe.gameObject.SetActive(true);
    }

    public void ReturnPipeToPool(PipeController pipe)
    {
        if (pipe == null) return;
        pipe.gameObject.SetActive(false);
        pipePool.Enqueue(pipe);
    }

    public void ResetSpawner()
    {
        timer = 0f;
        pipePool.Clear();

        foreach (PipeController pipe in allPipes)
        {
            if (pipe != null)
            {
                pipe.gameObject.SetActive(false);
                pipePool.Enqueue(pipe);
            }
        }
    }
}
