using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    private List<IPlayerObserver> observers = new List<IPlayerObserver>();

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

    public Vector3 BallPos { get; set; }

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
            NotifyNeedBallPosition(this);

            lastServingPlayer = this.gameObject.tag;

            NotifyBallShooter(lastServingPlayer);

            GameObject secondPlayer = lastServingPlayer == "Player" ? GameObject.FindGameObjectWithTag("SecondPlayer") : GameObject.FindGameObjectWithTag("Player");
            float direction = lastServingPlayer == "Player" ? 1f : -1f;

            if (secondPlayer != null)
            {
                Vector3 serveDirection = (secondPlayer.transform.position - BallPos).normalized;
                serveDirection.y = 0;

                serveDirection = serveDirection.normalized;

                float upwardForce = 1.5f;
                serveDirection += Vector3.up * upwardForce;

                float forwardForce = 1.1f;

                serveDirection += Vector3.forward * forwardForce * direction;

                float randomLateralOffset = Random.Range(-0.4f, 0.4f);
                float randomHeightOffset = Random.Range(-0.1f, 0.1f);
                float randomForwardOffset = Random.Range(-0.1f, 0.1f);

                serveDirection.x += randomLateralOffset;
                serveDirection.y += randomHeightOffset;
                serveDirection.z += randomForwardOffset;

                float appliedForce = serveForce;

                if (isMidAirServe)
                {
                    serveDirection -= Vector3.up * upwardForce;

                    appliedForce *= powerServeMultiplier;
                    isMidAirServe = false;
                }

                NotifyServe(serveDirection * appliedForce, ForceMode.Impulse);
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
            NotifyBallCollision();

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

    public void AddObserver(IPlayerObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IPlayerObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    private void NotifyNeedBallPosition(Player player)
    {
        foreach (var observer in observers)
        {
            observer.OnBallPosition(player);
        }
    }

    private void NotifyBallShooter(string player)
    {
        foreach (var observer in observers)
        {
            observer.OnBallShooter(player);
        }
    }

    private void NotifyServe(Vector3 direction, ForceMode mode)
    {
        foreach (var observer in observers)
        {
            observer.OnServe(direction, mode);
        }
    }

    private void NotifyBallCollision()
    {
        foreach (var observer in observers)
        {
            observer.OnBallCollision();
        }
    }
}
