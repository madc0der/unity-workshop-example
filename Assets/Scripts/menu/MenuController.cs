using System.Collections;
using DefaultNamespace;
using stat;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace menu
{
    public class MenuController : AbstractPanelController
    {
        public SceneFaderController fader;

        private void Start()
        {
            if (UserStatController.GetCurrentUserHolder() == null)
            {
                OnChangePlayerName();
            }
        }
        
        public void OnStartGame()
        {
            fader.FadeOutToWhite();
            UserStatController.SetLastScore(0);
            StartCoroutine(LoadLevel());
        }

        private IEnumerator LoadLevel()
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene("Level-1");
        }

        public void OnOptions()
        {
            
        }

        public void OnChangePlayerName()
        {
            PlayerNameInputController.GetInstance().Show(this);
        }

        public void OnShowBestScore()
        {
            BestScorePanelController.GetInstance().Show(this);
        }

        public void OnExit()
        {
            Application.Quit();
        }
    }
}