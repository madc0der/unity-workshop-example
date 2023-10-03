using System.Collections;
using DefaultNamespace.common;
using events;
using events.game;
using game;
using stat;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
    public class GameSceneController : MonoBehaviour
    {
        public SceneFaderController fader;
        
        public GameObject winLabel;
        public GameObject loseLabel;

        public int scorePointIncrement = 10;
        public int scoreFallDecrement = 5;
        
        private void Start()
        {
            CursorHelper.LockAndHideCursor();
            
            winLabel.SetActive(false);
            loseLabel.SetActive(false);
            
            EventBus.Subscribe(typeof(PlayerDieEvent), OnPlayerDie);
            EventBus.Subscribe(typeof(PlayerWinEvent), OnPlayerWin);
            EventBus.Subscribe(typeof(HitScorePointEvent), OnHitScorePoint);
            EventBus.Subscribe(typeof(PlayerHardFallEvent), OnPlayerHardFall);
        }

        private void OnPlayerDie(BasicEvent e)
        {
            fader.FadeOutToBlack();
            loseLabel.SetActive(true);
            StartCoroutine(WaitAndReload());
        }

        private void OnPlayerWin(BasicEvent e)
        {
            fader.FadeOutToWhite();
            winLabel.SetActive(true);
            StartCoroutine(WaitAndReload());
        }

        private void OnHitScorePoint(BasicEvent e)
        {
            var lastScore = UserStatController.IncreaseLastScore(scorePointIncrement);
            EventBus.Publish(new LastScoreUpdatedEvent(lastScore));
        }

        private void OnPlayerHardFall(BasicEvent e)
        {
            var lastScore = UserStatController.IncreaseLastScore(-scoreFallDecrement);
            EventBus.Publish(new LastScoreUpdatedEvent(lastScore));
        }

        private IEnumerator WaitAndReload()
        {
            yield return new WaitForSeconds(3f);
            UserStatController.Flush();
            CursorHelper.UnlockAndShowCursor();
            SceneManager.LoadScene("MenuScene");
        }
    }
}