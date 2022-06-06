using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Common
{
    public static class DataManagerUtility
    {
    
        public static string GetCreditsAmountWithStringFormat()
        {
            return FormatAmount(GetCreditsAmount());
        }
        public static long GetCreditsAmount()
        {
            var amount = Convert.ToInt64(PlayerPrefs.GetString("credits", "0"));
            return amount;
        }

        public static void AddCreditsAmount(long amount)
        {
            var newValue = GetCreditsAmount() + amount;
            PlayerPrefs.SetString("credits", newValue.ToString());
        }
        public static void SubtractCreditsAmount(long amount)
        {
            var newValue = GetCreditsAmount() - amount;
            PlayerPrefs.SetString("credits", newValue.ToString());
        }
        public static string FormatAmount(long amount)
        {
        
            if (amount >= 1000000000L)
            {
                return $"{amount / 1000000f:0.00}B";
            }

            if (amount >= 1000000L)
            {
                return $"{amount / 1000000f:0.00}M";
            }
        
            if (amount >= 1000L)
            {
                return $"{amount / 1000f:0.00}K";
            }

            return $"{amount}";
        }
        public static float SecondsToFloat(float value)
        {
            return value / 60f;
        }
        public static float GetModifiedVolumeForPause(float volume)
        {
            var volumeModifiedOnPause = volume - 10f < -80f ? -80f : volume - 10f;
            return volumeModifiedOnPause;
        }
        public static void Vibrate()
        {
            Handheld.Vibrate();
        }
        public static int GetDec(int level)
        {
            return level % 10;
        }
        public static float GetRepeatRateSpawn(int level) => Mathf.Exp(Mathf.Log(level, 0.1f)) * 8f;
        public static float GetRepeatRateSpawnForInfiniteMode(int level) => Mathf.Exp(Mathf.Log(level, 0.1f)) * 2f;
        public static float GetShotForce(int level) => Mathf.Log(level + 3, 3f) * 70f;
        public static float GetShotEarns(int level) => Mathf.Log(level + 1, 200f) * 70f;
        public static float GetShotSpeed(int level) => Mathf.Exp(Mathf.Log(level + 1, 0.05f));

        public static float GetScreenWidth()
        {
            Vector2 topRightCorner = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
            return edgeVector.x;
        }

        public static float GetScreenHeight()
        {
            Vector2 topRightCorner = new Vector2(1, 1);
            Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
            return edgeVector.y;
        }

        public static bool CanFertilize(float actualFertility)
        {
            return Mathf.RoundToInt(Random.Range(1f, 11f - (actualFertility / 10f))) == 1;
        }
    }
}

public enum AdRewardType { Retry, Coins, PowerUpgrade, SpeedUpgrade, EarnsUpgrade }
public enum GameMode { Adventure, Arcade, Endless }
public enum GameState
{
    Playing,
    Paused,
    Retry,
    Ended,
    Cleared
}
public enum UpgradeType { None, Power, Speed, Earns }
public enum SpermsType { Protected3, Protected2, Protected, FasterSperm, Beast, Ninja, Normal, None}
public enum PowerType { None, Speed, Force, Duplicate, Penetrating, Everything}