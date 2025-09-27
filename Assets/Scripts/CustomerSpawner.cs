using UnityEngine;
using System.Collections;
using System;
public class CustomerSpawner : MonoBehaviour
{
    public static CustomerSpawner Instance { get; private set; }

    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform servePoint;

    private bool hasActiveCustomer = false;
    [SerializeField] PlayerController playerController;
    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!hasActiveCustomer)   // only spawn if none waiting
            {
                SpawnCustomer();
                hasActiveCustomer = true;
            }
            yield return null; // check every frame
        }
    }

    private void SpawnCustomer()
    {
        var customer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        var ai = customer.GetComponent<CustomerAI>();
        ai.serveSpot = servePoint;
        ai.customerSpawner = this;
    }

    public void CustomerLeft()
    {
        hasActiveCustomer = false;
    }
    public IEnumerator MovePlayerToMachine()
    {
        yield return new WaitForSeconds(1);
        playerController.MoveToMachine();
    }
}
