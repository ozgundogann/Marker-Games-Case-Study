using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class LineOrganizer : MonoBehaviour
    {
        Dictionary<Vector3, Customer> customersInLineDict = new Dictionary<Vector3, Customer>();

        private void Start()
        {
            var points = TableLineController.Instance.PointAvailabilityDict.Keys.ToList();
            foreach (var point in points)
            {
                customersInLineDict.Add(point, null);
            }

            CustomerController.OnLineNeedToBeOrganized += UpdateDict;
        }

        private void OnDestroy()
        {
            CustomerController.OnLineNeedToBeOrganized -= UpdateDict;
        }

        public void UpdateDict(List<Customer> customers)
        {
            customersInLineDict.Clear();
            foreach (var customer in customers)
            {
                customersInLineDict[customer.Destination] = customer;
            }
        }

        public Customer GetFirstCustomer()
        {
            return customersInLineDict.Values.Count == 0 ? null : customersInLineDict.Values.FirstOrDefault();
        }
    }
}