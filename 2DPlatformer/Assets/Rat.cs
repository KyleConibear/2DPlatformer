using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rat : Enemy
{
    [SerializeField] private GameObject explosionPrefab = null;

    private int arrivalCounter = 0;
    protected override void ArrivedAtDestination()
    {
        arrivalCounter++;
        Debug.Log("Arrived at destination");

        if(arrivalCounter > 1)
        {
            Destroy(this.gameObject);
            Instantiate(explosionPrefab, base.actor.transform.position, Quaternion.identity);
        }
    }
}
