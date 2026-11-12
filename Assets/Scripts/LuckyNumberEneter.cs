
using UnityEngine;
using UnityEngine.UI;

public class LuckyNumberEnter : MonoBehaviour
{
    [SerializeField] private Button confirmButton;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.Invoke();
            }
        }
    }
}
