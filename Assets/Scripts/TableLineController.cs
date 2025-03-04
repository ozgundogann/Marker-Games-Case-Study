using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class TableLineController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        
        private Dictionary<Vector3, bool> pointAvailabilityDict = new Dictionary<Vector3, bool>();

        public static TableLineController Instance { get; private set; }

        public Dictionary<Vector3, bool> PointAvailabilityDict => pointAvailabilityDict;

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
        }

        private void Start()
        {
            for (int i = 0; i < CustomerInitializer.MaxCustomerCount; i++)
            {
                GameObject linePoint = new GameObject();
                linePoint.transform.SetParent(transform);
                linePoint.transform.position = new Vector3(0,0, transform.position.z - i * 2 - 2);
                
                PointAvailabilityDict.Add(linePoint.transform.position, true);
            }
        }

        public void SetPointEmpty(Vector3 point, bool isEmpty)
        {
            pointAvailabilityDict[point] = isEmpty;
        }
    }
}