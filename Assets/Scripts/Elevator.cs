using System;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public ElevatorView View;
    public ElevatorBehaviour Behaviour;
    public ElevatorShaftView Shaft;

	public void Launch (int floors)
    {
        View.GenerateElevatorButtons(floors, ElevatorButtonClicked);
        Shaft.Init(floors, FloorButtonClicked);
        Behaviour.OnPositonChanged += Shaft.ChangePosition;
        Behaviour.OnPositonChanged += View.SetFloor;
        Behaviour.OnStateChanged += FloorStateChanged;
        Behaviour.OnPeoplesCountChanged += PeoplesCountChanged;
        Behaviour.OnDoorsStateChanged += Shaft.SetCabinState;
        Behaviour.Launch(floors);
	}

    private void PeoplesCountChanged(int v)
    {
        View.LightUpButtons = v != 0;
    }

    private void FloorStateChanged(int floor, FloorState state)
    {
        if (!state.DownPressed)
        {
            Shaft.ResetButton(floor, FloorPanel.Direction.Down);
        }
        if (!state.UpPresed)
        {
            Shaft.ResetButton(floor, FloorPanel.Direction.Up);
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

    public void PlaceHumanInside(Human human)
    {
        Behaviour.AddHuman();
        Shaft.PlaceHumanInside(human.transform);
    }

    private void ElevatorButtonClicked(int floor)
    {
        FloorState state = Behaviour.GetState(floor);
        state.ChoosedInElevator = true;
        Behaviour.SetState(floor, state);
    }
}
