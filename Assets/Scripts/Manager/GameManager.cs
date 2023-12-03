using System;
using DefaultNamespace;
using RepeatUtil.DesignPattern.SingletonPattern;
using UnityEngine;

namespace Manager
{
    public class GameManager : SingletonDestroyOnLoad<GameManager>
    {
        [SerializeField] private GameDataSO _gameDataSo;
        public GameDataSO GameDataSo => _gameDataSo;
       
        public event EventHandler OnGamePaused;
        public event EventHandler OnGameUnpaused;
        public event EventHandler OnGameOver;
        public event EventHandler OnGameWin;
        
        private enum State
        {
            GamePlaying,
            PauseGame,
            GameOver,
            WinGame
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
            if (IsGameOver()) return;
            
            if (IsPauseGame()) state = State.GamePlaying;
            else state = State.PauseGame;
            
            if (IsPauseGame()) OnGamePaused?.Invoke(this, EventArgs.Empty);
            else  OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }

        public bool IsPauseGame() => state == State.PauseGame;
        
        public bool IsGamePlaying() => state == State.GamePlaying;
        
        public bool IsGameOver() => state == State.GameOver;
        
        public bool IsWinGame() => state == State.WinGame;

        public void GameOver()
        {
            state = State.GameOver;
            OnGameOver?.Invoke(this, EventArgs.Empty);
        }
        
        public void WinGame()
        {
            state = State.WinGame;
            OnGameWin?.Invoke(this, EventArgs.Empty);
        }
    }
}