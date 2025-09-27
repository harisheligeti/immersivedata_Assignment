using UnityEngine;

public class CustomerAI : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public Transform serveSpot;

    private bool reachedServeSpot = false;
    private bool isServed = false;
    public CustomerSpawner customerSpawner;
    private void Update()
    {
        if (!reachedServeSpot && serveSpot != null)
        {
            MoveToServeSpot();
        }
    }
   
    private void MoveToServeSpot()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            serveSpot.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, serveSpot.position) < 0.05f)
        {
            reachedServeSpot = true;
            StartCoroutine(customerSpawner.MovePlayerToMachine());
        }
    }

    public void ReceiveCup()
    {
        if (isServed) return;

        isServed = true;

        // Add reward
        GameManager.Instance.OnCustomerServed(5, transform.position);

        CustomerSpawner.Instance.CustomerLeft();
        Invoke(nameof(DisableMe),1f);
    }

    void DisableMe()
    {
        Destroy(gameObject);
    }
}
