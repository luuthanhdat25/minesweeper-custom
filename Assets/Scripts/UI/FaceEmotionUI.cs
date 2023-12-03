using System;
using System.Collections;
using Grid;
using Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class FaceEmotionUI : RepeatMonoBehaviour
    {
        [SerializeField] private Image _faceImage;
        
        private const float TIME_EMOITON = 0.3f;
        private FaceThemeSO _faceThemeSo;
        
        protected override void LoadComponents()
        {
            base.LoadComponents();
            if(_faceImage != null) return;
            this._faceImage = GetComponent<Image>();
        }

        private void Start()
        {
            SetStartUpFace();

            MyGrid.Instance.OnChooseTileAction += MyGridOn_ChooseTileAction;
            GameManager.Instance.OnGameOver += GameManager_OnGameOver;
        }

        private void SetStartUpFace()
        {
            _faceThemeSo = GameManager.Instance.GameDataSo.FaceThemeSo;
            _faceImage.sprite = _faceThemeSo.Smiles;
        }

        private void MyGridOn_ChooseTileAction(object sender, EventArgs e) => StartCoroutine(WowEmotion());

        private IEnumerator WowEmotion()
        {
            _faceImage.sprite = _faceThemeSo.Wow;
            yield return new WaitForSeconds(TIME_EMOITON);
            _faceImage.sprite = _faceThemeSo.Smiles;
            if(GameManager.Instance.IsWinGame()) _faceImage.sprite = _faceThemeSo.Cool;
        }
        
        private void GameManager_OnGameOver(object sender, EventArgs e) => _faceImage.sprite = _faceThemeSo.Sad;

        public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}