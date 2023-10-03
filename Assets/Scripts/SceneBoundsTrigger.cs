using events;
using events.game;
using UnityEngine;

namespace DefaultNamespace
{
    public class SceneBoundsTrigger : MonoBehaviour
    {
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                EventBus.Publish(new PlayerDieEvent());
            }
        }
    }
}