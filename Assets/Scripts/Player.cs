using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Camera playerCamera;
    private Rigidbody rb;


    // Movement
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float runningMultiplier = 1.4f;
    public float jumpForce = 5f;
    private bool isGrounded = true;
    private bool running = false;


    // Looking
    private float pitch = 0f; // Up-down rotation

    // Shooting
    public float rayRange = 100f;
    public LineRenderer lineRenderer;
    public float rayDuration = 0.1f;
    public int bullets = 10;
    public int bulletReloadCount = 10;
    public int maxBullets = 30;
    private int score = 0;
    public TMP_Text scoreText;
    public TMP_Text bulletText;

    // Interaction
    public float interactionRange = 2.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;

        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;

        bulletText.text = $"Bullets: {bullets}";
    }

    void Update()
    {
        Move();
        Look();
        Jump();
        Shoot();
        TryInteract();
    }

    void Move()
    {
        float forward = 0;
        float sideways = 0;

        // Get input
        if (Input.GetKey(KeyCode.W)) forward += 1;
        if (Input.GetKey(KeyCode.S)) forward -= 1;
        if (Input.GetKey(KeyCode.A)) sideways -= 1;
        if (Input.GetKey(KeyCode.D)) sideways += 1;

        running = Input.GetKey(KeyCode.LeftShift);

        // Calculate movement direction relative to the player
        Vector3 direction = transform.right * sideways + transform.forward * forward;

        // Apply movement
        if (direction.magnitude >= 0.1f)
        {
            Vector3 move = direction.normalized * moveSpeed * Time.deltaTime;

            if (running)
            {
                move *= runningMultiplier;
            }

            rb.MovePosition(rb.position + move);
        }
    }

    void Look()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate player around the Y-axis
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera around the X-axis
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(0) && bullets > 0)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            // Cast the ray
            bool washit = Physics.Raycast(ray, out hit, rayRange);

            if (washit)
            {
                Debug.Log("Hit: " + hit.collider.name);

                if (hit.collider.tag == "Enemy")
                {
                    Debug.Log("Killing enemy.");
                    Destroy(hit.collider.transform.gameObject);
                    score++;

                    scoreText.text = $"Score: {score}";
                }
            }

            bullets--;
            bulletText.text = $"Bullets: {bullets}";

            StartCoroutine(ShowRay(ray, washit ? hit.point : playerCamera.transform.position + ray.direction * 100));
        }
    }

    private System.Collections.IEnumerator ShowRay(Ray ray, Vector3 hitPoint)
    {
        if (lineRenderer != null)
        {
            // Configure the line renderer
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, ray.origin - Vector3.up / 2 + transform.right / 1.5f);
            lineRenderer.SetPosition(1, hitPoint);

            // Wait for the duration before disabling the line renderer
            yield return new WaitForSeconds(rayDuration);
            lineRenderer.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void TryInteract()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        // Raycast in front of the player
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            // Check if the object hit is a box
            if (hit.collider.CompareTag("Box"))
            {
                bullets += bulletReloadCount;
                if (bullets > maxBullets) bullets = maxBullets;
                Debug.Log("Interacted with the box! Player HP set to 10.");

                bulletText.text = $"Bullets: {bullets}";
            }
        }
        else
        {
            Debug.Log("No box in range to interact with.");
        }
    }
}
