using System;
using Arcade;
using Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpermController : MonoBehaviour
{
    public int id;
    public GameObject bubbleTrail;
    public Animator knockOutAnimation;
    public Animator knockOutAnimationWithoutStars;
    public Animator damageAnimation;
    public Rigidbody2D rb;
    public CapsuleCollider2D capsule;
    [Range(0f, 100f)] public float probabilityToSpawn;
    public SpermsType type;
    public bool isDestroyed, isEjected;
    public bool isInOvum;
    public bool tryToEnter;
    public AudioSource swimClip;
    public bool separated;
    public float Life
    {
        get => mLife; 
        set
        {
            mLife = value;
            lifeText.text = mLife >= 0 ? $"{Mathf.RoundToInt(mLife)}" : "0";
            lifeSlider.value = mLife;
        }
    }
    private float mLife;
    [Range(1f, 10000f)] public float totalLife;   
    public TextMeshProUGUI lifeText;
    public Slider lifeSlider;
    /*private void Start()
    {
        totalLife = mLife;
        lifeSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + Vector3.up);
        foreach (var mesh in GetComponentsInChildren<MeshRenderer>())
        {
            mesh.sortingOrder = -5;
        }
        Invoke(nameof(EnableSlider), 0.1f);
        switch (GameController.Instance.gameMode)
        {
            case GameMode.Arcade:
                return;
            case GameMode.Endless:
                rb.gravityScale = 0.05f + (SpermSpawnController.Instance.levelForInfiniteMode * 0.005f);
                return;
            case GameMode.Adventure when type == SpermsType.Fastersperm:
                rb.gravityScale = 0.2f;
                rb.drag = .5f;
                break;
            case GameMode.Adventure when type == SpermsType.Cuckold:
                rb.drag = 0f;
                rb.gravityScale = 0f;
                rb.velocity = Vector2.down * .5f;
                break;
            case GameMode.Adventure:
                rb.drag = 0f;
                rb.gravityScale = 0f;            
                rb.velocity = Vector2.down;
                break;
        }
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        if(swimClip) swimClip.pitch = Math.Abs(mVelocity.y);
        if(SpermSpawnController.Instance.infinite) return;
        lifeSlider.gameObject.SetActive(GameController.Instance.gameState == GameState.Playing);
        lifeText.gameObject.SetActive(GameController.Instance.gameState == GameState.Playing);
    }
    private float mAngle, mMoveSpeed;
    private Vector2 mMousePosition;
    private Vector3 mVelocity;
    private Vector3 mNewPos, mOldPos;

    private void CalculateVelocity()
    {
        mNewPos = transform.position;
        var media =  (mNewPos - mOldPos);
        mVelocity = media / Time.deltaTime;
        mOldPos = mNewPos;
        mNewPos = transform.position;
    }
    private void EnableSlider()
    {
        if(SpermSpawnController.Instance.infinite) return;
        lifeSlider.gameObject.SetActive(true);
    }

    private bool mFertilityIncremented;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("ovum")) return;
        if (!mFertilityIncremented)
        {
            mFertilityIncremented = true;
            if (GameController.Instance.gameState == GameState.Playing)
            {
                if(AudioManager.Instance) AudioManager.Instance.boingAudio.Play();
                if(bool.Parse(PlayerPrefs.GetString("vibrate", "true")) 
                   && GameController.Instance.gameState == GameState.Playing) DataManagerUtility.Vibrate();
            }
            if(OvumController.Instance.Fertility <= 98)
            {
                OvumController.Instance.Fertility += Mathf.RoundToInt((200f - PlayerPrefs.GetInt("level", 1)) / 78f) + 2; 
            }
            isInOvum = true;
            var canFertilize = CanFertilize();
            if (canFertilize)
            {
                Invoke(nameof(Fertilize), 1f);
                return;
            }

            tryToEnter = true;
            SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(this, 4f, "sperm controller: on collision enter 2d", false));
        }
    }


    private bool CanFertilize()
    {
        if (GameController.Instance.gameMode != GameMode.Adventure) return true;
        return Mathf.RoundToInt(Random.Range(1f, 11f - (OvumController.Instance.Fertility / 10f))) == 1;
    } 

    private void Fertilize()
    {
        if(OvumController.Instance.hasEnterSperm && GameController.Instance.gameMode == GameMode.Adventure)
        {
            //SpermSpawnController.Instance.StartCoroutine(SpermSpawnController.Instance.DestroySperm(this, 4f));
            return;
        }
        OvumController.Instance.hasEnterSperm = true;
        capsule.isTrigger = true;
        if(type != SpermsType.Fastersperm) rb.velocity = Vector2.down;
        if(AudioManager.Instance) AudioManager.Instance.whistleAudio.Play();
    }
    
    private void MoveTrailOutside()
    {
        if(GameController.Instance.gameState != GameState.Playing) return;
        bubbleTrail.transform.parent = null;
        Destroy(bubbleTrail, 1f);
    }

    private void OnBecameInvisible()
    {
        if(isInOvum) return;
        if(SpermSpawnController.Instance && SpermSpawnController.Instance.arcade) return;
        //Destroy(gameObject);
    }
    
    
    private void OnDestroy()
    {
        if(SpermGroupMovement.Instance && SpermSpawnController.Instance && SpermSpawnController.Instance.arcade)
        {
            SpermGroupMovement.Instance.UpdateSpermList(id, true);
        }
    }*/
}