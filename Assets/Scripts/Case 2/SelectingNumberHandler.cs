using System;
using TMPro;
using UnityEngine;

namespace Case_2
{
    public class SelectingNumberHandler : MonoBehaviour
    {
        private int firstNum = -1;
        private int secondNum = -1;
        private int thirdNum = -1;
        
        public static event Action<int, int, int> OnNumbersUpdated; 
        public static event Action<int, int, int> OnNumbersSelected; 

        private void Start()
        {
            UIInputReader.OnInput += UpdateSelectedNumberText;
        }

        private void UpdateSelectedNumberText(int num)
        {
            if(firstNum == -1)
                firstNum = num;
            else if (secondNum == -1)
                secondNum = num;
            else if (thirdNum == -1)
            {
                thirdNum = num;
                UIInputReader.OnInput -= UpdateSelectedNumberText;
                OnNumbersSelected?.Invoke(firstNum, secondNum, thirdNum);
            }

            OnNumbersUpdated?.Invoke(firstNum, secondNum, thirdNum);
        }
    }
}