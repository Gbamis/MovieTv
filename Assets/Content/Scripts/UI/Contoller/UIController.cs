using UnityEngine;
using System.Collections.Generic;
using System;

namespace Savana.Movie
{
    public interface UIState
    {
        public void Enter();
        public void Exit();
        public void Pause();
        public void Resume();
    }

    /// <summary>
    /// This class handles transition across screens using a state pattern
    /// </summary>
    public class UIController 
    {
        private UIState _currentMainState;
        private Stack<UIState> _currentSubStates = new();
        private Dictionary<Type, UIState> states = new();

        public void RegisterState<T>(T state) where T : UIState => states[typeof(T)] = state;

        public void ChangeState<T>() where T : UIState
        {
            if (states.TryGetValue(typeof(T), out UIState newState))
            {
                _currentMainState?.Exit();
                _currentMainState = newState;
                _currentMainState.Enter();
            }
        }
        public void ChangeSubState<T>() where T : UIState
        {
            if (states.TryGetValue(typeof(T), out UIState newState))
            {
                _currentMainState.Pause();
                _currentSubStates.Push(_currentMainState);
                _currentMainState = newState;
                _currentMainState.Enter();
            }
        }
        public void ExitSubState<T>() where T : UIState
        {
            if (states.TryGetValue(typeof(T), out UIState newState))
            {
                _currentMainState.Exit();
                _currentMainState = _currentSubStates.Pop();
                _currentMainState.Enter();
            }
        }

    }

}