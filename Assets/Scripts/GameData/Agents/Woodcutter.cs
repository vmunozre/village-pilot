using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Woodcutter : MonoBehaviour, IGoap
{
    public int wood = 0;
    public int energy = 100;
    public float moveSpeed = 2;

    // Estados para las animaciones
    public bool cutting = false;
    public bool recovering = false;
    public bool keeping = false;


    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        animator.SetBool("cutting", cutting);
        animator.SetBool("keeping", keeping);
        animator.SetBool("recovering", recovering);
    }

    /**
	 * Key-Value data that will feed the GOAP actions and system while planning.
	 */
    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("hasWood", (wood > 0)));
        worldData.Add(new KeyValuePair<string, object>("hasEnergy", (energy > 0)));

        return worldData;
    }

    /**
	 * Implement in subclasses
	 */
    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("collectWood", true));
        return goal;
    }


    public void planFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        // Not handling this here since we are making sure our goals will always succeed.
        // But normally you want to make sure the world state has changed before running
        // the same goal again, or else it will just fail.
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<GoapAction> actions)
    {
        // Yay we found a plan for our goal
        Debug.Log("<color=green>Plan found</color> " + GoapAgent.prettyPrint(actions));
    }

    public void actionsFinished()
    {
        // Everything is done, we completed our actions for this gool. Hooray!
        Debug.Log("<color=blue>Actions completed</color>");
    }

    public void planAborted(GoapAction aborter)
    {
        // An action bailed out of the plan. State has been reset to plan again.
        // Take note of what happened and make sure if you run the same goal again
        // that it can succeed.
        Debug.Log("<color=red>Plan Aborted</color> " + GoapAgent.prettyPrint(aborter));
    }

    public bool moveAgent(GoapAction nextAction)
    {
        // move towards the NextAction's target
        float step = moveSpeed * Time.deltaTime;
        Vector3 actualTarget = new Vector3(nextAction.target.transform.position.x, nextAction.target.transform.position.y, transform.position.z);
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, actualTarget, step);
        Vector3 toCompare = new Vector3(nextAction.target.transform.position.x, nextAction.target.transform.position.y, transform.position.z);
        if (gameObject.transform.position.Equals(toCompare))
        {
            // we are at the target location, we are done
            nextAction.setInRange(true);
            return true;
        }
        else
            return false;
    }
}


