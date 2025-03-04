using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class InputReader : MonoBehaviour
    {
        public static event Action OnSpacePressed;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnSpacePressed?.Invoke();
            }
        }
    }
}