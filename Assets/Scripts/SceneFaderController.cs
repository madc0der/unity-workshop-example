using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SceneFaderController : MonoBehaviour
    {
        public float fadeInTime = 3f;
        public float fadeOutTime = 3f;
        
        private RawImage faderImage;

        private void Awake()
        {
            faderImage = GetComponent<RawImage>();
            faderImage.color = Color.black;
        }

        private void Start()
        {
            faderImage.color = Color.black;
            faderImage.CrossFadeAlpha(0f, fadeInTime, true);
        }

        public void FadeOutToWhite()
        {
            faderImage.color = Color.white;
            faderImage.CrossFadeAlpha(1f, fadeOutTime, true);
        }

        public void FadeOutToBlack()
        {
            faderImage.color = Color.black;
            faderImage.CrossFadeAlpha(1f, fadeOutTime, true);
        }
    }
}