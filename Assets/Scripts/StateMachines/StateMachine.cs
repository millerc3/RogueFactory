using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    // Current state
    private IState currentState;

    // Are of all the transitions that exist in this state machine
    private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
    // Transitions that are currently available to us, given our current state
    private List<Transition> currentTransitions = new List<Transition>();
    // Transitions that are ALWAYS available to us, no matter our current state
    private List<Transition> anyTransitions = new List<Transition>();

    // Nice helper to just initialize an empty list of transitions
    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    public delegate void StateChanged(string stateName);
    public StateChanged OnStateChanged;

    public void Tick()
    {
        Transition transition = GetTransition();
        if (transition != null)
        {
            SetState(transition.To);
        }

        currentState?.Tick();
    }

    public void SetState(IState state)
    {
        if (state == currentState) return;

        currentState?.OnExit();
        currentState = state;

        transitions.TryGetValue(currentState.GetType(), out currentTransitions);
        if (currentTransitions == null)
        {
            currentTransitions = EmptyTransitions;
        }

        currentState.OnEnter();
        OnStateChanged?.Invoke(state.ToString());
    }

    public void AddTransition(IState fromState, IState toState, Func<bool> predicate)
    {
        // If we don't have the <fromState> stored in our transitions dict, we can't add this new transition to it
        if (!transitions.TryGetValue(fromState.GetType(), out var fromStateTransitions))
        {
            // Make a new list of transitions for us to set as the transitions stemming from <fromState>
            fromStateTransitions = new List<Transition>();
            // Add this list to the fromState index of our transitions dict
            transitions[fromState.GetType()] = fromStateTransitions;
        }

        // add this transition to the list of transitions stemming from the <fromState>
        fromStateTransitions.Add(new Transition(toState, predicate));
    }

    public void AddAnyTransition(IState toState, Func<bool> predicate)
    {
        anyTransitions.Add(new Transition(toState, predicate));
    }

    /// <summary>
    /// Given a positive Condition, what state will happen next
    /// </summary>
    private class Transition
    {
        public Func<bool> Condition { get; }
        public IState To { get; }

        public Transition(IState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    private Transition GetTransition()
    {
        foreach (Transition transition in anyTransitions)
        {
            if (transition.Condition())
            {
                return transition;
            }
        }

        foreach (Transition transition in currentTransitions)
        {
            if (transition.Condition())
            {
                return transition;
            }
        }
        return null;
    }

}
