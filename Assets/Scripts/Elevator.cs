using System;
using UnityEngine;

public class Elevator : MonoBehaviour {

    public int Floors;
    public FloorsView FloorsView;
    public ElevatorView View;
    public ElevatorBehaviour Behaviour;
    public ElevatorShaftView Shaft;

	// Use this for initialization
	void Start ()
    {
        FloorsView.GenerateFloorButtons(Floors, FloorButtonClicked);
        View.GenerateElevatorButtons(Floors, ElevatorButtonClicked);
        Shaft.Init(Floors);
        Behaviour.OnPositonChanged += Shaft.ChangePosition;
        Behaviour.OnPositonChanged += View.SetFloor;
        Behaviour.OnStateChanged += FloorStateChanged;
        Behaviour.Launch(Floors);
	}

    private void FloorStateChanged(int floor, FloorState state)
    {
        if (!state.DownPressed)
        {
            FloorsView.ResetButton(floor, FloorPanel.Direction.Down);
        }
        if (!state.UpPresed)
        {
            FloorsView.ResetButton(floor, FloorPanel.Direction.Up);
        }

        if (!state.ChoosedInElevator)
        {
            View.ResetButton(floor);
        }

    }

    private void FloorButtonClicked(int floor, FloorPanel.Direction direction)
    {
        FloorState state = Behaviour.GetState(floor);
        switch (direction)
        {
            case FloorPanel.Direction.Down:
                state.DownPressed = true;
                break;
            case FloorPanel.Direction.Up:
                state.UpPresed = true;
                break;
        }
        Behaviour.SetState(floor, state);
    }

    private void ElevatorButtonClicked(int floor)
    {
        FloorState state = Behaviour.GetState(floor);
        state.ChoosedInElevator = true;
        Behaviour.SetState(floor, state);
    }
}
