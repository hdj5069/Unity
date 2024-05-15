using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Slider Healthslider;
    [SerializeField] private Slider Shiledslider;
    public void UpdateHealthBar(float curHelath,float maxHealth){
        Healthslider.value = curHelath / maxHealth;
    }
    public void UpdateShieldBar(float curShiled,float maxShiled){
        if(curShiled <= 0){
            Shiledslider.gameObject.SetActive(false);
        }
        Shiledslider.value = curShiled / maxShiled;
    }
    void Update()
    {
    }
}
