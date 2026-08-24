using UnityEngine;

public class PipeController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float hideXPosition = -5f;

    private PipeSpawner spawner;
    private ScoreZone scoreZone;

    private void Awake()
    {
        scoreZone = GetComponentInChildren<ScoreZone>();
    }

    private void OnEnable()
    {
        if (scoreZone != null)
        {
            scoreZone.ResetScoreZone();
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState != GameState.Playing)
            return;

        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x < hideXPosition)
        {
            if (spawner != null)
            {
                spawner.ReturnPipeToPool(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void Initialize(PipeSpawner pipeSpawner)
    {
        spawner = pipeSpawner;
    }
}
