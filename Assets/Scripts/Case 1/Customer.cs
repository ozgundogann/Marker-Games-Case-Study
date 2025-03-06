using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class Customer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private CustomerState currentState;
    
    public float TimeToReachDestination;
    
    public event Action<Customer> OnReachingLine;
    public event Action<Customer> OnInteractionEnded;

    public CustomerState CurrentState => currentState;
    public Vector3 Destination;
    private void OnEnable()
    {
        CustomerController.Instance.RegisterInitializedCustomer(this);
        agent.isStopped = true;
        agent.speed = Random.Range(2f, 10f);
    }

    private void OnDisable()
    {
        CustomerController.Instance.RemoveFromInitializedList(this);
    }
    
    private void Update()
    {
        if(agent.isStopped) return;
        if (!agent.pathPending && agent.remainingDistance <= 0.3f)
        {
            agent.isStopped = true;
            OnReachingLine?.Invoke(this);
        }
    }
    
    public void SetDestination(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    public void SetIsStopped(bool isStopped)
    {
        agent.isStopped = isStopped;
    }

    public void SetState(CustomerState newCustomerState)
    {
        currentState = newCustomerState;
    }
    
    /*public void CalculateTimeToReachDestination(Vector3 destination)
    {
        float totalPathLength = 0f;
        totalPathLength += Vector3.Distance(destination, transform.position);
        TimeToReachDestination = totalPathLength / agent.speed;
    }*/
    
    public void CalculateTimeToReachDestination(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(destination, path)) // Geçerli bir yol var mı?
        {
            float totalPathLength = GetPathLength(path); // Gerçek yol uzunluğunu al
            TimeToReachDestination = totalPathLength / agent.speed; // Süreyi hesapla
        }
        else
        {
            TimeToReachDestination = Vector3.Distance(transform.position, destination); // Ulaşamazsa sonsuz yap
        }
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0f;

        // Path'in her noktasını gezerek uzunluğu hesapla
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return length;
    }

    public void StartInteraction()
    {
        SetState(CustomerState.INTERACTING);
        StartCoroutine(WaitToInteract());
    }

    IEnumerator WaitToInteract()
    {
        yield return new WaitForSeconds(5);
        OnInteractionEnded?.Invoke(this);
    }
}