using UnityEngine;

public interface IUpdateableView : IView
{
    void Tick(float deltaTime);
}
