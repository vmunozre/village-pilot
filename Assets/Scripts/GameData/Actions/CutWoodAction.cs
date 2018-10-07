using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CutWoodAction : GoapAction
{
    private bool cutted = false;
    private TreeEntity targetTree; // where we get the ore from

    private float startTime = 0;
    public float cuttingDuration = 3; // seconds

    public CutWoodAction()
    {
        addPrecondition("hasEnergy", true); // we need energy
        addPrecondition("hasWood", false); // if we have wood we don't want more
        addEffect("hasWood", true);
    }


    public override void reset()
    {
        cutted = false;
        targetTree = null;
        startTime = 0;
    }

    public override bool isDone()
    {
        return cutted;
    }

    public override bool requiresInRange()
    {
        return true; // yes we need to be near a tree
    }

    public override bool checkProceduralPrecondition(GameObject agent)
    {
        // find the nearest tree that we can mine
        TreeEntity[] trees = FindObjectsOfType(typeof(TreeEntity)) as TreeEntity[];
        TreeEntity closest = null;
        float closestDist = 0;
        foreach (TreeEntity tree in trees)
        {
            if(tree.wood > 0)
            {
                if (closest == null)
                {
                    // first one, so choose it for now
                    closest = tree;
                    closestDist = (tree.gameObject.transform.position - agent.transform.position).magnitude;
                }
                else
                {
                    // is this one closer than the last?
                    float dist = (tree.gameObject.transform.position - agent.transform.position).magnitude;
                    if (dist < closestDist)
                    {
                        // we found a closer one, use it
                        closest = tree;
                        closestDist = dist;
                    }
                }
            }
        }
        targetTree = closest;
        target = targetTree.gameObject;
        return closest != null;
    }

    public override bool perform(GameObject agent)
    {
        
        if (startTime == 0)
        {
            Woodcutter woodcutter = (Woodcutter)agent.GetComponent(typeof(Woodcutter));
            woodcutter.cutting = true;
            startTime = Time.time;
        }

        if (Time.time - startTime > cuttingDuration)
        {
            // finished cutting
            Woodcutter woodcutter = (Woodcutter)agent.GetComponent(typeof(Woodcutter));
            targetTree.clipped = true;
            int wood = 20;
            if((targetTree.wood - wood) >= 0)
            {
                woodcutter.wood += 20;
                targetTree.wood -= 20;
            } else
            {
                woodcutter.wood += targetTree.wood;
                targetTree.wood = 0;
            }
            woodcutter.cutting = false;
            woodcutter.energy -= 51;
            cutted = true;
        }
        return true;
    }

}


