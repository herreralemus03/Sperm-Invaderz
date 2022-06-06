using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Random = UnityEngine.Random;

namespace Invasion
{
    public class SpermSpawnController : MonoBehaviour
    {
        [Header("Level")] 
        public bool test, infinite, arcade;
        public int actualLevel, level;
        public GameObject knockOutAnimationWithoutStars, knockOutAnimation, splashParticles;
        public HealthController healthController;
        public bool spawn;
        [Range(0f, 1f)] public float offset;

        public int id;
        public List<SpermController> spermsActive;
        public static SpermSpawnController Instance { private set; get; }
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }
        private void Start()
        {
            if(!spawn) return;
            var repeatRate = DataManagerUtility.GetRepeatRateSpawn(PlayerPrefs.GetInt("level", 1));
            level = PlayerPrefs.GetInt("level", 1);
            spermsActive = new List<SpermController>();
            InvokeRepeating(nameof(InstantiateMeteor), 2f, repeatRate);
        }
        private void OnDestroy()
        {
            Instance = null;
        }
    
        private void InstantiateMeteor()
        {
            if(!OvumController.Instance) return;
            if(OvumController.Instance.Fertility <= 0f) return;
            if(GameController.Instance.gameState != GameState.Playing) return;
            StartCoroutine(InstantiateSpermWithRandomLife(level));
        }

        public List<SpermController> spermsToInstance;
        private IEnumerator InstantiateSpermWithRandomLife(int level)
        {
            var sperm = GetSpermType(level);
            var xPosition = Random.Range(-DataManagerUtility.GetScreenWidth() * .7f, DataManagerUtility.GetScreenWidth() * .7f);
            var spermInstantiated = Instantiate(sperm, new Vector3(xPosition, DataManagerUtility.GetScreenHeight()), Quaternion.identity);
            CanDropItem(spermInstantiated);
            var lifeOffset1 = Mathf.RoundToInt(spermInstantiated.totalLife * 1.35f);
            var lifeOffset2 = Mathf.RoundToInt(spermInstantiated.totalLife * 0.95f);
            spermInstantiated.Life = Random.Range(lifeOffset1, lifeOffset2);
            spermInstantiated.lifeSlider = Instantiate(healthController, spermInstantiated.transform.position, Quaternion.identity, GameController.Instance.canvasParent.transform);
            spermInstantiated.lifeSlider.slider.maxValue = spermInstantiated.Life;
            spermInstantiated.lifeSlider.slider.value = spermInstantiated.Life;
            sperms++;
            spermInstantiated.id = id++;
            spermsActive.Add(spermInstantiated);
            yield return null;
        }
        public IEnumerator InstantiateSpermWithRandomLife(SpermController spermT, Vector3 position)
        {
            var sperm = spermT;
            var spermInstantiated = Instantiate(sperm, position, Quaternion.identity);
            var lifeOffset1 = Mathf.RoundToInt(spermInstantiated.totalLife * 1.35f);
            var lifeOffset2 = Mathf.RoundToInt(spermInstantiated.totalLife * 0.95f);
            CanDropItem(spermInstantiated);
            spermInstantiated.Life = Random.Range(lifeOffset1, lifeOffset2);
            spermInstantiated.lifeSlider = Instantiate(healthController, position, Quaternion.identity, GameController.Instance.canvasParent.transform);
            spermInstantiated.lifeSlider.slider.maxValue = spermInstantiated.Life;
            spermInstantiated.lifeSlider.slider.value = spermInstantiated.Life;
            sperms++;
            spermInstantiated.id = id++;
            spermsActive.Add(spermInstantiated);
            yield return null;
        }

