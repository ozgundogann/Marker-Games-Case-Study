using System;
using UnityEngine;

namespace Case_2
{
    public class UIInputReader : MonoBehaviour
    {
        public static event Action<int> OnInput;
        
        public static void GetNumber(int number)
        {
            OnInput?.Invoke(number);
        }
    }
}