using System;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    #region publicVariables
    public ElevatorView View;
    public ElevatorBehaviour Behaviour;
    public ElevatorShaftView Shaft;
    #endregion

    private Human _draggingHuman;

    #region privateMethods
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

    private void ElevatorButtonClicked(int floor)
    {
        FloorState state = Behaviour.GetState(floor);
        state.ChoosedInElevator = true;
        Behaviour.SetState(floor, state);
        Behaviour.Continue();
    }
    #endregion

    #region publicMethods
    public void Launch(int floors)
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

    public void DragHuman(Human human)
    {
        _draggingHuman = human;
    }

    public void Drop(Transform aim)
    {
        if (_draggingHuman && Behaviour.DoorOpen && _draggingHuman.DraggingFrom != aim)
        {
            bool canDragFromThisFloor = _draggingHuman.DraggingFrom.GetComponent<ElevatorCabin>() || (Behaviour.Floors - _draggingHuman.DraggingFrom.GetComponentInParent<FloorPanel>().transform.GetSiblingIndex() - 1) == Behaviour.CurrentFloor;
            if (canDragFromThisFloor)
            {
                if (aim.GetComponent<ElevatorCabin>())
                {
                    Behaviour.AddHuman();
                    _draggingHuman.Drop(aim);
                }

                if (aim.GetComponent<WaitingPlace>() && !_draggingHuman.DraggingFrom.GetComponent<WaitingPlace>())
                {
                    Behaviour.RemoveHuman();
                    _draggingHuman.Drop(aim);
                }
            }
            _draggingHuman = null;
        }
    }
    #endregion
}
