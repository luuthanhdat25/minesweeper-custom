using RepeatUtil.DesignPattern.SingletonPattern;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : SingletonDestroyOnLoad<GameManager>
    {
        [SerializeField] private GameDataSO _gameDataSo;
        public GameDataSO GameDataSo => _gameDataSo;
       
    }
}