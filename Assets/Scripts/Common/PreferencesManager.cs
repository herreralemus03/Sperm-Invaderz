using UnityEngine;
using UnityEngine.Audio;

namespace Common
{
    public class PreferencesManager : MonoBehaviour
    {
        public AudioMixer music, sfx, ui;
        public static PreferencesManager Instance { private set; get; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void SetSfxVolume(float volume)
        {
            PlayerPrefs.SetFloat("sfx_volume", volume);
        }
    
        public void SetMusicVolume(float volume)
        {
            var modifiedVolume = DataManagerUtility.GetModifiedVolumeForPause(volume);
            music.SetFloat("music_volume", modifiedVolume);
            PlayerPrefs.SetFloat("music_volume", volume);
        }
    
        public void SetUiVolume(float volume)
        {
            ui.SetFloat("ui_volume", volume);
            PlayerPrefs.SetFloat("ui_volume", volume);
        }

        public void SetSfxAndUiVolume(float volume)
        {
            SetSfxVolume(volume);
            SetUiVolume(volume);
        }

        public void SetVibration(bool value)
        {
            PlayerPrefs.SetString("vibrate", value.ToString());
            if(value) DataManagerUtility.Vibrate();
        }

        public bool GetVibration() => bool.Parse(PlayerPrefs.GetString("vibrate", "true"));
        public float GetSfxVolume() => PlayerPrefs.GetFloat("sfx_volume", 0f);
        public float GetMusicVolume() => PlayerPrefs.GetFloat("music_volume", 0f);
        public float GetUiVolume() => PlayerPrefs.GetFloat("ui_volume", 0f);

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}