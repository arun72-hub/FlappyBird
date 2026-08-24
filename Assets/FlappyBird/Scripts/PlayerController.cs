using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatingAmplitude = 0.1f;
    [SerializeField] private float floatingSpeed = 4f;

    [Header("Movement")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float rotationSpeed = 20f;

    [Header("Boundaries")]
    [SerializeField] private float maxYPosition = 2.4f;

    private Rigidbody2D rg;
    private Vector3 startPosition;

    private void Awake()
    {
        rg = GetComponent<Rigidbody2D>();
        if (rg != null)
        {
            // Freeze X position and Z rotation in physics to prevent horizontal movement on collisions
            rg.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Start()
    {
        startPosition = new Vector3(-0.5f, 0f, 0f);
        transform.position = startPosition;
        if (rg != null)
        {
            rg.simulated = false;
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.GameState == GameState.Home || GameManager.Instance.GameState == GameState.GetReady)
        {
            FloatIdle();
        }
        else if (GameManager.Instance.GameState == GameState.Playing)
        {
            RotateBird();
            ClampPosition();
        }
    }

    private void FloatIdle()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatingSpeed) * floatingAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void RotateBird()
    {
        if (rg == null) return;

        float angle = Mathf.Clamp(rg.linearVelocity.y * 5f, -90f, 30f);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(0f, 0f, angle),
            rotationSpeed * Time.deltaTime
        );
    }

    private void ClampPosition()
    {
        Vector3 pos = transform.position;
        // Lock X position so bird stays at start X position
        pos.x = startPosition.x;

        // Prevent bird from going above top boundary
        if (pos.y > maxYPosition)
        {
            pos.y = maxYPosition;
            if (rg != null)
            {
                rg.linearVelocity = new Vector2(0f, 0f);
            }
        }
        transform.position = pos;
    }

    public void OnTap(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.GameState == GameState.Home || GameManager.Instance.GameState == GameState.GameOver)
            return;

        if (GameManager.Instance.GameState == GameState.GetReady)
        {
            GameManager.Instance.GamePlay();
            if (rg != null)
            {
                rg.simulated = true;
            }
        }

        if (rg != null)
        {
            rg.linearVelocity = Vector2.zero;
            rg.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Fly();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GameManager.Instance == null || GameManager.Instance.GameState == GameState.GameOver) return;

        if (!collision.collider.CompareTag("Obstacle"))
            return;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Hit();
        }

        StartCoroutine(DieSoundDelay());
        GameManager.Instance.GameOver();
    }

    private IEnumerator DieSoundDelay()
    {
        yield return new WaitForSeconds(0.3f);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.Die();
        }
    }

    public void ResetPlayer()
    {
        startPosition = new Vector3(-0.5f, 0f, 0f);
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;

        if (rg != null)
        {
            rg.simulated = false;
            rg.linearVelocity = Vector2.zero;
            rg.angularVelocity = 0f;
        }
    }
}