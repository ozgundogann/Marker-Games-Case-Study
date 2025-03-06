using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Case_2
{
    public class DiceThrowingManager : MonoBehaviour
    {
        private int[] diceValues = new int[3];
        
        private int throwCount = 0;
        private int diceValuesTotalSum = 0;
        private int threeDiceValuesSum = 0;
        
        public static int MaxDiceFaces = 6;
        
        public static event Action<int, int, int> OnValuesUpdated;
        public static event Action<int, int, int> OnDiceThrown;

        Dictionary<int, int> cheatedValues = new Dictionary<int, int>();
        private int firstNumber = 0;
        private int secondNumber = 0;
        private int thirdNumber = 0;
        
        private void Start()
        {
            SelectingNumberHandler.OnNumbersSelected += HandleSelectedNumber;
        }

        private void HandleSelectedNumber(int firstNum, int secondNum, int thirdNum)
        {
            SelectingNumberHandler.OnNumbersSelected -= HandleSelectedNumber;
            firstNumber = firstNum;
            secondNumber = secondNum;
            thirdNumber = thirdNum;
            AssignCheatedValues();

            foreach (var kvp in cheatedValues)
            {
                Debug.Log("Index: " + kvp.Key + "Val: " + kvp.Value);
            }
        }

        public void ThrowDice()
        {
            if (cheatedValues.ContainsKey(throwCount + 1))
            {
                int targetSum = cheatedValues[throwCount + 1];

                do
                {
                    for (int i = 0; i < diceValues.Length; i++)
                    {
                        diceValues[i] = Random.Range(1, MaxDiceFaces + 1);
                    }
                } while (diceValues.Sum() != targetSum);
            }
            else
            {
                for (int i = 0; i < diceValues.Length; i++)
                {
                    diceValues[i] = Random.Range(1, MaxDiceFaces + 1);
                }
            }

            throwCount++;
            threeDiceValuesSum = diceValues.Sum();
            diceValuesTotalSum += threeDiceValuesSum;

            OnValuesUpdated?.Invoke(throwCount, diceValuesTotalSum, threeDiceValuesSum);
            OnDiceThrown?.Invoke(diceValues[0], diceValues[1], diceValues[2]);

        }

        private void AssignCheatedValues()
        {
            AssignValueToRandomIndex(1, 10, cheatedValues, firstNumber);
            AssignValueToRandomIndex(5, 15, cheatedValues, secondNumber);
            AssignValueToRandomIndex(10, 20, cheatedValues, thirdNumber);
        }

        private void AssignValueToRandomIndex(int minIndex, int maxIndex, Dictionary<int, int> selectedNumbers, int numberToAssign)
        {
            int index;
    
            do
            {
                index = Random.Range(minIndex, maxIndex + 1);
            } while (selectedNumbers.ContainsKey(index));

            selectedNumbers[index] = numberToAssign;
        }

        
    }
}