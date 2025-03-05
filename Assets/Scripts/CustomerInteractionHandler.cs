using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class CustomerInteractionHandler : MonoBehaviour
    {
        [SerializeField] private LineOrganizer lineOrganizer;
        
        private bool isInteracting = false;

        private void Start()
        {
            CustomerController.OnLineUpdated += HandleOnLineUpdated;
        }

        private void OnDestroy()
        {
            CustomerController.OnLineUpdated -= HandleOnLineUpdated;
        }

        private void HandleOnLineUpdated(List<Customer> customers)
        {
            TryStartNextInteraction();
        }

        public void TryStartNextInteraction()
        {
            if (isInteracting) return;

            Customer firstCustomer = lineOrganizer.GetFirstCustomer();
            if (!firstCustomer) return;

            isInteracting = true;
            firstCustomer.StartInteraction();
        }

        public void SetInteracting(bool isInteracting)
        {
            this.isInteracting = isInteracting;
        }

        
    }
}