using UnityEngine;
using Fusion;

public class Shield : NetworkBehaviour
{
    public float shieldDuration = 3.0f; // Duration of the shield effect

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

                // Attach this shield to the player
                transform.SetParent(player.transform); // Make the shield a child of the player
                transform.localPosition = Vector3.zero; // Center the shield around the player

                // Optional: Adjust the scale to wrap around the player
                transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // Adjust based on your shield size

                // Optional: Disable collisions with the player
                Collider shieldCollider = GetComponent<Collider>();
                if (shieldCollider != null)
                {
                    shieldCollider.enabled = false; // Prevent further collisions
                }

                // Start a coroutine to destroy the shield after the duration
                StartCoroutine(DestroyShieldAfterDuration());
            }
        }
    }

    private System.Collections.IEnumerator DestroyShieldAfterDuration()
    {
        // Wait for the shield duration
        yield return new WaitForSeconds(shieldDuration);

        // Destroy the shield object across the network
        if (Object.HasStateAuthority)
        {
            Runner.Despawn(Object);
        }
    }
}
