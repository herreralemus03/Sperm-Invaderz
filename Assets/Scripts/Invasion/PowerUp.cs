using UnityEngine;
using UnityEngine.UI;

namespace Invasion
{
    public class PowerUp : MonoBehaviour
    {
        public static PowerUp Instance { private set; get; }
        public Image icon;
        public Slider slider;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void FixedUpdate()
        {
            slider.value = Mathf.Lerp(slider.value, PlayerController.Instance.powerDuration, slider.value >= PlayerController.Instance.powerDuration ? 0.03f : 0.125f);
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
