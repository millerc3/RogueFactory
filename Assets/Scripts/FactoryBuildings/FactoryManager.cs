using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryManager : Singleton<FactoryManager>
{
    [SerializeField] private float TICK_TIME_MAX = .2f;
    private float tickTimer;
    private ulong tick;

    public delegate void FactoryTick();
    public FactoryTick OnFactoryTick;

    protected override void Awake()
    {
        base.Awake();

        tick = 0;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;
        if (tickTimer >= TICK_TIME_MAX)
        {
            tickTimer -= TICK_TIME_MAX;
            tick++;
            OnFactoryTick?.Invoke();
        }
    }

    public float GetTimePerTick()
    {
        return TICK_TIME_MAX;
    }
}
