/*using System;
using Arcade;
using Common;
using TMPro;
using UnityEngine;
using PlayerController = Invasion.PlayerController;

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { private set; get; }
    public ScoreText scoreText, scoreText2;
    public TextMeshProUGUI textMesh;
    public long scoreAmount;
    public Animator animator;
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if(!SpermSpawnController.Instance.infinite) textMesh.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
        if (SpermSpawnController.Instance.arcade) textMesh.text = DataManagerUtility.FormatAmount(ScoreArcadeMode.Instance.score);
    }

    public long tempCredits;
    private long ScoreAmount
    {
        set
        {
            if (GameController.Instance.gameMode == GameMode.Adventure)
            {                
                if(GameController.Instance.gameState == GameState.Playing) tempCredits += value;
                DataManagerUtility.AddCreditsAmount(Convert.ToInt64(value));
                Invoke(nameof(IncreaseScoreAmountOnUi), 0.4f);
                return;
            }
            
            if (SpermSpawnController.Instance.arcade)
            {
                ScoreArcadeMode.Instance.score += value;
                Invoke(nameof(IncreaseScoreAmountOnUiArcadeMode), 0.4f);
                return;
            }
            scoreAmount += value;
            Invoke(nameof(IncreaseScoreAmountOnUiInfiniteMode), 0.4f);
        }
    }

    private void IncreaseScoreAmountOnUi()
    {
        if(AudioManager.Instance) AudioManager.Instance.creditsIncreaseAudio.Play();
        animator.SetTrigger("Increase");
        textMesh.text = DataManagerUtility.GetCreditsAmountWithStringFormat();
    }

    private void IncreaseScoreAmountOnUiInfiniteMode()
    {
        if(AudioManager.Instance) AudioManager.Instance.creditsIncreaseAudio.Play();
        animator.SetTrigger("Increase");
        textMesh.text = DataManagerUtility.FormatAmount(scoreAmount);
    }
    private void IncreaseScoreAmountOnUiArcadeMode()
    {
        if(AudioManager.Instance) AudioManager.Instance.creditsIncreaseAudio.Play();
        animator.SetTrigger("Increase");
        textMesh.text = DataManagerUtility.FormatAmount(ScoreArcadeMode.Instance.score);
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    public void InstantiateScoreText(Vector3 scorePosition, int amount)
    {
        if(SpermSpawnController.Instance.infinite) return;
        var scoreStr = Instantiate(scoreText, scorePosition, Quaternion.identity);
        scoreStr.textMesh.text = $"+{amount}";
        ScoreAmount = amount;
    }
    
    public void InstantiateScore(Vector3 scorePosition, int amount)
    {
        var scoreStr = Instantiate(scoreText2, scorePosition, Quaternion.identity);
        var multiplier = PlayerController.Instance.spermsDestroyedConsecutive;
        if (GameController.Instance.gameMode == GameMode.Arcade)
        {
            if(PlayerController.Instance.spermsDestroyedConsecutive > 1)
            {
                amount *= multiplier;
                InstantiateScoreTextBonus(multiplier, scorePosition);
            }
        }

        ScoreAmount = amount;
        scoreStr.textMesh.text = $"+{amount}";
    }

    public TextMeshProUGUI bonusTextMesh;
    public Canvas parent;
    private void InstantiateScoreTextBonus(int multiplier, Vector3 position)
    {
        switch (multiplier)
        {
            case 2:
                AudioManager.Instance.scoreAudio.pitch = 1f;
                bonusTextMesh.text = "GOOD";
                break;
            case 3:
                AudioManager.Instance.scoreAudio.pitch = 0.9f;
                bonusTextMesh.text = "NICE";
                break;
            case 4:
                AudioManager.Instance.scoreAudio.pitch = 0.8f;
                bonusTextMesh.text = "GREAT";
                break;
            case 5:
                AudioManager.Instance.scoreAudio.pitch = 0.7f;
                bonusTextMesh.text = "AWESOME";
                break;
            case 6:
                AudioManager.Instance.scoreAudio.pitch = 0.6f;
                bonusTextMesh.text = "AMAZING";
                break;
            case 7:
                AudioManager.Instance.scoreAudio.pitch = 0.5f;
                bonusTextMesh.text = "WONDERFUL";
                break;
            case 8:
                AudioManager.Instance.scoreAudio.pitch = 0.4f;
                bonusTextMesh.text = "FANTASTIC";
                break;
            default:
                AudioManager.Instance.scoreAudio.pitch = 0.4f;
                bonusTextMesh.text = "FANTASTIC";
                break;
        }

        AudioManager.Instance.scoreAudio.Play();
        
        //var viewportPosition = Camera.main.WorldToViewportPoint(position);
        //var newPos = parent.ViewportToCanvasPosition(viewportPosition);
        Instantiate(bonusTextMesh, position, Quaternion.identity, parent.transform);
        //WorldToScreenCanvas(position, canvasObject);
    }
    private void WorldToScreenCanvas(Vector3 position, TextMeshProUGUI canvasObject)
    {
 
        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
 
        var viewPortPosition=Camera.main.WorldToViewportPoint(position);
        var worldObjectScreenPosition=new Vector2(
            ((viewPortPosition.x*canvasObject.rectTransform.sizeDelta.x)-(canvasObject.rectTransform.sizeDelta.x*0.5f)),
            ((viewPortPosition.y*canvasObject.rectTransform.sizeDelta.y)-(canvasObject.rectTransform.sizeDelta.y*0.5f)));
 
        //now you can set the position of the ui element
        canvasObject.rectTransform.anchoredPosition=worldObjectScreenPosition;
    }
}*/