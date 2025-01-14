using UnityEngine;
using Fusion;

public class Swap : NetworkBehaviour
{
    [SerializeField] private float swapDuration = 10f;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with object having tag: {other.tag}");
        // Check if the colliding object has a Player component
        if (other.TryGetComponent<Player>(out Player player) && other.CompareTag("Player"))
        {
            // Apply the swap effect to the player
            player.SwapControls(swapDuration);

            // Despawn this buffer object
            if (Object != null && Object.HasStateAuthority)
            {
                Runner.Despawn(Object); // Use Fusion's despawn method
            }
        }
    }
}
