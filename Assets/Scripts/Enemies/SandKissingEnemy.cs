using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// comment for unity learn course: INHERITANCE
public class SandKissingEnemy : WanderingEnemy
{
    // comment for unity learn course: POLYMORPHISM
    protected override Vector3 GetNextDestination()
    {
        // trying to find the player first
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return base.GetNextDestination();

        Vector3 direction = player.transform.position - transform.position;
        
        float distanceToMove = Random.Range(moveRadius / 2, moveRadius);
        direction = Vector3.ClampMagnitude(direction, distanceToMove);
        
        Vector3 castOriginPoint = transform.position;

        // casting on the surface level, cause only lava needs to be found
        castOriginPoint.y = 0;
        Ray sphereCast = new Ray(castOriginPoint, direction);

        // if no lava on the chosen way, move to player's old position
        if (Physics.SphereCast(sphereCast, radius, direction.magnitude))
        {
            // ..or do it the usual wandering way
            return base.GetNextDestination();
        }

        return new Vector3(
                direction.x + transform.position.x,
                transform.position.y,
                direction.z + transform.position.z
            );
    }

}
