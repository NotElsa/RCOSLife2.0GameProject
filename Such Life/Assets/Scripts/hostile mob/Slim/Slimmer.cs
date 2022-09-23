using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Slimmer : mobBase
{
    private float timeDelay;
    private float time;
    private float alertRange;
    // Start is called before the first frame update
    public Transform target;
    public NavMeshAgent agent;
    public SpriteRenderer slimeSprite;
    
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        maxHealth = 5;
        damage = 2;
        currHealth = maxHealth;
        walkSpeed = 1f;
        playerSighted = false;
        time_move = 3.0f;
        currPosition = new Vector2(transform.position.x, transform.position.y);

        timeDelay = 400f;
        time = 0f;
        alertRange = 6.0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += 1f;
        float distance = findDistance(currPosition.x, target.position.x, currPosition.y, target.position.x);
        if (distance <= alertRange && !playerSighted)
        {
            playerSighted = true;
            flipSprite(-target.position.x);
        }
        if (time >= timeDelay)
        {
            time = 0;
            if (playerSighted)
            {
                chasing(distance);
            }
            else
            {
                wander();
            }
        }
    }

    void wander()
    {
        float oldPosX = currPosition.x;
        wanderPositionChange();
        flipSprite(oldPosX);
        agent.SetDestination(currPosition);
    }

    void chasing(float distance)
    {
        float oldPosX = currPosition.x;
        float angle = Mathf.Atan((target.position.y - currPosition.y) / (target.position.x - currPosition.x));
        if (distance >= 4.0f)
        {
            this.currPosition = new Vector2(currPosition.x + 4 * Mathf.Cos(angle), currPosition.y + 4 * Mathf.Sin(angle));
        }
        else if (distance < 1.0f)
        {
            this.currPosition = new Vector2(currPosition.x + 2 * Mathf.Cos(angle), currPosition.y + 2 * Mathf.Sin(angle));
        }
        else
        {
            this.currPosition = new Vector2(currPosition.x + 3 * Mathf.Cos(angle), currPosition.y + 3 * Mathf.Sin(angle));
        }
        flipSprite(oldPosX);
        agent.SetDestination(currPosition);
    }

    void wanderPositionChange()
    { 
        posxmin = transform.position.x - 1.0f;
        posxmax = transform.position.x + 1.0f;
        posymin = transform.position.y - 1.0f;
        posymax = transform.position.y + 1.0f;

        this.currPosition = new Vector2(Random.Range(posxmin, posxmax), Random.Range(posymin, posymax));
    }

    void flipSprite(float PosX)
    {
        if (currPosition.x > PosX)
        {
            slimeSprite.flipX = true;
        }
        else
        {
            slimeSprite.flipX = false;
        }
    }

    float findDistance(float x1, float y1, float x2, float y2)
    {
        return Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2));
    }

}