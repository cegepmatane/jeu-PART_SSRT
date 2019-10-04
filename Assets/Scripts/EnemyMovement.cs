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
    public float attackRate = 2f;

    // Start is called before the first frame update
    void Start()
    {
        movementSpeed = valueMovementSpeed;
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

    private IEnumerator Attack()
    {
        while (isAttacking)
        {
            treeCollider.gameObject.GetComponent<TreeHealth>().ApplyDamage(damage);
            yield return new WaitForSeconds(attackRate);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == treeCollider)
        {
            isAttacking = true;
            StartCoroutine("Attack");
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider == treeCollider)
            isAttacking = false;
    }

}
