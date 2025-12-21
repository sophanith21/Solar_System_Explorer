using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollisionDetection : MonoBehaviour
{
    public SpaceshipRB spaceShipController;
    public void OnParticleCollision(GameObject other)
    {
      
        spaceShipController.teleportSpaceship();

        Debug.Log("Particle Collision Detected with " + other.name + "!");
    }
}
