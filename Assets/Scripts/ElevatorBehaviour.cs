
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElevatorBehaviour : MonoBehaviour
{
    public enum ElevatorDirection
    {
        Up,
        Down
    }

    public enum ElevatorState
    {
        Moving,
        Waiting,
        Stoped,
        Loading
    }

    public float Speed = 1f;
    public float LoadingTime = 2f;

    private int _peoplesInside = 0;

    public Action<bool> OnDoorsStateChanged = (v) => { };
    public Action<int> OnPeoplesCountChanged = (i) => { };

    public int PeoplesInside
    {
        get
        {
            return _peoplesInside;
        }
        private set
        {
            _peoplesInside = value;
            if (_peoplesInside == 0)
            {
                for(int i = 0; i<_floors; i++)
                {
                    FloorState state = _floorStates[i];
                    state.ChoosedInElevator = false;
                    SetState(i, state);
                }
            }
            OnPeoplesCountChanged(_peoplesInside);
        }
    }

    public ElevatorState __elevatorState = ElevatorState.Waiting;
    public ElevatorDirection __elevatorDirection = ElevatorDirection.Up;

    private bool __doorOpen = false;
    public bool DoorOpen
    {
        get
        {
            return __doorOpen;
        }
        private set
        {
            __doorOpen = value;
            OnDoorsStateChanged(__doorOpen);
        }
    }

    public ElevatorState _elevatorState
    {
       get
        {
            return __elevatorState;
            }
set{
            if (__elevatorState!=value)
            {
                __elevatorState = value;
                if (__elevatorState == ElevatorState.Loading)
                {
                    DoorOpen = true;
                    
                    StartCoroutine(ContinueMooving(LoadingTime));
                }
            }
           
    }
    }

    private IEnumerator ContinueMooving(float loadingTime)
    {
        yield return new WaitForSeconds(loadingTime);

        DoorOpen = false;
        _aimFloor = GetNextAim();

        if (HasCalls)
        {
            _elevatorState = ElevatorState.Moving;
        }
        else
        {
            _elevatorState = ElevatorState.Waiting;
        }
        
    }

    private Dictionary<int, FloorState> _floorStates = new Dictionary<int, FloorState>();
    public int _aimFloor;
    public float __currentPosition;
    private float _currentPosition
    {
        get
        {
            return __currentPosition;
        }
        set
        {
            if (__currentPosition!=value)
            {
                __currentPosition = value;
                OnPositonChanged(__currentPosition);
            }
        }
    }

    public Action<float> OnPositonChanged = (p)=> { };
    public Action<int, FloorState> OnStateChanged = (f, s) => { };
    private float _arrivingTresshold = 0.01f;

    private int _floors
    {
        get
        {
            return _floorStates.Count;
        }
    }

    public int CurrentFloor
    {
        get
        {
            return Mathf.RoundToInt(_currentPosition);
        }
    }

    public bool HasCalls
    {
        get
        {
            return CallsNumber != 0;
        }
    }

    public int CallsNumber
    {
        get
        {
            return _floorStates.Values.Where(s => s.ChoosedInElevator || s.DownPressed || s.UpPresed).Count();
        }
    }

    public void Launch(int floors)
    {
        _floorStates = new Dictionary<int, FloorState>();
        for (int i = 0; i<floors; i++)
        {
            _floorStates.Add(i, new FloorState());
        }
        StartCoroutine(MoveElevator(Speed));
    }

    private IEnumerator MoveElevator(float speed)
    {
        while (true)
        {

            if (_elevatorState == ElevatorState.Moving)
            {
                if (Mathf.Abs(_currentPosition - _aimFloor) < _arrivingTresshold)
                {
                    FloorState state = _floorStates[CurrentFloor];

                    state.ChoosedInElevator = false;
                    if (__elevatorDirection == ElevatorDirection.Down)
                    {
                        state.DownPressed = false;
                    }
                    if (__elevatorDirection == ElevatorDirection.Up)
                    {
                        state.UpPresed = false;
                    }

                    SetState(CurrentFloor, state);

                    if (_elevatorState != ElevatorState.Loading && _elevatorState != ElevatorState.Waiting)
                    {
                        if (true)
                        {
                            _elevatorState = ElevatorState.Loading;
                        }
                        else
                        {
                            //_elevatorState = ElevatorState.Staying;
                        }

                        yield return new WaitForSeconds(LoadingTime);
                    }
                    yield return null;
                }

                if (_elevatorState != ElevatorState.Waiting && _elevatorState != ElevatorState.Stoped && _elevatorState != ElevatorState.Loading)
                {
                    if (_currentPosition > _aimFloor)
                    {
                        _currentPosition -= Speed * Time.deltaTime;
                    }
                    else
                    {
                        _currentPosition += Speed * Time.deltaTime;
                    }

                    yield return null;
                }

                yield return null;
            }
            yield return null;
        }
    }

    public void AddHuman()
    {
        PeoplesInside--;
    }

    public void RemoveHuman()
    {
        PeoplesInside++;
    }

    private int GetNextAim()
    {
        if (!HasCalls)
        {
            if (PeoplesInside == 0)
            {
                __elevatorDirection = ElevatorDirection.Down;
                return 0;
            }
            else
            {
                return CurrentFloor;
            }
        }

        List<int> avaliableFloors = new List<int>();
        for (int i = 0; i<_floors; i++)
        {
            bool stopedfromOutside = (_floorStates[i].DownPressed && __elevatorDirection == ElevatorDirection.Down) || (_floorStates[i].UpPresed && __elevatorDirection == ElevatorDirection.Up);
            bool stopedFromInside = _floorStates[i].ChoosedInElevator && ((i < _currentPosition && __elevatorDirection == ElevatorDirection.Down) ||(i > _currentPosition && __elevatorDirection == ElevatorDirection.Up));

            if (stopedFromInside || stopedfromOutside)
            {
                avaliableFloors.Add(i);
            }
        }

        if (avaliableFloors.Count == 0)
        {
            if (HasCalls)
            {
                if (__elevatorDirection == ElevatorDirection.Down)
                {
                    __elevatorDirection = ElevatorDirection.Up;
                    return GetNextAim();
                }
                if (__elevatorDirection == ElevatorDirection.Up)
                {
                    __elevatorDirection = ElevatorDirection.Down;
                    return GetNextAim();
                }
            }
        }
        else
        {
            return avaliableFloors.OrderBy(f => Mathf.Abs(f - CurrentFloor)).First();
        } 
        return 0;
    }

    public void SetState(int i, FloorState state)
    {
        if (_elevatorState == ElevatorState.Waiting && (state.ChoosedInElevator || state.UpPresed || state.DownPressed))
        {
            _elevatorState = ElevatorState.Moving;
        }

        _floorStates[i] = state;
        OnStateChanged(i, state);

        if (CallsNumber == 1 || (i<=_currentPosition && __elevatorDirection == ElevatorDirection.Down) || (i >= _currentPosition && __elevatorDirection == ElevatorDirection.Up))
        {
            _aimFloor = GetNextAim();
        }
    }

    public FloorState GetState(int i)
    {
        return _floorStates[i];
    }
}
