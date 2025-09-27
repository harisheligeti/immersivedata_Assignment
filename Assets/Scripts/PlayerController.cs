using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerState { Idle, MovingToBeans, Collecting, MovingToMachine, Processing, MovingToCustomer, Serving }

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Spots")]
    public Transform beanSpot;
    public Transform machineSpot;
    public Transform customerSpot;

    [Header("Stacking")]
    public Transform[] stackSlots;
    public GameObject beanPrefab;
    public int maxStack = 3;
    private readonly List<GameObject> stackedBags = new();

    [Header("Cup Carry")]
    public Transform cupAnchor;
    public GameObject cupPrefab;
    private GameObject carriedCup;

    [Header("UI")]
    public ProgressBarUI progressBar;

    private PlayerState state = PlayerState.Idle;
    private Transform target;

    private void Start()
    {
        NextState(PlayerState.MovingToBeans);
    }

    private void Update()
    {
        if (target != null && state.ToString().StartsWith("Moving"))
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            target = null;
            OnReachedTarget();
        }
    }

    private void OnReachedTarget()
    {
        if (state == PlayerState.MovingToBeans) StartCoroutine(CollectBeansRoutine());
        if (state == PlayerState.MovingToMachine) StartCoroutine(ProcessBeansRoutine());
        if (state == PlayerState.MovingToCustomer) StartCoroutine(ServeRoutine());
    }

    private void NextState(PlayerState next)
    {
        state = next;
        switch (state)
        {
            case PlayerState.MovingToBeans:
                target = beanSpot;
                break;
            case PlayerState.MovingToMachine:
                target = machineSpot;
                break;
            case PlayerState.MovingToCustomer:
                target = customerSpot;
                break;
        }
    }
    public void MoveToMachine()
    {
        NextState(PlayerState.MovingToMachine);
        progressBar.Show(0.1f);
    }
    private IEnumerator CollectBeansRoutine()
    {
        state = PlayerState.Collecting;
        for (int i = 0; i < maxStack; i++)
        {
            progressBar.Show(1.5f); // 1.5s per bag
            yield return new WaitForSeconds(1.5f);

            var bag = Instantiate(beanPrefab);
            bag.transform.SetParent(stackSlots[stackedBags.Count], false);
            stackedBags.Add(bag);
        }
        NextState(PlayerState.MovingToMachine);
    }

    private IEnumerator ProcessBeansRoutine()
    {
        state = PlayerState.Processing;

        foreach (var bag in stackedBags)
            Destroy(bag);
        stackedBags.Clear();

        progressBar.Show(2.5f); // processing time
        yield return new WaitForSeconds(2.5f);

        carriedCup = Instantiate(cupPrefab, cupAnchor);
        carriedCup.transform.localPosition = Vector3.zero;

        NextState(PlayerState.MovingToCustomer);
        StartCoroutine(CustomerSpawner.Instance.SpawnLoop());
    }

    private IEnumerator ServeRoutine()
    {
        state = PlayerState.Serving;

        if (carriedCup != null)
        {
            Destroy(carriedCup);
            carriedCup = null;
        }
        yield return new WaitForSeconds(0.1f);
        var customer = Object.FindAnyObjectByType<CustomerAI>();
        if (customer) customer.ReceiveCup();
        progressBar.Show(1f);
    }
}
