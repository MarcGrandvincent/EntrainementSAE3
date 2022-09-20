using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    private float amnt;
    private float camPosY;
    private float camPosX;

    // Start is called before the first frame update
    void Start()
    {
        camPosX = transform.position.x;
        camPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float xNew = Mathf.Lerp(transform.position.x, camPosY, Time.deltaTime * 10);
        transform.position = new Vector3(xNew, transform.position.y, transform.position.z);
    }



    public void Shake(float amount, float time)
    {
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
        CancelInvoke("BeginShake");
        float xNew = Mathf.Lerp(transform.position.x, camPosX, Time.deltaTime * 10);
        transform.position = new Vector3(xNew, camPosX, transform.position.z);
    }
}
