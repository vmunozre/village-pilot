using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseEntity : MonoBehaviour {
    public int wood = 0;
    public int food = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void addFood(int _food)
    {
        food += _food;
    }

    public void addWood(int _wood)
    {
        wood += _wood;
    }

    public void removeFood(int _food)
    {
        food -= _food;
    }

    public void removeWood(int _wood)
    {
        wood -= _wood;
    }
}
