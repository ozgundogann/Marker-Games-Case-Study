using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;


public class CustomerController : MonoBehaviour
{
    [SerializeField] private CustomerInteractionHandler interactionHandler;
    
    
    
    [SerializeField] private List<Customer> initializedCustomers;
    [SerializeField] private List<Customer> walkingCustomerList;
    [SerializeField] private List<Customer> waitingInLineList;
    [SerializeField] private Transform ExitPointTransform;
    
    
    public static event Action<List<Customer>> OnLineNeedToBeOrganized;
    public static event Action<List<Customer>> OnLineUpdated;
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

        
        UpdateWalkingCustomersDestinations();
    }

    private void UpdateWalkingCustomersDestinations()
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
        customer.OnInteractionEnded += HandleOnInteractionEnded;
        customer.SetState(CustomerState.WAITING_IN_LINE);
        
        TableLineController.Instance.SetPointEmpty(customer.Destination, false);
        
        walkingCustomerList.Remove(customer);
        waitingInLineList.Add(customer);
        
        OrganizeLine();
    }

    private void HandleOnInteractionEnded(Customer customer)
    {
        customer.OnInteractionEnded -= HandleOnInteractionEnded;
        interactionHandler.SetInteracting(false);
        TableLineController.Instance.SetPointEmpty(customer.Destination, true);
        MoveCustomer(customer, ExitPointTransform.position);
        waitingInLineList.Remove(customer);
        
        OrganizeLine();
    }

    private void OrganizeLine()
    {
        OnLineNeedToBeOrganized?.Invoke(waitingInLineList);
        MoveReorganizedLine();
        UpdateWalkingCustomersDestinations();
        OnLineUpdated?.Invoke(waitingInLineList);
    }

    private void MoveReorganizedLine()
    {
        foreach (var waitingCustomer in waitingInLineList)
        {
            TableLineController.Instance.SetPointEmpty(waitingCustomer.Destination, true);

            Vector3 availablePoint = TableLineController.Instance.GetAvailablePoint();
            
            if(availablePoint == Vector3.one * -1) return;
            
            MoveCustomer(waitingCustomer, availablePoint); //Destination Changed.

            TableLineController.Instance.SetPointEmpty(waitingCustomer.Destination, false);
        }
    }
}