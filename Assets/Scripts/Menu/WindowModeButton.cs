using UnityEngine;

public enum WindowModeButtonMode
{
    Maximize = 0,
    RestoreWindow = 1,
    TestVictory = 2
}

public class WindowModeButton : MonoBehaviour
{
    [SerializeField] private WindowModeButtonMode mode = WindowModeButtonMode.Maximize;
    [SerializeField] private GameController gameController;

    public void Initialize(GameController controller, WindowModeButtonMode buttonMode)
    {
        gameController = controller;
        mode = buttonMode;
    }

    private void OnMouseDown()
    {
        if (gameController == null)
        {
            gameController = FindObjectOfType<GameController>();
            if (gameController == null)
            {
                return;
            }
        }

        if (mode == WindowModeButtonMode.Maximize)
        {
            gameController.SetWindowMaximized();
        }
        else if (mode == WindowModeButtonMode.RestoreWindow)
        {
            gameController.SetWindowedMode();
        }
        else
        {
            gameController.TriggerVictoryEffectsTest();
        }
    }
}
