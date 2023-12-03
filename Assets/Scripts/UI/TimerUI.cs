using System.Collections.Generic;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TimerUI : RepeatMonoBehaviour
    {
        [SerializeField] private List<Image> _imageList;

        private float _currentTimer;
        private int _lastTimer;
        private TimerThemeSO _timerThemeSo;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            foreach (Transform childTransform in transform)
                _imageList.Add(childTransform.GetComponent<Image>());
        }

        private void Start()
        {
            _timerThemeSo = GameManager.Instance.GameDataSo.TimerThemeSo;
            foreach (Image image in _imageList)
                image.sprite = _timerThemeSo.NumberSpritesList[0];
        }

        private void FixedUpdate()
        {
            if (!GameManager.Instance.IsGamePlaying()) return;
            CountToOneSecond();
        }

        private void CountToOneSecond()
        {
            _currentTimer += Time.fixedDeltaTime;
            if (_currentTimer - _lastTimer < 1) return;
            _lastTimer = (int)_currentTimer;
            UpdateTimerUI();
        }

        private void UpdateTimerUI()
        {
            int index = 0;
            int tempTimer = _lastTimer;
            while (tempTimer > 0 && index < _imageList.Count)
            {
                int temp = tempTimer % 10;
                tempTimer /= 10;
                SetImageAtIndex(index++, temp);
            }
        }

        private void SetImageAtIndex(int index, int value) => 
            _imageList[index].sprite = _timerThemeSo.NumberSpritesList[value];

        public float GetTime() => _currentTimer;
    }
}