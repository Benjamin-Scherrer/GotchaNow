using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverBench : MonoBehaviour
{
    public InputActionReference retryInput;
    public InputActionReference titleInput;
    
    void Update()
    {
        if (retryInput.action.IsPressed()) //debug : retry battle
        {
            Debug.Log("retry");
            GameOver.instance.RetryBattle();
        }
    }
}
