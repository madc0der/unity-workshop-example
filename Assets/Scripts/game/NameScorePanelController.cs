using events;
using menu;
using stat;
using TMPro;
using UnityEngine;

namespace game
{
    public class NameScorePanelController : MonoBehaviour
    {
        public TMP_Text label;
        public bool useLastScore;
        private UserStatHolder currentHolder;

        private void Start()
        {
            EventBus.Subscribe(typeof(UserChangedEvent), OnUserChanged);
            EventBus.Subscribe(typeof(LastScoreUpdatedEvent), OnLastScoreUpdated);
            currentHolder = UserStatController.GetCurrentUserHolder();
            UpdateCurrentScore();
        }

        private void OnUserChanged(BasicEvent e)
        {
            currentHolder = UserStatController.GetCurrentUserHolder();
            UpdateCurrentScore();
        }

        private void OnLastScoreUpdated(BasicEvent e)
        {
            UpdateCurrentScore();
        }

        private void UpdateCurrentScore()
        {
            if (useLastScore)
            {
                label.text = $"{currentHolder.name}: {UserStatController.GetLastScore()}";
                return;
            }
            
            if (currentHolder != null)
            {
                label.text = $"{currentHolder.name}: {currentHolder.bestScore}";
            }
            else
            {
                label.text = "";
            }
        }
    }
}