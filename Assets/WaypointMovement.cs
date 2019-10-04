using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    public Transform tree;
    private float minDistance = 5f;

    public float valueMovementSpeed = 10f;
    private float movementSpeed;
    private float rotationSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = valueMovementSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Calcul de la vitesse et de la rotation de l'ennemi
        float enemySpeed = movementSpeed * Time.deltaTime;
        float enemyRotation = rotationSpeed * Time.deltaTime;
        Vector3 direction = tree.position - transform.position;

        //Application du mouvement
        transform.position = Vector3.MoveTowards(transform.position, tree.position, enemySpeed);
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;

        checkMinDistance();
    }

    //S'arrête a une distance prédéfinie
    private void checkMinDistance()
    {
        if (Vector3.Distance(transform.position, tree.position) <= minDistance)
        {
            movementSpeed = 0f;
        }
        else
        {
            movementSpeed = valueMovementSpeed;
        }
    }
}
