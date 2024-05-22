using System;
namespace NetworkedRigidbody
{
    public interface IStateMachine
    {
        void Init();
        void Tick();
        void TryInterrupt(Action doOnInterruptable);
        void TryInterrupt<T>(Action<T> doOnInterruptable, T param);
        void TryInterrupt<T1, T2>(Action<T1, T2> doOnInterruptable, T1 param1, T2 param2);
    }
}