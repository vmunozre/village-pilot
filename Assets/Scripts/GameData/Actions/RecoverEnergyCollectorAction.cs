using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverEnergyCollectorAction : GoapAction
{
    private bool recovered = false;
    private CenterEntity targetCenter;

    private float startTime = 0;
    public float recoveringDuration = 5; // seconds

    public RecoverEnergyCollectorAction()
    {
        addPrecondition("hasEnergy", false); // we need energy
        addEffect("hasEnergy", true);
    }


    public override void reset()
    {
        recovered = false;
        targetCenter = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return recovered;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a tree
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest tree that we can mine
        CenterEntity[] centers = FindObjectsOfType(typeof(CenterEntity)) as CenterEntity[];
        CenterEntity closest = null;
        float closestDist = 0;
        foreach (CenterEntity center in centers)
        {

            if (closest == null)
            {
                // first one, so choose it for now
                closest = center;
                closestDist = (center.gameObject.transform.position - agent.transform.position).magnitude;
            }
            else
            {
                // is this one closer than the last?
                float dist = (center.gameObject.transform.position - agent.transform.position).magnitude;
                if (dist < closestDist)
                {
                    // we found a closer one, use it
                    closest = center;
                    closestDist = dist;
                }
            }

        }
        targetCenter = closest;
        target = targetCenter.gameObject;
        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        if (startTime == 0)
        {
            Collector collector = (Collector)agent.GetComponent(typeof(Collector));
            collector.recovering = true;
            startTime = Time.time;
        }

        if (Time.time - startTime > recoveringDuration)
        {
            Collector collector = (Collector)agent.GetComponent(typeof(Collector));
            collector.energy = 100;
            collector.recovering = false;
            recovered = true;
        }
        return true;
    }

}


