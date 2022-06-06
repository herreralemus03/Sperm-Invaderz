using UnityEngine;

namespace Common
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { private set; get; }
        public AudioSource evolutionClip, evolutionDoneClip;
        public AudioSource
            explosionAudio, 
            loseAudio, 
            shootAudio, 
            playerCollisionAudio, 
            punchAudio, 
            boingAudio, 
            boingAudio2, 
            whistleAudio, 
            levelClear, 
            levelFail, 
            switchTabAudio, 
            upgradeAudio, 
            clickAudio, 
            splashAudio,
            creditsIncreaseAudio,
            spermSwimAudio,
            swimFast1Audio,
            swimFast2Audio,
            swimMedium1Audio,
            swimMedium2Audio,
            swimSoft1Audio,
            swimSoft2Audio,
            spawnAudio,
            dashAudio,
            fillAudio,
            scoreAudio,
            clearAudio,
            failAudio,
            gameOverAudio,
            missAudio,
            swordAudio, 
            dizzyBirds,
            spitAudio,
            trumptAudio, 
            lifeUp;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayPopAudio() => clickAudio.Play();
        public void PlaySplashAudio() => splashAudio.Play();
    }
}