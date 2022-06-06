using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Arcade
{
    public class SpermGroupMovement : MonoBehaviour
    {
        public int spermsAmount = 35;
        public float speed, offset, distance;
        public bool separateIsExecuted;
        public List<SpermController> sperms;
        public GameObject pill;
        public static SpermGroupMovement Instance { private set; get; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }
        private void Start()
        {
            AddAllSpermsToList();
            var repeatRate = DataManagerUtility.GetDec(ScoreArcadeMode.Instance.level) * 0.25f;
            speed -= repeatRate;
            InvokeRepeating(nameof(InstantiatePill), 3f, speed * 5f);
            MoveRight();
            StartCoroutine(StartSeparateFromGroup());
        }
        private void Update()
        {
            var position = transform.position;
            distance = Vector3.Distance(position, new Vector3(0f, position.y));
            if (!(distance <= 0.1f)) return;
            if (!separateIsExecuted) StartCoroutine(SeparateFromGroup());
        }
        public void UpdateSpermList(int id, bool reduce, bool incrementLevelSpeed = true)
        {
            if (incrementLevelSpeed) speed *= 0.94f;
            sperms.RemoveAll(sperm => sperm.id == id);
            if (ScoreArcadeMode.Instance.playerLife <= 0) return;
            if (sperms.Count > 0) return;
            if(!GameController.Instance) return;
            if(GameController.Instance.gameState != GameState.Ended)
            {
                GameController.Instance.SetGameState(GameState.Cleared);
            }
        }
        private void AddAllSpermsToList()
        {
            var spermFound = GetComponentsInChildren<SpermController>();
            for (var i = 0; i <= spermFound.Length - 1; i++)
            {
                spermFound[i].id = i;
                sperms.Add(spermFound[i]);
            }
        }
        private void InstantiatePill()
        {
            if (GameController.Instance.gameState != GameState.Playing) return;
            Instantiate(pill, new Vector3(-DataManagerUtility.GetScreenWidth(), 3.5f), Quaternion.identity);
        }
        public void MoveLeft()
        {
            offset += 0.05f;
            LeanTween.moveLocal(gameObject, new Vector3(-DataManagerUtility.GetScreenWidth() *.25f, transform.position.y - offset), speed).setOnComplete(MoveRight);
        }
        public void MoveRight()
        {
            LeanTween.moveLocal(gameObject, new Vector3(DataManagerUtility.GetScreenWidth() *.25f, transform.position.y - offset), speed).setOnComplete(MoveLeft);
        }
        private void OnDestroy()
        {
            Instance = null;
        }
        private IEnumerator SeparateFromGroup()
        {
            separateIsExecuted = true;
            if (sperms.Count <= 0) yield break;
            var spermsToSeparate = Mathf.RoundToInt(ScoreArcadeMode.Instance.level / 10f);
            for (var i = 0; i <= spermsToSeparate; i++)
            {
                yield return new WaitForSeconds(.2f);
                if (sperms.Count <= 0) break;
                SeparateSperm();
            }
            yield return new WaitForSeconds(10f);
            separateIsExecuted = false;
        }

        private void SeparateSperm()
        {
            var sperm = sperms[Random.Range(0, sperms.Count - 1)];
            sperm.transform.parent = null;
            sperm.rb.gravityScale = 0.05f;
            sperm.separated = true;
        }
        private IEnumerator StartSeparateFromGroup()
        {
            separateIsExecuted = true;
            yield return new WaitForSeconds(10f);
            separateIsExecuted = false;
        }
    }
}
