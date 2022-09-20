using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextShakeScript : MonoBehaviour
{
    private float amnt;
    private float textPosY;
    private float textPosX;
    private bool shake = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (shake)
        {
            float xNew = Mathf.Lerp(transform.position.x, textPosY, Time.deltaTime * 10);
            transform.position = new Vector3(xNew, transform.position.y, transform.position.z);
        }
    }


    public void SetCoordinateShake(float x, float y)
    {
        textPosX = x;
        textPosY = y;
    }

    public void Shake(float amount, float time)
    {
        shake = true;
        amnt = amount;
        InvokeRepeating("BeginShake", 0, 0.01f);
        Invoke("StopShake", time);
    }

    void BeginShake()
    {
        if (amnt > 0)
        {
            Vector3 textPos = transform.position;

            float shakeAmtX = Random.value * amnt * .2f - amnt;
            float shakeAmtY = Random.value * amnt * .2f - amnt;

            textPos.x += shakeAmtX;
            textPos.y += shakeAmtY;

            transform.position = textPos;
        }
    }

    void StopShake()
    {
        shake = false;
        CancelInvoke("BeginShake");
        float xNew = Mathf.Lerp(transform.position.x, textPosX, Time.deltaTime * 10);
        transform.position = new Vector3(xNew, textPosX, transform.position.z);
    }
}
