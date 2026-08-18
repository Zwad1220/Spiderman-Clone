using System;
using UnityEngine; 

public class MatchTimer 
{
    public float CurrentTime { get; private set; }
    public float TotalTime { get; }
    public bool IsRunning { get; private set; }
    public bool IsComplete => CurrentTime >= TotalTime;

    public event Action<float> OnTimeChanged;
    public event Action OnTimerComplete;

    public MatchTimer(float totalTime, bool startRunning = true)
    {
        TotalTime = totalTime;
        IsRunning = startRunning;
    }

    public void Tick(float deltaTime)
    {
        if (!IsRunning) return;

        CurrentTime = Mathf.Min(CurrentTime + deltaTime, TotalTime);
        OnTimeChanged?.Invoke(CurrentTime);

        if (IsComplete)
        {
            IsRunning = false;
            OnTimerComplete?.Invoke();
        }
    }

    public void Pause() => IsRunning = false;
    public void Resume(){ 
        if (!IsComplete) IsRunning = true; 
    }
    public void Reset()
    {
        CurrentTime = 0f;
        OnTimeChanged?.Invoke(CurrentTime);
    }
    public void Restart()
    {
        CurrentTime = 0f;
        IsRunning = true;
        OnTimeChanged?.Invoke(CurrentTime);
    }
}