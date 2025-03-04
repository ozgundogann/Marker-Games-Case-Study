using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class CustomerInteractionHandler : MonoBehaviour
    {
        Queue<Customer> waitingCustomers = new Queue<Customer>();
        
        private bool isInteracting = false;

        public void RegisterCustomer(Customer customer)
        {
            waitingCustomers.Enqueue(customer);
        }

        public void RemoveCustomer(Customer customer)
        {
            if (waitingCustomers.Count > 0)
            {
                waitingCustomers.Dequeue();
                TryStartNextInteraction();
            }
        }

        public void TryStartNextInteraction()
        {
            if (waitingCustomers.Count == 0) return;
            
            isInteracting = true;
            Customer nextCustomer = waitingCustomers.Peek();
            nextCustomer.StartInteraction();
            nextCustomer.OnInteractionEnded += HandleOnInteractionEnded;
        }

        private void HandleOnInteractionEnded(Customer customer)
        {
            customer.OnInteractionEnded -= HandleOnInteractionEnded;
            isInteracting = false;
            RemoveCustomer(customer);
        }

        public void OnQueueUpdated()
        {
            if(!isInteracting)
                TryStartNextInteraction();
        }
    }
}