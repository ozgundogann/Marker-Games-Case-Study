using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Case_2
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject firstScreen;
        [SerializeField] private GameObject secondScreen;

        [SerializeField] private List<Button> numberSelectionButtons;
        
        
        [SerializeField] List<TMP_Text> diceThrowingTexts;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text selectedNumberTxt;

        [SerializeField] private TMP_Text thrownDiceCountText;
        [SerializeField] private TMP_Text totalSumText;
        [SerializeField] private TMP_Text threeDiceSumText;
        

        private void Start()
        {
            SelectingNumberHandler.OnNumbersSelected += HandleOnNumbersSelected;
            SelectingNumberHandler.OnNumbersUpdated += HandleOnNumbersUpdated;
            DiceThrowingManager.OnValuesUpdated += HandleOnValuesUpdated;
            DiceThrowingManager.OnDiceThrown += HandleThrowingDices;
            
            inputField.onValueChanged.AddListener(OnInputFieldUpdated);
            
            firstScreen.SetActive(true);
            secondScreen.SetActive(false);
        }

        private void HandleOnNumbersSelected(int firstNumber, int secondNumber, int thirdNumber)
        {
            SelectingNumberHandler.OnNumbersSelected -= HandleOnNumbersSelected;
            
            firstScreen.SetActive(false);
            secondScreen.SetActive(true);
        }

        private void HandleThrowingDices(int arg1, int arg2, int arg3)
        {
            diceThrowingTexts[0].text = arg1.ToString();
            diceThrowingTexts[1].text = arg2.ToString();
            diceThrowingTexts[2].text = arg3.ToString();
        }

        private void OnInputFieldUpdated(string input)
        {
            if (int.TryParse(input, out int faces) && faces > 1)
            {
                DiceThrowingManager.MaxDiceFaces = faces;
                
                //TODO
            }
            
            
        }

        private void HandleOnValuesUpdated(int thrownDiceAmount, int diceValuesSum, int threeDiceValuesSum)
        {
            thrownDiceCountText.text = $"Throw Count: {thrownDiceAmount}";
            totalSumText.text = $"TOTAL: {diceValuesSum}";
            threeDiceSumText.text = $"Dice Total: {threeDiceValuesSum}";
        }

        private void HandleOnNumbersUpdated(int firstNumber, int secondNumber, int thirdNumber)
        {
            SelectingNumberHandler.OnNumbersSelected -= HandleOnNumbersUpdated;

            selectedNumberTxt.text =
                $"{(firstNumber == -1 ? "?" : firstNumber)} - {(secondNumber == -1 ? "?" : secondNumber)} - {(thirdNumber == -1 ? "?" : thirdNumber)}";
        }
        
    }
}