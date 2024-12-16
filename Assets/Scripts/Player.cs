using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float serveForce;
    [SerializeField] private float powerServeMultiplier;

    private Rigidbody playerRb;
    private bool isJumping = false;
    private bool isMidAirServe = false;
    private bool ballContact = false;

    private string lastServingPlayer;

    [SerializeField] private KeyCode upKey;
    [SerializeField] private KeyCode downKey;
    [SerializeField] private KeyCode leftKey;
    [SerializeField] private KeyCode rightKey;
    [SerializeField] private KeyCode jumpKey;
    [SerializeField] private KeyCode serveKey;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleMovement();
        HandleServe();
    }

    private void HandleMovement()
    {
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(leftKey)) horizontal = -1;
        if (Input.GetKey(rightKey)) horizontal = 1;
        if (Input.GetKey(upKey)) vertical = 1;
        if (Input.GetKey(downKey)) vertical = -1;

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
        playerRb.MovePosition(playerRb.position + moveDirection * moveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            isMidAirServe = true;
        }
    }

    private void HandleServe()
    {
        if (Input.GetKeyDown(serveKey) && ballContact)
        {
            lastServingPlayer = this.gameObject.tag;

            GameObject ball = GameObject.FindGameObjectWithTag("Ball");
            Rigidbody ballRb = ball.GetComponent<Rigidbody>();

            GameObject secondPlayer = lastServingPlayer == "Player" ? GameObject.FindGameObjectWithTag("SecondPlayer") : GameObject.FindGameObjectWithTag("Player");
            float direction = lastServingPlayer == "Player" ? 1f : -1f;

            //TODO: random forces and direction

            if (secondPlayer != null)
            {
                Vector3 serveDirection = (secondPlayer.transform.position - ball.transform.position).normalized;
                serveDirection.y = 0;

                serveDirection = serveDirection.normalized;

                float upwardForce = 1.5f;
                serveDirection += Vector3.up * upwardForce;

                float forwardForce = 1.1f;

                serveDirection += Vector3.forward * forwardForce * direction;

                float appliedForce = serveForce;

                if (isMidAirServe)
                {
                    serveDirection -= Vector3.up * upwardForce;

                    appliedForce *= powerServeMultiplier;
                    isMidAirServe = false;
                }

                ballRb.AddForce(serveDirection * appliedForce, ForceMode.Impulse);

            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
        if(collision.gameObject.CompareTag("Ball"))
        {
            ballContact = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            ballContact = false;
        }
    }
}
