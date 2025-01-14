using UnityEngine;
using Fusion;

public class Player : NetworkBehaviour
{
    private CharacterController _cc;

    [SerializeField] float speed = 5f;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject floatingIcon; // Reference to the icon above the player's head
    [Networked] public bool controlsSwapped { get; private set; } // Tracks whether the player controls are swapped
    [Networked] private TickTimer SwapTimer { get; set; } // Tracks shield duration


    private Camera firstPersonCamera;

    [Networked] private TickTimer ShieldTimer { get; set; } // Tracks shield duration
    [Networked] public bool IsShielded { get; private set; } // Tracks whether the player is shielded

    public override void Spawned()
    {
        _cc = GetComponent<CharacterController>();
        if (HasStateAuthority)
        {
            firstPersonCamera = Camera.main;
            var firstPersonCameraComponent = firstPersonCamera.GetComponent<FirstPersonCamera>();
            if (firstPersonCameraComponent && firstPersonCameraComponent.isActiveAndEnabled)
                firstPersonCameraComponent.SetTarget(this.transform);
        }

        // Ensure the floating icon is initially hidden
        if (floatingIcon != null)
        {
            floatingIcon.SetActive(false);
        }
    }

    private Vector3 moveDirection;

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData inputData))
        {
            if (inputData.moveActionValue.magnitude > 0)
            {
                inputData.moveActionValue.Normalize(); // Ensure that the vector magnitude is 1

                // Swap controls if the effect is active
                var moveX = controlsSwapped ? -inputData.moveActionValue.x : inputData.moveActionValue.x;
                var moveY = controlsSwapped ? -inputData.moveActionValue.y : inputData.moveActionValue.y;

                moveDirection = new Vector3(moveX, 0, moveY);
                Vector3 DeltaX = speed * moveDirection * Runner.DeltaTime;
                _cc.Move(DeltaX);
            }

            if (HasStateAuthority)
            {
                if (inputData.shootActionValue)
                {
                    Debug.Log("SHOOT!");
                    Runner.Spawn(ballPrefab,
                        transform.position + moveDirection, Quaternion.LookRotation(moveDirection),
                        Object.InputAuthority);
                }
            }
        }

        // Check if the shield timer has expired
        if (IsShielded && ShieldTimer.Expired(Runner))
        {
            DeactivateShield();
        }
        // Check if the swap timer has expired
        if (controlsSwapped && SwapTimer.Expired(Runner))
        {
            UnswapControls();
        }
    }

    public void ActivateShield(float duration)
    {
        if (IsShielded) return; // Prevent multiple shields

        IsShielded = true;
        ShieldTimer = TickTimer.CreateFromSeconds(Runner, duration);

        // No need to instantiate a separate shield effect, as the Shield object is now wrapping the player
    }

    private void DeactivateShield()
    {
        IsShielded = false;
        // Shield object will automatically despawn after its duration
    }
    public void SwapControls(float duration)
    {
        if (controlsSwapped) return;

        controlsSwapped = true;
        SwapTimer = TickTimer.CreateFromSeconds(Runner, duration);

        // Trigger a network event to show the icon
        RPC_ShowFloatingIcon(true);
    }
    private void UnswapControls()
    {
        controlsSwapped = false;

        // Trigger a network event to hide the icon
        RPC_ShowFloatingIcon(false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_ShowFloatingIcon(bool isVisible)
    {
        // Update the floating icon visibility on all clients
        if (floatingIcon != null)
        {
            floatingIcon.SetActive(isVisible);
        }
    }

    

    private void OnCollisionEnter(Collision collision)
    {
        if (IsShielded && collision.gameObject.CompareTag("Ball"))
        {
            // Absorb the ball hit
            Debug.Log("Shield absorbed the ball!");
        }

        if(controlsSwapped && collision.gameObject.CompareTag("Ball"))
        {
            // Swap the controls
            Debug.Log("Controls swapped!");
        }
    }
}
