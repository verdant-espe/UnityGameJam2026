using UnityEngine;

public class LuckyNumberManager : MonoBehaviour
{
    private int selectedLuckyNumber;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetLuckyNumber(42); // Example of setting a lucky number
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
