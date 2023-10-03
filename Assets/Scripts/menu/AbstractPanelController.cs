using UnityEngine;

namespace menu
{
    public class AbstractPanelController : MonoBehaviour
    {
        public bool showOnStart;
        private AbstractPanelController previous;

        protected virtual void Awake()
        {
            if (!showOnStart)
            {
                Hide();
            }
        }

        public void Show(AbstractPanelController previous)
        {
            this.previous = previous;
            previous.Hide();
            Restore();
        }

        public void Restore()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            previous?.Restore();
        }
    }
}