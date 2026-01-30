using System;
using UnityEngine;


public interface IInteraction
{
    public void Begin();
    public void ForceEnd();
    public bool IsDone();
}
