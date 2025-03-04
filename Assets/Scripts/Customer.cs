using System;
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
        CustomerController.Instance.RemoveCustomer(this);
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
    
    public void CalculateTimeToReachDestination(Vector3 destination)
    {
        /*float totalPathLength = 0f;
        if(agent.path.corners.Length > 1)
        {
            for (int i = 0; i < agent.path.corners.Length - 1; i++)
            {
                totalPathLength += Vector3.Distance(agent.path.corners[i], agent.path.corners[i + 1]);
            }
        }
        else if(agent.path.corners.Length == 1)
        {
            totalPathLength = agent.remainingDistance;
        }
        
        TimeToReachDestination = totalPathLength / agent.speed;*/
        
        TimeToReachDestination = Vector3.Distance(transform.position, destination) / agent.speed;
    }

    private void Update()
    {
        if(agent.isStopped) return;
        if (!agent.pathPending && agent.remainingDistance <= 0.01f)
        {
            agent.isStopped = true;
            OnReachingLine?.Invoke(this);
        }
    }
}