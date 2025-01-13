// using UnityEngine;
// using Fusion;

// public class Shield : NetworkBehaviour
// {
//     public float shieldDuration = 3.0f; // Duration of the shield effect

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             // Get the player's NetworkObject
//             var player = other.GetComponent<Player>();
//             if (player != null)
//             {
//                 // Grant the shield to the player who collided
//                 player.ActivateShield(shieldDuration);

//                 // Destroy this shield object across the network
//                 if (Object.HasStateAuthority)
//                 {
//                     Runner.Despawn(Object);
//                 }
//             }
//         }
//     }
// }

using UnityEngine;
using Fusion;

public class Shield : NetworkBehaviour
{
    public float shieldDuration = 5.0f; // Duration of the shield effect

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Get the player's NetworkObject
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                // Grant the shield to the player who collided
                player.ActivateShield(shieldDuration);

                // Destroy this shield object across the network
                if (Object.HasStateAuthority)
                {
                    Runner.Despawn(Object);
                }
            }
        }
    }
}
