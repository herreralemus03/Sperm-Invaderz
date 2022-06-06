using System;
using UnityEngine;
using ScoreController = Arcade.ScoreController;

namespace Common
{
    public class ScoreText : MonoBehaviour
    {
        public TextMesh textMesh;
        public Vector3 movePoint;
        private void Start()
        {
            LeanTween.move(gameObject, Camera.main.ScreenToWorldPoint(movePoint), 1f).setOnComplete(DestroyText);
        }

        private void DestroyText()
        {
            Destroy(gameObject);
        }
        private bool mDestroyCalled;
    }
}
