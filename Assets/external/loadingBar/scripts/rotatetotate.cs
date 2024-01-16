using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class rotatetotate : MonoBehaviour {

    private RectTransform rectComponent;

    public float rotateSpeed = 200f;

    void Start () {
        rectComponent = GetComponent<RectTransform>();
    }
	
	void Update () {

        float currentSpeed = rotateSpeed * Time.deltaTime;
        rectComponent.Rotate(0f, 0f, currentSpeed);
    }
}