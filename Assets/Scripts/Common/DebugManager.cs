using System;
using TMPro;
using UnityEngine;

namespace Common
{
    public class DebugManager : MonoBehaviour
    {
        #if UNITY_EDITOR || DEBUG 
        public TextMeshProUGUI fpsText;

        private int m_frameCounter = 0;
        private float m_timeCounter = 0.0f;
        private float m_lastFramerate = 0.0f;
        public float m_refreshTime = 0.5f;
        public static DebugManager Instance { private set; get; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else
            {
                Destroy(gameObject); 
            }
        }

        private void Update()
        {
            if( m_timeCounter < m_refreshTime )
            {
                m_timeCounter += Time.deltaTime;
                m_frameCounter++;
            }
            else
            {
                //This code will break if you set your m_refreshTime to 0, which makes no sense.
                m_lastFramerate = (float)m_frameCounter/m_timeCounter;
                m_frameCounter = 0;
                m_timeCounter = 0.0f;
            }

            fpsText.text = $"{ m_lastFramerate : 0.0} FPS";
        }
        #endif
    }
}