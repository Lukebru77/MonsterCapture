using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public StateMachine[] enemy;
    public Transform pointA, pointB;
    public float timer;
    public float resetTime;


    // Start is called before the first frame update
    void Start()
    {
        timer = resetTime;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            Spawn();

            if (resetTime > .5f)
            {
                resetTime -= .1f;
            }
            resetTime *= .99f;
            timer = resetTime;
        }
    }

    void Spawn()
    {
        Vector3 pos = Vector3.Lerp(pointA.position, pointB.position, Random.Range(-1f, 1f));
        StateMachine randomAsteroid = enemy[Random.Range(0, enemy.Length)];
        Instantiate(randomAsteroid, pos, Quaternion.identity);
    }
}
