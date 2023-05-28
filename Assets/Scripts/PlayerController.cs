using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : EntityController
{    
    private List<UnitController> unitList = new List<UnitController>();

    public GameEnums.CommandTypes selectedCommand = GameEnums.CommandTypes.FOLLOW;

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
        Vector2 mov = targetPos * movementSpeed;
        Vector2 newPos = currentPos + mov * Time.fixedDeltaTime;

        rb.MovePosition(newPos);
    }

    void SendCommand() {
        Debug.Log("Command");
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        foreach (UnitController unit in unitList)
        {
            unit.SetCommand(selectedCommand, new Vector2(mousePos.x, mousePos.y));
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
        Debug.Log("Drawing");
    }
    
    void PlayerControls() {
        targetPos = Vector2.ClampMagnitude(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), 1);

        if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt)) {
            DrawFormation();
        }
        else if (Input.GetMouseButtonDown(0)) {
            SendCommand();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            ChangeCommand(true);
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            ChangeCommand(false);
        }

    }

    public void RegisterUnit(UnitController unit) {
        unitList.Add(unit);
    }

    public Vector2 GetPosition() {
        return rb.position;
    }

    public List<Vector2> GetUnitsPositions() {
        return unitList.Select(unit => new Vector2(unit.transform.position.x, unit.transform.position.y)).ToList();
    }
}
