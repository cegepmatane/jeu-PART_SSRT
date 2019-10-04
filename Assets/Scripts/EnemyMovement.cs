using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform treePosition;
    public Collider treeCollider;

    public float valueMovementSpeed = 10f;
    private float movementSpeed;
    private float rotationSpeed = 2f;

    private bool isAttacking = false;
    public float damage = 1f;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = valueMovementSpeed;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isAttacking)
            Attack();
    }


    void FixedUpdate()
    {
        //Calcul de la vitesse et de la rotation de l'ennemi
        float enemySpeed = movementSpeed * Time.deltaTime;
        float enemyRotation = rotationSpeed * Time.deltaTime;
        Vector3 direction = treePosition.position - transform.position;

        //Application du mouvement
        transform.position = Vector3.MoveTowards(transform.position, treePosition.position, enemySpeed);
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;
    }

    private void Attack()
    {
        treeCollider.gameObject.GetComponent<TreeHealth>().ApplyDamage(damage);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == treeCollider)
            isAttacking = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == treeCollider)
            isAttacking = false;
    }

}
