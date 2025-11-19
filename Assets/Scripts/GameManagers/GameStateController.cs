using System;

/// <summary>
/// The Game State Controller managers what state the game is in
/// This uses a version of the observer pattern when an event is sent out and all "observers" obey based on the message
/// </summary>
public static class GameStateController
{
    private static GameState _currentState = GameState.GamePlay;

    public static Action<GameState> OnStateChange;

    public static GameState GetCurrentState { get { return _currentState; } }

    /// <summary>
    /// If the newely requested state is not the current state, change the state
    /// Send a message to all objects listening to this change in state such as the player input listener
    /// </summary>
    public static void ChangeStateRequest(GameState newState)
    {
        if(_currentState == newState)
        {
            return;
        }

        _currentState = newState;
        OnStateChange?.Invoke(newState);
    }
}

public enum GameState
{
    GamePlay,
    Menu
}