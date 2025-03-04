using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;
using Random = UnityEngine.Random;


public class CustomerController : MonoBehaviour
{
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
        
        customer.OnReachingLine += HandleReachingLine;
    }

    private static void MoveCustomer(Customer customer, Vector3 destinationPoint)
    {
        customer.SetDestination(destinationPoint);
        customer.SetIsStopped(false);
        customer.Destination = destinationPoint;
    }

    private List<Vector3> GetAvailableLinePoints()
    {
        return TableLineController.Instance.WaitingLinePointsDict.Keys.Where(point => TableLineController.Instance.WaitingLinePointsDict[point]).ToList();
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

    public void RemoveCustomer(Customer customer)
    {
        initializedCustomers.Remove(customer);
    }

    private void HandleReachingLine(Customer customer)
    {
        customer.OnReachingLine -= HandleReachingLine;
        
        walkingCustomerList.Remove(customer);
        waitingInLineList.Add(customer);
        
        customer.SetState(CustomerState.WAITING_IN_LINE);
        TableLineController.Instance.SetEmpty(customer.Destination, false);
        
        customer.OnInteractionEnded += HandleOnOnInteractionEnded;
        
        //Start Interaction
    }

    private void HandleOnOnInteractionEnded(Customer customer)
    {
        customer.OnInteractionEnded -= HandleOnOnInteractionEnded;
        MoveCustomer(customer, ExitPointTransform.position);
        TableLineController.Instance.SetEmpty(customer.Destination, true);
        waitingInLineList.Remove(customer);
        
        UpdateWalkingCustomersDestination();

        
    }
}