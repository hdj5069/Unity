using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRockEffectPool : MonoBehaviour
{
    public static BossRockEffectPool Instance { get; private set; }
    public GameObject effectPrefab;
    public GameObject flooreffect;
    public GameObject bossParent;
    private Queue<GameObject> effectsPool = new Queue<GameObject>();
    private Queue<GameObject> floorsPool = new Queue<GameObject>();
    public int poolSize = 20;

    private void Awake() {
        Instance = this;
        InitializePool();
        initFloorPool();
    }

    private void InitializePool() {
        for (int i = 0; i < poolSize; i++) {
            GameObject effect = Instantiate(effectPrefab);
            effect.transform.SetParent(transform);
            effect.SetActive(false);
            effectsPool.Enqueue(effect);
        }
    }
    public GameObject GetEffect() {
        if (effectsPool.Count > 0) {
            GameObject effect = effectsPool.Dequeue();
            effect.SetActive(true);
            return effect;
        } else {
            GameObject effect = Instantiate(effectPrefab);
            effect.SetActive(true);
            return effect;
        }
    }
    private void initFloorPool() { 
        for (int i = 0; i < poolSize; i++) {
            GameObject effect = Instantiate(flooreffect);
            effect.transform.SetParent(bossParent.transform);
            effect.transform.localScale = new Vector3(1,1,1);
            effect.SetActive(false);
            floorsPool.Enqueue(effect);
        }
    }
    public GameObject GetfloorEffect() {
        if (floorsPool.Count > 0) {
            GameObject effect = floorsPool.Dequeue();
            effect.SetActive(true);
            return effect;
        } else {
            GameObject effect = Instantiate(flooreffect);
            effect.SetActive(true);
            return effect;
        }
    }
    public void ReturnfloorEffect(GameObject effect) {
        effect.SetActive(false);
        floorsPool.Enqueue(effect);
    }
    public void ReturnEffect(GameObject effect) {
        effect.SetActive(false);
        effectsPool.Enqueue(effect);
    }
}