using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 2f;

    private Material material;
    private Vector2 offset;

    private void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            material = sr.material;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState == GameState.GameOver)
            return;

        if (material != null)
        {
            offset.x += scrollSpeed * Time.deltaTime;
            material.mainTextureOffset = offset;
        }
    }
}
