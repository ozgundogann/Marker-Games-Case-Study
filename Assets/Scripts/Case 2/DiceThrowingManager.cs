using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public void ThrowDice()
        {
            if (throwCount >= 20)
                return;

            int remainingThrows = 20 - throwCount;
            int minPerThrow = 3;
            int maxPerThrow = MaxDiceFaces * 3;
            int targetSum;

            targetSum = ThrowCalculatedDiceValues(minPerThrow, maxPerThrow, remainingThrows);
            
            var rollSum = ThrowDiceUntilTargetSumOccurs(targetSum);

            throwCount++;
            threeDiceValuesSum = rollSum;
            diceValuesTotalSum += rollSum;

            OnValuesUpdated?.Invoke(throwCount, diceValuesTotalSum, threeDiceValuesSum);
            OnDiceThrown?.Invoke(diceValues[0], diceValues[1], diceValues[2]);
        }

        private int ThrowDiceUntilTargetSumOccurs(int targetSum)
        {
            int rollSum;
            int breakLoopValue = 0;
            do
            {
                for (int i = 0; i < diceValues.Length; i++)
                {
                    diceValues[i] = Random.Range(1, MaxDiceFaces + 1);
                }
                rollSum = diceValues.Sum();
                
                breakLoopValue++;
                
                if (breakLoopValue > 10000)
                {
                    Debug.Log("Target: " + targetSum + " rollSum: " + rollSum);
                    Debug.LogError("INFINITE ERROR HANDLED");
                    break;
                }
                
            } while (rollSum != targetSum);

            return rollSum;
        }

        private int ThrowCalculatedDiceValues(int minPerThrow, int maxPerThrow, int remainingThrows)
        {
            int targetSum;
            int worstCasePerThrow = 9;

            if (cheatedValues.ContainsKey(throwCount + 1))
            {
                targetSum = cheatedValues[throwCount + 1];
            }
            else
            {
                int worstCaseFuture = (remainingThrows - 1) * worstCasePerThrow;
        
                targetSum = 200 - diceValuesTotalSum - worstCaseFuture;
        
                targetSum = Mathf.Clamp(targetSum, minPerThrow, maxPerThrow);
            }

            return targetSum;
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
            int breakLoopValue = 0;

            do
            {
                index = Random.Range(minIndex, maxIndex + 1);
                
                breakLoopValue++;
                
                if (breakLoopValue > 10000)
                {
                    Debug.LogError("INFINITE ERROR HANDLED");
                    break;
                }
                
            } while (selectedNumbers.ContainsKey(index));

            

            selectedNumbers[index] = numberToAssign;
        }

        
    }
}