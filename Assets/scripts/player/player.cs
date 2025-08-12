using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D myRigidBody;
    public Vector2 velocity;
    public float speed;

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            myRigidBody.velocity = new Vector2(-speed, myRigidBody.velocity.y);
            //myRigidBody.MovePosition(myRigidBody.position - velocity * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            myRigidBody.velocity = new Vector2(speed, myRigidBody.velocity.y);
            //myRigidBody.MovePosition(myRigidBody.position + velocity * Time.deltaTime);
        }
    }
}
