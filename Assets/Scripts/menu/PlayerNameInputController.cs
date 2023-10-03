using events;
using stat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace menu
{
    public class PlayerNameInputController : AbstractPanelController
    {
        private static AbstractPanelController instance;

        public TMP_InputField nameInput;
        public Button saveButton;

        private UserStatHolder currentHolder;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
        }

        private void OnEnable()
        {
            nameInput.onValueChanged.AddListener(OnNameChanged);
            currentHolder = UserStatController.GetCurrentUserHolder();
            if (currentHolder != null)
            {
                nameInput.text = currentHolder.name;
            }
            else
            {
                nameInput.text = "";
            }
            OnNameChanged(nameInput.text);
        }

        private void OnDisable()
        {
            nameInput.onValueChanged.RemoveListener(OnNameChanged);
        }

        private void OnNameChanged(string value)
        {
            saveButton.interactable = value.Length > 0;
        }

        public static AbstractPanelController GetInstance()
        {
            return instance;
        }

        public void SaveAndHide()
        {
            var holder = UserStatController.SetCurrentUser(nameInput.text);
            EventBus.Publish(new UserChangedEvent(holder));
            Hide();
        }
    }
}