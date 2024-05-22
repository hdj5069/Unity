using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossEffectPool : MonoBehaviour
{
    public static BossEffectPool Instance { get; private set; }
    public GameObject effectPrefab;
    public GameObject flooreffect;
    public GameObject SadPattern2Effect;

    public GameObject bossParent;
    private Queue<GameObject> effectsPool = new Queue<GameObject>();
    private Queue<GameObject> SadPattern2Pool = new Queue<GameObject>();
    GameObject sadeffect;
    GameObject sad2effect;
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
    public void ReturnEffect(GameObject effect) {
        effect.SetActive(false);
        effectsPool.Enqueue(effect);
    }
    private void initFloorPool() { 
        for (int i = 0; i < 1; i++) {
            sadeffect = Instantiate(flooreffect);
            sadeffect.transform.SetParent(bossParent.transform);
            sadeffect.transform.localScale = new Vector3(1,1,1);
            sadeffect.SetActive(false);
            floorsPool.Enqueue(sadeffect);
        }
    }
    public GameObject GetfloorEffect() {
        if (floorsPool.Count > 0) {
            sadeffect = floorsPool.Dequeue();
            sadeffect.SetActive(true);
            return sadeffect;
        } else {
            sadeffect = Instantiate(flooreffect);
            sadeffect.SetActive(true);
            return sadeffect;
        }
    }
    public void ReturnEffectall(){
        sadeffect.SetActive(false);
        floorsPool.Enqueue(sadeffect);
    }
    
    private void initsadpattern2() { 
        for (int i = 0; i < 1; i++) {
            sad2effect = Instantiate(SadPattern2Effect);
            sad2effect.transform.SetParent(bossParent.transform);
            sad2effect.transform.localScale = new Vector3(1,1,1);
            sad2effect.SetActive(false);
            SadPattern2Pool.Enqueue(sad2effect);
        }
    }
    public GameObject GetSadPattern2() {
        if (SadPattern2Pool.Count > 0) {
            sad2effect = SadPattern2Pool.Dequeue();
            sad2effect.SetActive(true);
            return sad2effect;
        } else {
            sad2effect = Instantiate(SadPattern2Effect);
            sad2effect.SetActive(true);
            return sad2effect;
        }
    }
    public void ReturnSadPattern2(){
        sad2effect.SetActive(false);
        SadPattern2Pool.Enqueue(sad2effect);
    }

}