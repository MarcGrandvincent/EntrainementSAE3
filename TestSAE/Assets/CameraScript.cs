using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    private float amnt;
    private float camPosY;
    private float camPosX;
    private bool stopShake = false;
    private bool stopDeplacement;

    // Start is called before the first frame update
    void Start()
    {
        camPosX = 0;
        camPosY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (stopShake && !stopDeplacement)
        {
            float xNew = Mathf.Lerp(transform.position.x, camPosY, Time.deltaTime * 10);
            transform.position = new Vector3(xNew, transform.position.y, -5);
            if (transform.position.x >= camPosX && transform.position.y >= camPosY)
            {
                transform.position = new Vector2(camPosX, camPosY);
                stopDeplacement = true;
            }
        }
    }



    public void SetCoordinates(Vector3 position)
    {
        camPosX = position.x;
        camPosY = position.y;
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
            Vector3 camPos = transform.position;

            float shakeAmtX = Random.value * amnt * 2 - amnt;
            float shakeAmtY = Random.value * amnt * 2 - amnt;

            camPos.x += shakeAmtX;
            camPos.y += shakeAmtY;

            transform.position = camPos;
        }
    }

    void StopShake()
    {
        stopDeplacement = false;
        CancelInvoke("BeginShake");
        float xNew = Mathf.Lerp(transform.position.x, camPosX, Time.deltaTime * 10);
        transform.position = new Vector3(xNew, camPosX, -5);
        stopShake = true;
    }
}
