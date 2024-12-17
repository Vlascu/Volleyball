using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private Vector3 firstPlayerPos;
    [SerializeField] private Vector3 secondPlayerPos;

    private List<IBallObserver> observers = new List<IBallObserver>();

    private Rigidbody ballRb;
    public string ShootingPlayer
    {
        get; set;
    }
    public Vector3 FirstPlayerPos
    {
        get { return firstPlayerPos; }
    }

    public Vector3 SecondPlayerPos
    {
        get { return secondPlayerPos; }
    }

    void Start()
    {
        ballRb = GetComponent<Rigidbody>();

        transform.position = firstPlayerPos;
    }

    void Update()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 contactPoint = contact.point;

                Collider[] overlappingColliders = Physics.OverlapSphere(contactPoint, 0.1f);

                if (Physics.OverlapSphere(contactPoint, 0.1f).Any(col => col.CompareTag("SideB")))
                {
                    NotifyGroundTouched("SideB");
                }
                else if (Physics.OverlapSphere(contactPoint, 0.1f).Any(col => col.CompareTag("SideA")))
                {
                    NotifyGroundTouched("SideA");
                }
                else
                {
                    if (ShootingPlayer == "Player")
                        NotifyGroundTouched("SideA");
                    else
                        NotifyGroundTouched("SideB");
                }
            }

            NotifySwitchPlayer();
        }
        if (collision.gameObject.CompareTag("Net"))
        {
            if (ShootingPlayer == "Player")
                NotifyGroundTouched("SideA");
            else
                NotifyGroundTouched("SideB");

            NotifySwitchPlayer();
        }

    }
    public void AddObserver(IBallObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IBallObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    private void NotifyGroundTouched(string side)
    {
        foreach (var observer in observers)
        {
            observer.OnSideTouched(side);
        }
    }

    private void NotifySwitchPlayer()
    {
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.useGravity = false;

        foreach (var observer in observers)
        {
            observer.OnSwitchPlayer();
        }
    }

}
