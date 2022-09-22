using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextShakeScript : MonoBehaviour
{
    private float amnt;
    private float textPosY;
    private float textPosX;
    private bool stopShake = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        if (stopShake)
        {
            float xNew = Mathf.Lerp(transform.position.x, textPosX, Time.deltaTime * 30);
            float yNew = Mathf.Lerp(transform.position.y, textPosY, Time.deltaTime * 30);
            GetComponent<RectTransform>().transform.position = new Vector3(xNew, yNew, 0);
        }
    }


    public void SetCoordinateShake(float x, float y)
    {
        textPosX = x;
        textPosY = y;
    }

    public void Shake(float amount, float time)
    {
        stopShake = false;
        amnt = amount;
        InvokeRepeating("BeginShake", 0, 0.01f);
        Invoke("StopShake", time);
    }

    void BeginShake()
    {
        if (amnt > 0)
        {


            Vector3 textPos = GetComponent<RectTransform>().transform.position;

            float shakeAmtX = Random.value * amnt * 2f - amnt;
            float shakeAmtY = Random.value * amnt * 2f - amnt;

            textPos.x += shakeAmtX;
            textPos.y += shakeAmtY;

            GetComponent<RectTransform>().transform.position = textPos;
        }
    }

    void StopShake()
    {
        stopShake = true;
        CancelInvoke("BeginShake");
        //transform.position = new Vector3(textPosX, textPosY, 0);
        //float xNew = Mathf.Lerp(GetComponent<RectTransform>().transform.position.x, textPosY, Time.deltaTime * 10);
        //GetComponent<RectTransform>().transform.position = new Vector3(xNew, textPosX, textPosY);
    }
}
