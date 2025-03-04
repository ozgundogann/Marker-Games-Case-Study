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
        }

        public void ReorganizeCustomers(List<Customer> customers)
        {
            foreach (var customer in customers)
            {
                customersInLineDict[customer.Destination] = customer;
            }

            foreach (var x in customersInLineDict.Keys)
            {
                Debug.Log("DEST: " + x);
            }
        }

        public List<Customer> GetOrganizedCustomers()
        {
            return customersInLineDict.Values.ToList();
        }
    }
}