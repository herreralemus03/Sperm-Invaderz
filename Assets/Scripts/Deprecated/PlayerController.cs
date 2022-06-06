/*using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Invasion;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private static readonly int Shoot1 = Animator.StringToHash("shoot");
    private static readonly int Show = Animator.StringToHash("show");
    private static readonly int Hide = Animator.StringToHash("hide");    
    public Color 
        normalColor, 
        speedColor, 
        forceColor, 
        duplicateColor, 
        phantomColor;
    public static PlayerController Instance { private set; get; }
    [Header("Spawn Object")] 
    public ShootController 
        laser;
    [Header("Other")] 
    public Animator 
        animator, 
        shootAnimation;
    private float
        mAngle,
        mMoveSpeed;
    private Vector3 
        mMousePosition, 
        mVelocity, 
        mNewPos, 
        mOldPos;
    public GameObject 
        trigger0, 
        trigger1, 
        trigger2;
    public Material playerColor;
    public GameObject model3D;
    public Sprite 
        shuriken, 
        pill, 
        multishot, 
        power, 
        speed, 
        phantom;
    private PowerType 
        mPowerType;
    public bool
        powerIsOn,
        isHitByBoss,
        canDoExtraScore;
    public int 
        powerDuration, 
        spermsDestroyedConsecutive;
    public PowerType PowerType
    {
        get => mPowerType;
        private set
        {
            switch(value)
            {
                case PowerType.None:
                    ChangePlayerColor(normalColor);
                    ChangeShootSpeedRate(1f);
                    DisableSecondaryShoot();
                    break; 
                case PowerType.Duplicate:
                    PowerUp.Instance.icon.sprite = multishot;
                    ChangePlayerColor(duplicateColor);
                    ChangeShootSpeedRate(1f);
                    EnableSecondaryShoot();
                    break; 
                case PowerType.Speed:
                    PowerUp.Instance.icon.sprite = speed;
                    ChangePlayerColor(speedColor);
                    ChangeShootSpeedRate(.5f);
                    DisableSecondaryShoot();
                    break;
                case PowerType.Force:
                    PowerUp.Instance.icon.sprite = power;
                    ChangePlayerColor(forceColor);
                    ChangeShootSpeedRate(1f);
                    DisableSecondaryShoot();
                    break; 
                case PowerType.Everything:
                    ChangePlayerColor(forceColor);
                    ChangeShootSpeedRate(.5f);
                    EnableSecondaryShoot(.5f);
                    break; 
                case PowerType.Intangible:
                    PowerUp.Instance.icon.sprite = phantom;
                    ChangePlayerColor(phantomColor);
                    ChangeShootSpeedRate(1f);
                    DisableSecondaryShoot();
                    break;
            }
            mPowerType = value;
        }
    }

    public bool CanDoExtraScore
    {
        get => canDoExtraScore;
        set
        {
            if (value)
            {
                if(spermsDestroyedConsecutive <= 8) spermsDestroyedConsecutive++;
            }
            else
            {
                spermsDestroyedConsecutive = 0;
            }

            canDoExtraScore = value;
        }
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        Rotate();
    }
    private void Rotate(){
        mNewPos = transform.position;
        var media =  (mNewPos - mOldPos);
        mVelocity = media /Time.deltaTime;
        mOldPos = mNewPos;
        mNewPos = transform.position;
        model3D.transform.Rotate(Vector3.up, mVelocity.x * 5f);
        PlaySwimSound();
    }
    private void PlaySwimSound()
    {
        if(mVelocity.x <= 0 || !AudioManager.Instance) return;
        
        if (mVelocity.x > 90)
        {
            if(AudioManager.Instance.swimFast1Audio.isPlaying) return;
            AudioManager.Instance.swimFast1Audio.Play();
        } else if (mVelocity.x > 80)
        {
            if(AudioManager.Instance.swimFast2Audio.isPlaying) return;
            AudioManager.Instance.swimFast2Audio.Play();
        } else if (mVelocity.x > 60)
        {
            if(AudioManager.Instance.swimMedium1Audio.isPlaying) return;
            AudioManager.Instance.swimMedium1Audio.Play();
        } else if (mVelocity.x > 40)
        {
            if(AudioManager.Instance.swimMedium2Audio.isPlaying) return;
            AudioManager.Instance.swimMedium2Audio.Play();
        } else if (mVelocity.x > 20)
        {
            if(AudioManager.Instance.swimSoft1Audio.isPlaying) return;
            AudioManager.Instance.swimSoft1Audio.Play();
        } else
        {
            if(AudioManager.Instance.swimSoft2Audio.isPlaying) return;
            AudioManager.Instance.swimSoft2Audio.Play();
        }
    }
    private void FixedUpdate()
    {
        mMousePosition = Input.mousePosition;
        mMousePosition = Camera.main.ScreenToWorldPoint(mMousePosition);
        if(mMousePosition.y > SpermSpawnController.Instance.cameraHeight/2) return;
        if(Input.GetMouseButton(0)) transform.position = Vector2.Lerp(transform.position, new Vector3(mMousePosition.x, transform.position.y), 0.2f);
    }
    public List<GameObject> shoots = new List<GameObject>();
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    public void Shoot()
    {
        if(GameController.Instance.gameState != GameState.Playing) return;
        if(!Input.GetMouseButton(0)) return;
        var shoot = Instantiate(laser, trigger0.transform.position, Quaternion.identity);
        shoot.sprite.sprite = pill;
        shootAnimation.SetTrigger(Shoot1);
        if(AudioManager.Instance) AudioManager.Instance.shootAudio.Play();
    }
    private void SecondaryShoot()
    {
        if(GameController.Instance.gameState != GameState.Playing) return;
        if(!Input.GetMouseButton(0)) return;
        var shoot1 = Instantiate(laser, trigger1.transform.position, Quaternion.identity);
        shoot1.sprite.sprite = shuriken;
        var shoot2 = Instantiate(laser, trigger2.transform.position, Quaternion.identity);
        shoot2.sprite.sprite = shuriken;
        if(AudioManager.Instance) AudioManager.Instance.shootAudio.Play();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("meteor") || other.gameObject.CompareTag("shoot") || other.gameObject.CompareTag("item")) return;
        if (GameController.Instance.gameState != GameState.Playing) return;
        if (AudioManager.Instance) AudioManager.Instance.playerCollisionAudio.Play();
        if (AudioManager.Instance) AudioManager.Instance.loseAudio.Play();
        if(isHitByBoss) return;
        if (other.gameObject.CompareTag("boss")) isHitByBoss = true;
        GameController.Instance.SetGameState(GameController.Instance.CheckIfCanRetry() ? GameState.Retry : GameState.Ended);
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    private void DisableSecondaryShoot()
    {
        CancelInvoke(nameof(SecondaryShoot));
    }
    private void EnableSecondaryShoot(float modifier = 1f)
    {
        InvokeRepeating(nameof(SecondaryShoot), 0f, DataManagerUtility.GetShotSpeed(PlayerPrefs.GetInt("speed", 1)) * modifier);
    }
    private void ChangeShootSpeedRate(float modifier)
    { 
        CancelInvoke(nameof(Shoot));
        InvokeRepeating(nameof(Shoot), 0f, DataManagerUtility.GetShotSpeed(PlayerPrefs.GetInt("speed", 1))*modifier);
    }
    private IEnumerator EnablePower()
    {
        PowerUp.Instance.animator.SetTrigger(Show);
        powerIsOn = true;
        while (powerDuration > 0)
        {
            yield return new WaitForSeconds(1f);
            powerDuration--;
        }
        PowerUp.Instance.animator.SetTrigger(Hide);
        PowerType = PowerType.None;
        powerIsOn = false;
    }
    public void GivePower(PowerType powerType)
    {
        if(PowerType != powerType) PowerType = powerType;
        AudioManager.Instance.upgradeAudio.Play();
        powerDuration = 10;
        if (!powerIsOn)
        {
            StartCoroutine(EnablePower());
        }
    }
    public void ChangePlayerColor(Color color)
    {
        LeanTween.value(gameObject, playerColor.color, color, .5f).setOnUpdateColor((colorUpdate) =>
        {
            playerColor.SetColor(BaseColor, colorUpdate);
            playerColor.SetColor(EmissionColor, colorUpdate);
        });
    }
}*/