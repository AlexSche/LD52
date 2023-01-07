using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 0.5f;
    public Tilemap map;
    private PlayerInput playerInput;
    private Vector2 moveDirection = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    private Rigidbody2D rb2D;

    private void Awake() => playerInput = new PlayerInput();
    private void OnEnable() => playerInput.Enable();
    private void OnDisable() => playerInput.Disable();
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerInput.Player.Move.performed += ctx => Move(ctx);
        playerInput.Player.Fire.performed += ctx => Dig(ctx);
    }

    // Update is called once per frame
    void Update()
    {
    }


    void Move(CallbackContext context) {
        moveDirection = playerInput.Player.Move.ReadValue<Vector2>();
        Debug.Log(moveDirection);
        if (moveDirection.x != 0) {
            // Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(moveDirection.x * movementSpeed, rb2D.velocity.y);
			// And then smoothing it out and applying it to the character
			//rb2D.velocity = Vector3.SmoothDamp(rb2D.velocity, targetVelocity, ref velocity, 3);
            // Test without smoothing since it doesnt work
            rb2D.velocity = targetVelocity;
        }
        if (moveDirection.y < 0) {
            //Look down
        }
        if (moveDirection.y > 0) {
            //Look up;
        }
    }

    void Dig(CallbackContext context) {
        // Find the tile we are looking at
        Vector2 tileUnderneath = new Vector2(transform.position.x, transform.position.y) + moveDirection;
        Debug.DrawLine(transform.position, tileUnderneath,Color.red,4);
        // Translate worldPosition to gridPosition
        Vector3Int gridPosition = map.WorldToCell(tileUnderneath);
        // Remove looked at tile
        // Check if position is on a tile
        if (map.HasTile(gridPosition)) {
            Debug.Log("Remove Tile!");
            map.SetTile(gridPosition,null);
        }
    }
}
