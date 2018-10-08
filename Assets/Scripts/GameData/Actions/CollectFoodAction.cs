using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectFoodAction : GoapAction
{
    private bool collected = false;
    private BushEntity targetBush;

    private float startTime = 0;
    public float collectDuration = 3; // seconds

    public CollectFoodAction()
    {
        addPrecondition("hasEnergy", true); // we need energy
        addPrecondition("hasFood", false); // if we have wood we don't want more
        addEffect("hasFood", true);
    }


    public override void reset()
    {
        collected = false;
        targetBush = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return collected;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a tree
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest tree that we can mine
        BushEntity[] bushes = FindObjectsOfType(typeof(BushEntity)) as BushEntity[];
        BushEntity closest = null;
        float closestDist = 0;
        foreach (BushEntity bush in bushes)
        {
            if (bush.food > 0)
            {
                if (closest == null)
                {
                    // first one, so choose it for now
                    closest = bush;
                    closestDist = (bush.gameObject.transform.position - agent.transform.position).magnitude;
                }
                else
                {
                    // is this one closer than the last?
                    float dist = (bush.gameObject.transform.position - agent.transform.position).magnitude;
                    if (dist < closestDist)
                    {
                        // we found a closer one, use it
                        closest = bush;
                        closestDist = dist;
                    }
                }
            }
        }
        targetBush = closest;
        target = targetBush.gameObject;
        return closest != null;
    }

    public override bool perform(GameObject agent)
    {

        if (startTime == 0)
        {
            Collector collector = (Collector)agent.GetComponent(typeof(Collector));
            collector.collecting = true;
            startTime = Time.time;
        }

        if (Time.time - startTime > collectDuration)
        {
            // finished cutting
            Collector collector = (Collector)agent.GetComponent(typeof(Collector));

            int food = 20;
            if ((targetBush.food - food) >= 0)
            {
                collector.food += 20;
                targetBush.food -= 20;
            }
            else
            {
                collector.food += targetBush.food;
                targetBush.food = 0;
            }
            collector.collecting = false;
            collector.energy -= 51;
            collected = true;
        }
        return true;
    }

}


