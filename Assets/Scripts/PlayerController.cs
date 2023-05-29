using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : EntityController
{    
    private List<UnitController> unitList = new List<UnitController>();
    private Vector2 directionMovement;
    
    // private List<UnitController> unitList = new List<UnitController>();

    private List<Vector3> mousePositions = new List<Vector3>();
    private bool isDrawingFormation = false;
    public int mousePositionInterval = 100;

    public int playerSpeed = 1;
    public GameEnums.CommandTypes selectedCommand = GameEnums.CommandTypes.FOLLOW;
    public int selectedSquad = -1;

    // Update is called once per frame
    void Update()
    {
        PlayerControls();
    }

    void FixedUpdate() {
        PlayerMovement();
    }

    void PlayerMovement() {
        Vector2 currentPos = rb.position;
        Vector2 mov = directionMovement * movementSpeed;
        Vector2 newPos = currentPos + mov * Time.fixedDeltaTime;

        rb.MovePosition(newPos);
    }

    void SendCommand() {
        Debug.Log("Command");
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (selectedSquad == -1) {
            foreach (UnitController unit in BattleManager.Instance.totalPlayerUnits)
            {
                unit.SetCommand(selectedCommand, new Vector2(mousePos.x, mousePos.y));
            }
        }
        else {
            foreach (UnitController unit in BattleManager.Instance.squads[selectedSquad].units)
            {
                unit.SetCommand(selectedCommand, new Vector2(mousePos.x, mousePos.y));
            }
        }
    }

    void ChangeCommand(bool isUp) {
        GameEnums.CommandTypes newCommand = selectedCommand;
        System.Array enumTypes = System.Enum.GetValues(typeof(GameEnums.CommandTypes));

        if (isUp) {
            newCommand++;

            selectedCommand = (GameEnums.CommandTypes) ((int)(newCommand) % enumTypes.Length);
        }
        else{
            newCommand--;

            if (newCommand == 0) {
                selectedCommand = GameEnums.CommandTypes.FOLLOW;
            }
            else {
                selectedCommand = (GameEnums.CommandTypes) (Mathf.Abs(enumTypes.Length + (int)(newCommand)) % enumTypes.Length);
            }
        }
        Debug.Log("Current Command: " + selectedCommand);
    }

    void DrawFormation() {
        if (!isDrawingFormation) {
            mousePositions.Clear();
            isDrawingFormation = true;
        }

        Vector3 mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0) ;

        if (mousePositions.Count == 0) {
            mousePositions.Add(mousePos);
            Debug.Log("Added Position in " + mousePos);
            return;
        }

        if (Vector2.Distance(mousePos, mousePositions[mousePositions.Count - 1]) > mousePositionInterval) {
            mousePositions.Add(mousePos);
            Debug.Log("Added Position in " + mousePos);
            return;
        }
    }
    
    void PlayerControls() {
        directionMovement = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1);

        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt)) {
            DrawFormation();
        }
        else if (Input.GetMouseButtonDown(0)) {
            isDrawingFormation = false;
            BattleManager.Instance.SetFormation(mousePositions, selectedSquad);
            SendCommand();
        }
        else{
            isDrawingFormation = false;
        } 

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ChangeCommand(true);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            ChangeCommand(false);
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            selectedSquad--;
            if (selectedSquad < -1) {
                selectedSquad = -1;
            }
        }
        else if(Input.GetKeyDown(KeyCode.E)) {
            selectedSquad++;
            if (selectedSquad > BattleManager.Instance.squads.Count) {
                selectedSquad = BattleManager.Instance.squads.Count;
            }
        }

    }


    public Vector2 GetPosition() {
        return rb.position;
    }

}
