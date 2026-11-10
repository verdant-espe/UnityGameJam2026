using UnityEngine;
using TMPro;
public class LuckyNumberManager : MonoBehaviour
{
    private int selectedLuckyNumber;
    public bool HasLuckyNumber => selectedLuckyNumber >= 1 && selectedLuckyNumber <= 100;

#if UNITY_EDITOR
    [SerializeField] private bool forceLuckyMatchForTesting = false;
#endif
    [SerializeField] private TMP_InputField luckyNumberInput;
    [SerializeField] private GameObject luckyNumberPanel;
    private string lastValidInput = "";
    private bool hasRolledForSecondChance= false;

    public void SetLuckyNumber(int number)
    { 
       if (number < 1 || number > 100)
        {
            Debug.LogError("Lucky number must be between 1 and 100.");
            return;
        }

        selectedLuckyNumber = number;
        Debug.Log("Selected Lucky Number: " + selectedLuckyNumber);
    }

    public void ConfirmLuckyNumber()
    {
        if (int.TryParse(luckyNumberInput.text, out int number)
            && number >= 1 && number <= 100)
        {
            SetLuckyNumber(number);

            if (luckyNumberPanel != null)
            {
                luckyNumberPanel.SetActive(false);
            }
        }
    }

    public void ValidateLuckyNumberInput(string typedText)
    {
        if (string.IsNullOrEmpty(typedText))
        {
            lastValidInput = "";
            return;
        }
        if (int.TryParse(typedText, out int number) && number >= 1 && number <= 100)
        {
            lastValidInput = typedText;
        }
        else
        {
            luckyNumberInput.SetTextWithoutNotify(lastValidInput);
        }

    }
    public bool TrySecondChance()
    {
        if (hasRolledForSecondChance)
        {
            return false;
        }

        if (!HasLuckyNumber)
        {
            Debug.LogWarning("No lucky number selected!");
            return false;
        }

        hasRolledForSecondChance = true;

        int rolledNumber = Random.Range(1, 101);
#if UNITY_EDITOR
        if (forceLuckyMatchForTesting)
        {
            rolledNumber = selectedLuckyNumber;
        }
#endif

        if (rolledNumber == selectedLuckyNumber)
        {
            Debug.Log("Congratulations! You rolled your lucky number: " + rolledNumber);
            return true;
        }
        else
        {
            Debug.Log("Rolled number: " + rolledNumber + ". Better luck next time!");
            return false;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