        private void CanDropItem(SpermController spermController)
        {
            if (spermController.type == SpermsType.Beast || 
                spermController.type == SpermsType.FasterSperm || 
                spermController.type == SpermsType.Ninja || 
                spermController.type == SpermsType.Protected)
            {
                spermController.special = Random.Range(1, 8) == 5;
            }
        }
        public IEnumerator InstantiateSpermWithRandomLife(Vector3 position, int level)
        {
            var sperm = GetSpermType(level);
            var spermInstantiated = Instantiate(sperm, position, Quaternion.identity);
            var lifeOffset1 = Mathf.RoundToInt(spermInstantiated.totalLife * 1.35f);
            var lifeOffset2 = Mathf.RoundToInt(spermInstantiated.totalLife * 0.95f);
            CanDropItem(spermInstantiated);
            spermInstantiated.Life = Random.Range(lifeOffset1, lifeOffset2);
            spermInstantiated.lifeSlider = Instantiate(healthController, position, Quaternion.identity, GameController.Instance.canvasParent.transform);
            spermInstantiated.lifeSlider.slider.maxValue = spermInstantiated.Life;
            spermInstantiated.lifeSlider.slider.value = spermInstantiated.Life;
            sperms++;
            spermInstantiated.id = id++;
            spermsActive.Add(spermInstantiated);
            yield return null;
        }
        private SpermController GetSpermType(int level)
        {
            if(level > 40)
            {
                return GetSpermRandomly(level, SpermsType.Protected2, SpermsType.Ninja, SpermsType.FasterSperm, SpermsType.Beast);
            } 
        
            if (level > 30)
            {
                return GetSpermRandomly(level, SpermsType.Ninja, SpermsType.FasterSperm, SpermsType.Beast);
            } 
        
            if (level > 20)
            {
                return GetSpermRandomly(level, SpermsType.Protected, SpermsType.FasterSperm);
            } 
        
            return level > 10 ? GetSpermRandomly(level, SpermsType.Normal, SpermsType.Protected) : spermsToInstance.Find(sperm => sperm.type == SpermsType.Normal);
        }
        private SpermController GetSpermRandomly(
            int level, 
            SpermsType firstType = SpermsType.Normal, 
            SpermsType secondType = SpermsType.None, 
            SpermsType thirdType = SpermsType.None, 
            SpermsType fourthType = SpermsType.None)
        {
            var probabilityRange = Random.Range(1, 100);
            if (probabilityRange < Mathf.RoundToInt(50 + GetProbabilityForLevel(20, level, 50)))
            {
                return spermsToInstance.Find(sperm => sperm.type == firstType);
            }
            probabilityRange = Random.Range(1, 100);
            if(secondType == SpermsType.None)
            {
                return spermsToInstance.Find(sperm => sperm.type == firstType);
            }

            if (thirdType == SpermsType.None)
            {
                return spermsToInstance.Find(sperm => sperm.type == secondType);
            }

            if (fourthType == SpermsType.None)
            {
                return probabilityRange < 75 ? spermsToInstance.Find(sperm => sperm.type == secondType) : spermsToInstance.Find(sperm => sperm.type == thirdType);
            }

            if (probabilityRange < 50)
            {
                return spermsToInstance.Find(sperm => sperm.type == secondType);
            } 
        
            return probabilityRange < 80 ? spermsToInstance.Find(sperm => sperm.type == thirdType) : spermsToInstance.Find(sperm => sperm.type == fourthType);
        }
        public IEnumerator DestroySperm(SpermController objectToDestroy, float time, String location, bool destroyedByPlayer = false, bool spawnOther = false)
        {
            if (objectToDestroy && objectToDestroy.gameObject == null) yield break;
        
            if (objectToDestroy && (spawnOther || objectToDestroy.type == SpermsType.Beast))
            {
                DropItem(objectToDestroy.transform.position, objectToDestroy);
            }
        
            sperms--;
            spermsActive.RemoveAll(sperm=> objectToDestroy.id == sperm.id);
            yield return new WaitForSeconds(time);
            if (objectToDestroy) Instantiate(spawnOther ? knockOutAnimationWithoutStars : knockOutAnimation, objectToDestroy.gameObject.transform.position, Quaternion.identity);
            if(!spawnOther && AudioManager.Instance) AudioManager.Instance.explosionAudio.Play();
            if(spawnOther && AudioManager.Instance) AudioManager.Instance.spawnAudio.Play();
            if(!spawnOther && destroyedByPlayer && spawn && OvumController.Instance.Fertility > 0) OvumController.Instance.Fertility -= OvumController.Instance.damageAmount;
            if (objectToDestroy)
            {
                objectToDestroy.lifeSlider.DestroySlider();
                Destroy(objectToDestroy.gameObject);
            }
            CheckIfClearGame();
        }

        public ItemUpgradeController extraSpeedItem, extraForceItem, extraShootItem, intangibleShoot, everything;
        private void DropItem(Vector3 position, SpermController spermController)
        {
            if(!spermController.special) return;
            switch (spermController.type)
            {
                case SpermsType.Beast:
                    Instantiate(extraForceItem, position, Quaternion.identity);
                    return;
                case SpermsType.FasterSperm:
                    Instantiate(extraSpeedItem, position, Quaternion.identity);
                    return;
                case SpermsType.Ninja:
                    Instantiate(extraShootItem, position, Quaternion.identity);
                    break;
                case SpermsType.Protected:
                    Instantiate(intangibleShoot, position, Quaternion.identity);
                    break;
                case SpermsType.Protected2:
                    Instantiate(intangibleShoot, position, Quaternion.identity);
                    break;
            }
        }
    
        public int sperms = 0;
    
        public int GetSpermCount()
        {
            var spermsLength = FindObjectsOfType<SpermController>();
            return spermsLength.Length;
        }
        public void CheckIfClearGame()
        {
            if (GameController.Instance.gameState == GameState.Cleared || GameController.Instance.gameState == GameState.Ended) return;
            if (OvumController.Instance.Fertility <= 0)
            {
                if (spermsActive.Count <= 0)
                {
                    GameController.Instance.SetGameState(GameState.Cleared);
                }
            }
        }
        public void PlaySplashParticles(Vector3 position)
        {
            Instantiate(splashParticles, position, Quaternion.identity);
        }
        private float GetProbabilityForLevel(int range, int level, int probabilityRange) => probabilityRange/Convert.ToSingle(level - range);
    }

}