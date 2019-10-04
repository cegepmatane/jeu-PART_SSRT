using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    public Transform tree;
    private float minDistance = 5f;

    public float movementSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float enemySpeed = movementSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, tree.position, enemySpeed);

        checkMinDistance();
    }

    private void checkMinDistance()
    {
        if (Vector3.Distance(transform.position, tree.position) <= minDistance)
        {
            movementSpeed = 0;
        }
    }
}
