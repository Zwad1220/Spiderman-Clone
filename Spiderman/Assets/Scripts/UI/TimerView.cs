using UnityEngine;
using UnityEngine.UIElements;

public class TimerView : IView
{
    readonly Label _timerLabel;
    readonly MatchTimer _timer;
    public TimerView(Label label, MatchTimer timer){
        _timerLabel = label;
        _timer = timer;
        _timer.OnTimeChanged += HandleTimeChanged;

        HandleTimeChanged(_timer.CurrentTime); 
    }

    void HandleTimeChanged(float currentTime)
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        _timerLabel.text = $"{minutes:00}:{seconds:00}";
    }
    public void Clear()=> _timer.OnTimeChanged -= HandleTimeChanged;
}
