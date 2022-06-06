using UnityEngine;

namespace Arcade
{
    public class ScoreArcadeMode : MonoBehaviour
    {
        public static ScoreArcadeMode Instance { private set; get; }
        public long score;
        public int level = 1;
        public int attempts = 1;
        public int playerLife = 3;
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
    }
}