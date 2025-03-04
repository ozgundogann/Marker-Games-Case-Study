using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;


public class CustomerController : MonoBehaviour
{
    [SerializeField] private CustomerInteractionHandler interactionHandler;
    [SerializeField] private LineOrganizer lineOrganizer;
    
    
    [SerializeField] private List<Customer> initializedCustomers;
    [SerializeField] private List<Customer> walkingCustomerList;
    [SerializeField] private List<Customer> waitingInLineList;
    [SerializeField] private Transform ExitPointTransform;
    
    public static CustomerController Instance { get; private set; }

    private void Awake() 
    { 
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        
        InputReader.OnSpacePressed += HandleOnSpacePressed;
    }

    private void OnDestroy()
    {
        InputReader.OnSpacePressed -= HandleOnSpacePressed;
    }

    private void HandleOnSpacePressed()
    {
        Customer customer = GetRandomCustomer();
        if (!customer) return;
        walkingCustomerList.Add(customer);
        initializedCustomers.Remove(customer);
        customer.OnReachingLine += HandleReachingLine;

        
        UpdateWalkingCustomersDestination();
    }

    private void UpdateWalkingCustomersDestination()
    {
        Queue<Vector3> availablePoints = new Queue<Vector3>(GetAvailableLinePoints());
        List<Customer> remainingCustomers = new List<Customer>(walkingCustomerList);
        
        
        while (availablePoints.Count > 0 && remainingCustomers.Count > 0)
        {
            Vector3 destinationPoint = availablePoints.Dequeue();
            
            foreach (Customer customer in remainingCustomers)
            {
                customer.CalculateTimeToReachDestination(destinationPoint);
            }
            remainingCustomers = remainingCustomers.OrderBy(c => c.TimeToReachDestination).ToList();
            
            Customer fastestCustomer = remainingCustomers[0];

            MoveCustomerFromStartToPoint(fastestCustomer, destinationPoint);
            remainingCustomers.RemoveAt(0);
        }
        
    }

    private void MoveCustomerFromStartToPoint(Customer customer, Vector3 destinationPoint)
    {
        MoveCustomer(customer, destinationPoint);

        if(customer.CurrentState != CustomerState.WAITING_IN_LINE)
        {
            customer.SetState(CustomerState.WALKING);
        }
    }

    private static void MoveCustomer(Customer customer, Vector3 destinationPoint)
    {
        customer.SetDestination(destinationPoint);
        customer.SetIsStopped(false);
        customer.Destination = destinationPoint;
    }

    private List<Vector3> GetAvailableLinePoints()
    {
        return TableLineController.Instance.PointAvailabilityDict.Keys.Where(point => TableLineController.Instance.PointAvailabilityDict[point]).ToList();
    }


    private Customer GetRandomCustomer()
    {
        if (initializedCustomers.Count == 0) return null;
        
        int rand = Random.Range(0, initializedCustomers.Count);//Max should be 30
        
        return initializedCustomers[rand];
    }
    public void RegisterInitializedCustomer(Customer customer)
    {
        initializedCustomers.Add(customer);
    }

    public void RemoveFromInitializedList(Customer customer)
    {
        initializedCustomers.Remove(customer);
    }

    private void HandleReachingLine(Customer customer)
    {
        customer.OnReachingLine -= HandleReachingLine;

        walkingCustomerList.Remove(customer);

        waitingInLineList.Add(customer);
        
        customer.SetState(CustomerState.WAITING_IN_LINE);
        TableLineController.Instance.SetPointEmpty(customer.Destination, false);
        
        customer.OnInteractionEnded += HandleOnInteractionEnded;
        
        HandleInteraction(customer);
    }

    private void HandleInteraction(Customer customer)
    {
        interactionHandler.RegisterCustomer(customer);
        interactionHandler.OnQueueUpdated();
    }

    private void HandleOnInteractionEnded(Customer customer)
    {
        customer.OnInteractionEnded -= HandleOnInteractionEnded;
        
        TableLineController.Instance.SetPointEmpty(customer.Destination, true);
        
        MoveCustomer(customer, ExitPointTransform.position);

        waitingInLineList.Remove(customer);
        
        ReorganizeLine();
        
        UpdateWalkingCustomersDestination();

        interactionHandler.OnQueueUpdated();
    }

    private void ReorganizeLine()
    {
        lineOrganizer.ReorganizeCustomers(waitingInLineList);
        waitingInLineList = lineOrganizer.GetOrganizedCustomers();
        
        foreach (var waitingCustomer in waitingInLineList)
        {
            TableLineController.Instance.SetPointEmpty(waitingCustomer.Destination, true);

            Vector3 availablePoint = GetAvailableLinePoints().FirstOrDefault();
            MoveCustomer(waitingCustomer, availablePoint); //Destination Changed.

            TableLineController.Instance.SetPointEmpty(waitingCustomer.Destination, false);
        }
    }
}