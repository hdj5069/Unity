using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Collider2D))]
public class Detector : MonoBehaviour
{
    public UnityEvent<Collider2D> OnTriggerEnter2DEvent;
    public UnityEvent<Collider2D> OnTriggerStay2DEvent;
    public UnityEvent<Collider2D> OnTriggerExit2DEvent;
    private void OnTriggerEnter2D(Collider2D other){
        OnTriggerEnter2DEvent?.Invoke(other);
    }
    private void OnTriggerStay2D(Collider2D other) {
        OnTriggerStay2DEvent?.Invoke(other);
    }
    private void OnTriggerExit2D(Collider2D other) {
        OnTriggerExit2DEvent?.Invoke(other);
    }
}
