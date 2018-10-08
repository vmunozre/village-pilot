using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropFoodAction : GoapAction
{
    private bool droppedFood = false;
    private WarehouseEntity targetWarehouse; 

    private float startTime = 0;
    public float dropDuration = 1.5f; // seconds

    public DropFoodAction()
    {
        addPrecondition("hasFood", true); 
        addEffect("hasFood", false); 
        addEffect("collectFood", true); 
    }


    public override void reset()
    {
        droppedFood = false;
        targetWarehouse = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return droppedFood;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a supply pile so we can drop off the ore
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest supply pile that has spare ore
        WarehouseEntity[] supplyPiles = (WarehouseEntity[])UnityEngine.GameObject.FindObjectsOfType(typeof(WarehouseEntity));
        WarehouseEntity closest = null;
        float closestDist = 0;

        foreach (WarehouseEntity supply in supplyPiles)
        {
            if (closest == null)
            {
                // first one, so choose it for now
                closest = supply;
                closestDist = (supply.gameObject.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                // is this one closer than the last?
                float dist = (supply.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist)
                {
                    // we found a closer one, use it
                    closest = supply;
                    closestDist = dist;
                }
            }
        }
        if (closest == null)
            return false;

        targetWarehouse = closest;
        target = targetWarehouse.gameObject;

        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
        {
            Collector collector = (Collector)agent.GetComponent(typeof(Collector));
            collector.keeping = true;
            startTime = Time.time;
        }

        if (Time.time - startTime > dropDuration)
        {
            Collector collector = (Collector)agent.GetComponent(typeof(Collector));
            targetWarehouse.food += collector.food;
            collector.food = 0;
            collector.keeping = false;
            droppedFood = true;
        }
        return true;
    }
}

