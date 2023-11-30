using System;
using RepeatUtil.DesignPattern.SingletonPattern;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : SingletonDestroyOnLoad<GameManager>
    {
        [SerializeField] private GameDataSO _gameDataSo;
        public GameDataSO GameDataSo => _gameDataSo;
       
        public event EventHandler OnGamePaused;
        public event EventHandler OnGameUnpaused;
        public event EventHandler OnGameOver;
        
        private enum State
        {
            GamePlaying,
            PauseGame,
            GameOver,
        }
        
        private State state;

        private void Start()
        {
            state = State.GamePlaying;
            InputManager.Instance.OnPauseAction += InputManager_OnPauseAction;
        }
        
        private void InputManager_OnPauseAction(object sender, EventArgs e)
            => TogglePauseGame();

        public void TogglePauseGame()
        {
            if (IsPauseGame()) state = State.GamePlaying;
            else state = State.PauseGame;
            
            if (IsPauseGame()) OnGamePaused?.Invoke(this, EventArgs.Empty);
            else  OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }

        public bool IsPauseGame() => state == State.PauseGame;

        public void GameOver()
        {
            state = State.GameOver;
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
    }
}