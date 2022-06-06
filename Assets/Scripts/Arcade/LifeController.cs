using System;
using Common;
using TMPro;
using UnityEngine;

namespace Arcade
{
    public class LifeController : MonoBehaviour
    {
        public Animator animator;
        public TextMeshProUGUI amount;
        private static readonly int Increase = Animator.StringToHash("Increase");
        public static LifeController Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void Start()
        {
            amount.text = $"{ScoreArcadeMode.Instance.playerLife}";
        }
        
        public void IncrementLife(Vector3 position, int amount)
        {
            var lifeObject = Instantiate(ScoreController.Instance.scoreText, position, Quaternion.identity);
            lifeObject.movePoint = transform.position;
            lifeObject.textMesh.text = $"+{amount}";
            if(ScoreArcadeMode.Instance.playerLife < 3) ScoreArcadeMode.Instance.playerLife++;
            Invoke(nameof(AnimateLabel), 1f);
        }

        public void AnimateLabel()
        {
            amount.text = $"{ScoreArcadeMode.Instance.playerLife}";
            animator.SetTrigger(Increase);
            if (AudioManager.Instance) AudioManager.Instance.lifeUp.Play();
        }

        private void OnDestroy()
        {
            Instance = null;
        }
    }
}