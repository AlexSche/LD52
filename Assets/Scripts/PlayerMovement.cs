using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 0.5f;
    public Tilemap map;
    public Tilemap background;
    public TileBase ladder;
    public Text collectedDiamondsText;
    private PlayerInput playerInput;
    private Vector2 moveDirection = Vector2.zero;
    private Vector3 velocity = Vector3.zero;
    private bool isMoving;
    private Rigidbody2D rb2D;

    private void Awake() => playerInput = new PlayerInput();
    private void OnEnable() => playerInput.Enable();
    private void OnDisable() => playerInput.Disable();
    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        playerInput.Player.Move.performed += ctx => Move(ctx);
        playerInput.Player.Move.started += ctx => SetIsMoving(ctx);
        playerInput.Player.Move.canceled += ctx => SetIsNotMoving(ctx);
        playerInput.Player.Fire.performed += ctx => Dig(ctx);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) {
            Move(new CallbackContext());
        }
        int amount = PlayerPrefs.GetInt("collectedDiamonds", 0);
        Debug.Log(amount);
        collectedDiamondsText.text = amount.ToString();
    }

    private void SetIsMoving(CallbackContext context) => isMoving = true;
    private void SetIsNotMoving(CallbackContext context) => isMoving = false;

    void Move(CallbackContext context) {
        moveDirection = playerInput.Player.Move.ReadValue<Vector2>();
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
            //if Ladder climb down else dig
        }
        if (moveDirection.y > 0) {
            //Look up;
            //if Ladder climb up
            Vector3Int gridPosition = map.WorldToCell(transform.position);
            if (background.HasTile(gridPosition)) {
                Debug.Log("Move up!");
                Vector3 targetVelocity = new Vector2(rb2D.velocity.x, moveDirection.y * movementSpeed);
                rb2D.velocity = targetVelocity;
            }
        }
    }

    void Dig(CallbackContext context) {
        // Find the tile we are looking at
        Vector2 tileUnderneath = new Vector2(transform.position.x, transform.position.y) + moveDirection;
        // Translate worldPosition to gridPosition
        Vector3Int gridPosition = map.WorldToCell(tileUnderneath);
        // Remove looked at tile
        // Check if position is on a tile
        if (map.HasTile(gridPosition)) {
            // If tile contains resources -> collect
            TileBase tileToDestory = map.GetTile(gridPosition);
            if (tileToDestory.name == "Diamonds") {
                // Collect diamonds
                PlayerPrefs.SetInt("collectedDiamonds", PlayerPrefs.GetInt("collectedDiamonds",0) + 3);
                PlayerPrefs.Save();
            }
            map.SetTile(gridPosition,null);
        }
        // If direction was down place a ladder
        if (moveDirection.y < 0) {
            background.SetTile(gridPosition,ladder);
        }
    }
}
