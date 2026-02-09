using System;
using System.Collections.Generic;

public class PlayerInput
{
    private bool[] _inputStates;
    private short[] _inputValues;

    public PlayerInput()
    {
        _inputStates = new bool[(int)EInputType.Count];
        _inputValues = new short[(int)EInputType.Count];
    }

    public void ReadFromPlayer(GamePlayer player) { }

    public void AddInput(InputInfo info)
    {
        _inputStates[(int)info.InputType] = true;
        _inputValues[(int)info.InputType] = info.Value;
    }

    public bool HasInput(EInputType inputType)
    {
        return _inputStates[(int)inputType];
    }

    public short GetValue(EInputType inputType)
    {
        return _inputValues[(int)inputType];
    }

    public void Clear()
    {
        Array.Clear(_inputStates, 0, _inputStates.Length);
        Array.Clear(_inputValues, 0, _inputValues.Length);
    }
}
