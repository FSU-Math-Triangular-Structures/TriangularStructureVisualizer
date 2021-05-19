using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// # IncrementSteps.cs

/// The IncrementSteps class is used to respond to the user clicking 
/// the + and - buttons on the UI.
public class IncrementSteps : MonoBehaviour
{
    /// ## Properties
    
    /// Current number of generations that are drawn
    public int current;

    /// UI label that displays the number of generations
    public TMP_Text label;

    /// UI increment button
    public Button incrementButton;

    /// UI decrement button
    public Button decrementButton;

    /// ## Methods

    /// *Unity Function* 
    /// 
    /// **Overview** Sets `References.StepsManager` to this object.
    void Awake()
    {
        References.StepsManager = this;
    }

    /// *Unity Function* 
    /// 
    /// **Overview** Inits the current generations to 0.
    void Start()
    {
        current = 0;
    }

    /// **Overview** Reset the current generations to 0 and update label.
    public void Reset()
    {
        current = 0;
        label.text = current.ToString();
    }

    /// **Overview** When the + button is clicked, increment the current generations.
    /// This function also calls the Trungle Manager's `AddGenerations` function.
    public void Increment()
    {
        current++;
        label.text = current.ToString();

        References.TrungleManager.AddGenerations();
    }

    /// **Overview** When the - button is clicked, decrement the current generations.
    /// This function also calls the Trungle Manager's `DeleteGenerations` function.
    public void Decrement()
    {
        current--;

        if (current < 0)
            current = 0;

        label.text = current.ToString();

        References.TrungleManager.DeleteGenerations(current);
    }
}
