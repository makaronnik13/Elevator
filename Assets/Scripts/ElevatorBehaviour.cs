
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElevatorBehaviour: MonoBehaviour
{
    private enum ElevatorState
    {
        MovingUp,
        MovingDown,
        Staying,
        Stoped,
        Loading
    }

    private ElevatorState _elevatorState = ElevatorState.Staying;

    public float Speed = 1f;
    public float LoadingTime = 2f;
    private Dictionary<int, FloorState> _floorStates = new Dictionary<int, FloorState>();
    private int _aimFloor;
    private float __currentPosition;
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
            _aimFloor = GetNextAim();

            Debug.Log(_aimFloor);

            float _lastFloor = CurrentFloor;
            yield return StartCoroutine(MoveElevator(_aimFloor));

            FloorState state = _floorStates[CurrentFloor];

            state.ChoosedInElevator = false;
            if (CurrentFloor<_lastFloor)
            {
                state.DownPressed = false;
            }
            else
            {
                state.UpPresed = false;
            }
            SetState(CurrentFloor, state);

            yield return new WaitForSeconds(LoadingTime);
        }
    }

    private IEnumerator MoveElevator(int aim)
    {
        float t = Mathf.Abs(_currentPosition - aim);
        float pathTime = t;
        float from = _currentPosition;
        while (_elevatorState!= ElevatorState.Staying && _elevatorState!= ElevatorState.Stoped && _elevatorState!= ElevatorState.Loading && !Mathf.Approximately(_currentPosition, aim))
        {
            _currentPosition = Mathf.Lerp(aim, from, t/pathTime);
            t -= Time.deltaTime*Speed;
            yield return null;
        }
    }

    private int GetNextAim()
    {
        if (_elevatorState == ElevatorState.MovingUp || _elevatorState == ElevatorState.Staying)
        {
            IEnumerable<int> upperFloors = _floorStates.Keys.Where(f => (_floorStates[f].ChoosedInElevator  || _floorStates[f].UpPresed) && f>=CurrentFloor);

            if (upperFloors.Count() != 0)
            {
                if (_elevatorState == ElevatorState.Staying)
                {
                    _elevatorState = ElevatorState.MovingUp;
                }
                return upperFloors.OrderBy(f => f).First();      
            }
        }

        if (_elevatorState == ElevatorState.MovingDown || _elevatorState == ElevatorState.Staying)
        {
            IEnumerable<int> downFloors = _floorStates.Keys.Where(f =>( _floorStates[f].ChoosedInElevator || _floorStates[f].DownPressed) && f <= CurrentFloor);
            if (downFloors.Count() != 0)
            {
                if (_elevatorState == ElevatorState.Staying)
                {
                    _elevatorState = ElevatorState.MovingDown;
                }
                return downFloors.OrderBy(f => f).Last();
            }
        }

        return Mathf.Max(0, CurrentFloor-1);
    }

    public void SetState(int i, FloorState state)
    {
        _floorStates[i] = state;
        OnStateChanged(i, state);
    }

    public FloorState GetState(int i)
    {
        return _floorStates[i];
    }
}
