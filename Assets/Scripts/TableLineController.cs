using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    public class TableLineController : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        
        private Dictionary<Vector3, bool> waitingLinePointsDict = new Dictionary<Vector3, bool>();

        public static TableLineController Instance { get; private set; }

        public Dictionary<Vector3, bool> WaitingLinePointsDict => waitingLinePointsDict;

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
                
                waitingLinePointsDict.Add(linePoint.transform.position, true);
            }
        }

        public void SetEmpty(Vector3 point, bool isEmpty)
        {
            waitingLinePointsDict[point] = isEmpty;
        }
    }
}