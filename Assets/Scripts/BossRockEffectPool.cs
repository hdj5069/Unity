using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRockEffectPool : MonoBehaviour
{
    public static BossRockEffectPool Instance { get; private set; }
    public GameObject effectPrefab;
    private Queue<GameObject> effectsPool = new Queue<GameObject>();
    public int poolSize = 20;

    private void Awake() {
        Instance = this;
        InitializePool();
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

    public void ReturnEffect(GameObject effect) {
        effect.SetActive(false);
        effectsPool.Enqueue(effect);
    }
}