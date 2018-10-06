
using System;
using UnityEngine;

public class DropWoodAction : GoapAction
{
    private bool droppedWood = false;
    private WarehouseEntity targetWarehouse; // where we drop off the ore

    public DropWoodAction()
    {
        addPrecondition("hasWood", true); // can't drop off ore if we don't already have some
        addEffect("hasWood", false); // we now have no ore
        addEffect("collectWood", true); // we collected ore
    }


    public override void reset()
    {
        droppedWood = false;
        targetWarehouse = null;
    }

    public override bool isDone()
    {
        return droppedWood;
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
        Woodcutter woodcutter = (Woodcutter)agent.GetComponent(typeof(Woodcutter));
        targetWarehouse.wood += woodcutter.wood;
        woodcutter.wood = 0;
        droppedWood = true;
        //TODO play effect, change actor icon

        return true;
    }
}
