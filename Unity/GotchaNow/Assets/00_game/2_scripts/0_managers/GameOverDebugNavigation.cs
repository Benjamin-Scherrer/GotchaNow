using UnityEngine;
using UnityEngine.InputSystem;
public class GameOverDebugNavigation : MonoBehaviour
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

        if (titleInput.action.IsPressed()) //debug : go to title
        {
            Debug.Log("title");
            GameOver.instance.GoToTitle();
        }
    }
    
}
