using UnityEngine;

namespace CodeBase.Services.Input
{
    public abstract class InputService : IInputService
    {
        protected const string Horizontal = "Horizontal";
        protected const string Vertical = "Vertical";
        protected const string Fire = "Fire";
        protected const string Save = "Save";
        protected const string Load = "Load";

        public abstract Vector2 Axis { get; }

        public bool IsAttackButtonUp() => 
            SimpleInput.GetButtonUp(Fire);

        public bool IsSaveButtonUp() => 
            SimpleInput.GetButtonUp(Save);

        public bool IsLoadButtonUp() => 
            SimpleInput.GetButtonUp(Load);

        protected static Vector2 SimpleInputAxis() =>
            new Vector2(SimpleInput.GetAxis(Horizontal), SimpleInput.GetAxis(Vertical));

    }
}
