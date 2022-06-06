using System;
using UnityEngine;
using UnityEngine.UI;

namespace Invasion
{
    public class HealthController : MonoBehaviour
    {
        public Slider slider;
        public Color32 colorLow;
        public Color32 colorHigh;

        public void DestroySlider()
        {
            Destroy(gameObject);
        }
    }
}