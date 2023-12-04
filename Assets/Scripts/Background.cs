using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float speed;
    public int starti;
    public int endi;
    public Transform[] sprites;

    float viewHeight;
    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;
    }
    void Update()
    {
        Move();
        Scrolling();
    }

    void Move()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * speed * Time.deltaTime;
        transform.position = curPos + nextPos;
    }

    void Scrolling()
    {
        if (sprites[endi].position.y < viewHeight * (-1))
        {
            Vector3 backSpritePos = sprites[starti].localPosition;
            Vector3 frontSpritePos = sprites[endi].localPosition;
            sprites[endi].transform.localPosition = backSpritePos + Vector3.up * viewHeight;

            int startiSave = starti;
            starti = endi;
            endi = (startiSave - 1 == -1) ? sprites.Length - 1 : startiSave - 1;
        }
    }
}
