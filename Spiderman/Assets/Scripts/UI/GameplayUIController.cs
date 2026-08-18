using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] UIDocument document;
    [SerializeField] float TotalTime;
    MatchTimer matchTimer;
    IView timerView;

    void Start(){
        var root = document.rootVisualElement;
        matchTimer = new MatchTimer(TotalTime); // Timer for the player display

        var timeLabel = root.Q<Label>("time-label");
        timerView = new TimerView(timeLabel, matchTimer);
    }
}
