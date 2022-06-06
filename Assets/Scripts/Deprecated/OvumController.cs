﻿using Common;
using Invasion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OvumController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI fertilityText;
    public Slider fertilitySlider;
    private bool mLevelCleared;
    private float mFertility;
    public float damageAmount;
    public bool isInmune;
    public bool alreadyFertilized;
    private void Start()
    {
        var level = SpermSpawnController.Instance.test
            ? SpermSpawnController.Instance.actualLevel
            : PlayerPrefs.GetInt("level", 1);
        damageAmount = Mathf.Exp(Mathf.Log(level + 1, 0.05f))* 10;
    }

    public float Fertility
    {
        get => mFertility;
        set
        {
            mFertility = value;
            fertilitySlider.value = mFertility >= 0 ? mFertility : 0;
            if (SpermSpawnController.Instance != null)
            {
                if (!SpermSpawnController.Instance.spawn)
                {
                    if (BossController.Instance.waitTimeToChangeMovement > 10)
                        BossController.Instance.waitTimeToChangeMovement = 35f - (0.2f * (100f - value));
                
                    if (BossController.Instance.waitTimeToSpawnSperm > 2)
                        BossController.Instance.waitTimeToSpawnSperm = 5f - (0.05f * (100f - value));
                }
            }
            
            
            if (mFertility <= 0)
            {
                //if(GetSpermCount() <= 0) GameController.Instance.SetGameState(GameState.Cleared);
            }
        }
    }


    public static OvumController Instance { private set; get; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        Fertility = 100;
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameController.Instance.gameState != GameState.Playing || isInmune) return;
        if (other.gameObject.CompareTag("meteor"))
        {
            isInmune = true;
            if(AudioManager.Instance) AudioManager.Instance.loseAudio.Play();
            GameController.Instance.SetGameState(GameController.Instance.CheckIfCanRetry() ? GameState.Retry : GameState.Ended);
        }
    }

    public void ChangeFertilityText(float value) => fertilityText.text = $"{Mathf.RoundToInt(value)}%";
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("boss"))
        {
            IncrementFertility();
        }

        if (other.gameObject.CompareTag("meteor"))
        {
            other.gameObject.transform.parent = null;
        }
    }

    public void IncrementFertility(int amount = 5)
    {
        
        if(AudioManager.Instance) AudioManager.Instance.fillAudio.Play();
        if(bool.Parse(PlayerPrefs.GetString("vibrate", "true"))) DataManagerUtility.Vibrate();
        if(Fertility < 94) {
            Fertility += amount;
        }
        else
        {
            Fertility += (100 - Fertility);
        }
    }
}