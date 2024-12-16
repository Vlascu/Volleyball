using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector3 initialPosition;

    private Rigidbody ballRb;

    void Start()
    {
        ballRb = GetComponent<Rigidbody>();
        initialPosition = transform.position;

        ballRb.useGravity = true;
        ballRb.mass = 0.5f;
    }

    void Update()
    {
    }

    private void ResetPosition()
    {
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            ResetPosition();
        }
        if (collision.gameObject.CompareTag("Net"))
        {
            ResetPosition();
        }
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("SecondPlayer"))
        {
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
    }
}
