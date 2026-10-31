using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSmoothTime;

    [Header("Game Manager")]
    [SerializeField] private GameManager gameManager;

    [Header("Gravity & Jump Settings")]
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight;
    [SerializeField] private LayerMask jumpableLayer;
    [SerializeField] private Transform playerFootTransform;
    [SerializeField] private float groundCheckDistance;

    [Header("Audio")]
    [SerializeField] private AudioClip coinSFX;
    [SerializeField] private AudioClip injurySFX;
    [SerializeField] private AudioClip gameOverSFX;

    [Header("Keybinds")]
    public InputAction moveAction;
    public InputAction runAction;
    public InputAction jumpAction;

    [Header("Chopper Damage")]
    [SerializeField] private int chopperDamage = 20;
    [SerializeField] private float chopperDamageCooldown = 1f;

    private float nextChopperDamageTime = 0f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool gameEnded = false;

    private AudioSource audioSource;
    private CharacterController controller;
    private Animator animator;
    private Camera mainCam;

    public int score;
    public int health;

    public TextMeshProUGUI scoreTextObj;
    public TextMeshProUGUI healthTextObj;
    public TextMeshProUGUI GameOverTextObj;
    public TextMeshProUGUI coinsTextObj;
    public TextMeshProUGUI winCoinsTextObj;

    private void OnEnable()
    {
        moveAction.Enable();
        runAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        runAction.Disable();
        jumpAction.Disable();
    }

    private void Start()
    {
        score = 0;
        health = 100;

        if (scoreTextObj != null)
            scoreTextObj.text = ": " + score;

        if (healthTextObj != null)
            healthTextObj.text = ": " + health;

        if (GameOverTextObj != null)
            GameOverTextObj.text = "";

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (gameEnded)
            return;

        isGrounded = Physics.CheckSphere(
            playerFootTransform.position,
            groundCheckDistance,
            jumpableLayer
        );

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 direction = new Vector3(
            input.x, 0f, input.y
        ).normalized;

        bool isRunning = runAction.IsPressed();

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle =
                Mathf.Atan2(direction.x, direction.z)
                * Mathf.Rad2Deg
                + mainCam.transform.eulerAngles.y;

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationSpeed,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 newDir =
                Quaternion.Euler(0f, targetAngle, 0f)
                * Vector3.forward;

            controller.Move(
                newDir.normalized * currentSpeed * Time.deltaTime
            );
        }

        float animationSpeed = direction.magnitude * currentSpeed;

        if (animator != null)
        {
            animator.SetFloat(
                "Speed",
                animationSpeed,
                0.1f,
                Time.deltaTime
            );
        }

        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);

            if (animator != null)
                animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(
        ControllerColliderHit collision
    )
    {
        if (gameEnded)
            return;

        // Chopper blade damage
        if (collision.gameObject.CompareTag("Chopper"))
        {
            if (Time.time < nextChopperDamageTime)
                return;

            nextChopperDamageTime =
                Time.time + chopperDamageCooldown;

            TakeDamage(chopperDamage);
        }
        // Enemy damage
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(20);

            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameEnded)
            return;

        // Collect coins
        if (other.CompareTag("Coins"))
        {
            score++;

            if (scoreTextObj != null)
                scoreTextObj.text = ": " + score;

            if (audioSource != null && coinSFX != null)
                audioSource.PlayOneShot(coinSFX);

            Destroy(other.gameObject);
        }

        // Zombie collision
        else if (other.CompareTag("Zombie"))
        {
            TakeDamage(100);
        }

        // Win trigger
        else if (other.CompareTag("Win"))
        {
            TriggerWin();
        }
        
        // Collect Life (Heart)
        else if (other.CompareTag("Life"))
        {
            if (health < 100)
            {
                health = Mathf.Min(health + 20, 100);
                healthTextObj.text = ": " + health;

                other.gameObject.SetActive(false);
                StartCoroutine(RestoreHeart(other.gameObject));
            }
        }
    }
    
    private System.Collections.IEnumerator RestoreHeart(GameObject heart)
    {
        yield return new WaitForSeconds(10f);

        if (heart != null)
            heart.SetActive(true);
    }


    private void TakeDamage(int damage)
    {
        if (gameEnded)
            return;

        health -= damage;
        health = Mathf.Max(health, 0);

        if (healthTextObj != null)
            healthTextObj.text = ": " + health;

        if (health > 0)
        {
            if (audioSource != null && injurySFX != null)
                audioSource.PlayOneShot(injurySFX);
        }
        else
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        if (gameEnded)
            return;

        // Prevent further gameplay.
        gameEnded = true;

        // Update Game Over UI.
        if (GameOverTextObj != null)
            GameOverTextObj.text = "GAME OVER";

        if (coinsTextObj != null)
            coinsTextObj.text = "Coins Collected: " + score;

        // Play sound before showing the menu.
        // This AudioSource must remain active.
        if (audioSource != null && gameOverSFX != null)
        {
            audioSource.PlayOneShot(gameOverSFX);
            Debug.Log("Game Over SFX triggered.");
        }
        else
        {
            Debug.LogWarning(
                "Game Over SFX or AudioSource is missing!"
            );
        }

        // Stop player movement without destroying the player.
        if (controller != null)
            controller.enabled = false;

        // Show Game Over menu.
        if (gameManager != null)
        {
            gameManager.ShowGameOverMenu();
        }
        else
        {
            Debug.LogError("GameManager is not assigned!");
        }
    }

    private void TriggerWin()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        // Add win bonus.
        score += 20;

        if (scoreTextObj != null)
            scoreTextObj.text = ": " + score;

        if (winCoinsTextObj != null)
            winCoinsTextObj.text = "Coins Collected: " + score;

        if (audioSource != null && coinSFX != null)
            audioSource.PlayOneShot(coinSFX);

        if (controller != null)
            controller.enabled = false;

        if (gameManager != null)
        {
            gameManager.ShowWinMenu();
        }
        else
        {
            Debug.LogError("GameManager is not assigned!");
        }
    }
}
