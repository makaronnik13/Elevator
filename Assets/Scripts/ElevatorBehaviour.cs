
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElevatorBehaviour : MonoBehaviour
{
    #region enums
    public enum ElevatorDirection
    {
        Up,
        Down
    }
    public enum ElevatorState
    {
        Moving,
        Stoped,
        Loading
    }
    #endregion

    #region publicVariables
    public float Speed = 1f;
    public float LoadingTime = 2f;
    #endregion

    #region publicActions
    public Action<bool> OnDoorsStateChanged = (v) => { };
    public Action<int> OnPeoplesCountChanged = (i) => { };
    public Action<float> OnPositonChanged = (p) => { };
    public Action<int, FloorState> OnStateChanged = (f, s) => { };
    #endregion

    #region privateVariables
    private int _peoplesInside = 0;
    private ElevatorState __elevatorState = ElevatorState.Moving;
    private ElevatorDirection __elevatorDirection = ElevatorDirection.Up;
    private bool __doorOpen = false;
    private int _aimFloor;
    private Dictionary<int, FloorState> _floorStates = new Dictionary<int, FloorState>();
    private float __currentPosition;
    private float _arrivingTresshold = 0.01f;
    #endregion

    #region publicProperties
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
                for (int i = 0; i < Floors; i++)
                {
                    FloorState state = _floorStates[i];
                    state.ChoosedInElevator = false;
                    SetState(i, state);
                }
            }
            OnPeoplesCountChanged(_peoplesInside);
        }
    }

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
        set
        {
            if (__elevatorState != value)
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

    public int Floors
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
    #endregion

    private float _currentPosition
    {
        get
        {
            return __currentPosition;
        }
        set
        {
            if (__currentPosition != value)
            {
                __currentPosition = value;
                OnPositonChanged(__currentPosition);
            }
        }
    }

    #region publicMethods
    public void Pause()
    {
        if (_elevatorState == ElevatorState.Moving)
        {
            _elevatorState = ElevatorState.Stoped;
        }
    }

    public void Continue()
    {
        if (_elevatorState == ElevatorState.Stoped)
        {
            _elevatorState = ElevatorState.Moving;
        }       
    }

    public void SetState(int i, FloorState state)
    {
        _floorStates[i] = state;
        OnStateChanged(i, state);

        if (CallsNumber == 1 || (i <= _currentPosition && __elevatorDirection == ElevatorDirection.Down) || (i >= _currentPosition && __elevatorDirection == ElevatorDirection.Up))
        {
            _aimFloor = GetNextAim();
        }
    }

    public FloorState GetState(int i)
    {
        return _floorStates[i];
    }

    public void Launch(int floors)
    {
        _floorStates = new Dictionary<int, FloorState>();
        for (int i = 0; i < floors; i++)
        {
            _floorStates.Add(i, new FloorState());
        }
        StartCoroutine(MoveElevator(Speed));
    }

    public void AddHuman()
    {
        PeoplesInside--;
    }

    public void RemoveHuman()
    {
        PeoplesInside++;
    }
    #endregion

    #region privateMethods
    private IEnumerator ContinueMooving(float loadingTime)
    {
        yield return new WaitForSeconds(loadingTime);

        DoorOpen = false;
        _aimFloor = GetNextAim();
        _elevatorState = ElevatorState.Moving;
    }

    private IEnumerator MoveElevator(float speed)
    {
        while (true)
        {

            if (_elevatorState == ElevatorState.Moving)
            {
                if (Mathf.Abs(_currentPosition - _aimFloor) < _arrivingTresshold)
                {
                    bool hadCalls = HasCalls;

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

                    if (_elevatorState != ElevatorState.Loading && hadCalls)
                    {
                       _elevatorState = ElevatorState.Loading;
                        yield return new WaitForSeconds(LoadingTime);
                    }
                    yield return null;
                }
                else
                {
                    if (_currentPosition > _aimFloor)
                    {
                        _currentPosition -= Speed * Time.deltaTime;
                    }
                    else
                    {
                        _currentPosition += Speed * Time.deltaTime;
                    }

                }
               
                    yield return null;
            }
            yield return null;
        }
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
        for (int i = 0; i < Floors; i++)
        {
            bool stopedfromOutside = (_floorStates[i].DownPressed && __elevatorDirection == ElevatorDirection.Down) || (_floorStates[i].UpPresed && __elevatorDirection == ElevatorDirection.Up);
            bool stopedFromInside = _floorStates[i].ChoosedInElevator && ((i < _currentPosition && __elevatorDirection == ElevatorDirection.Down) || (i > _currentPosition && __elevatorDirection == ElevatorDirection.Up));

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
    #endregion
}
