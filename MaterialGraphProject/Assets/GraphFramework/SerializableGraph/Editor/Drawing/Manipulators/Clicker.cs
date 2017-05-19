using UnityEngine;
using UnityEngine.Experimental.UIElements;
using MouseManipulator = UnityEngine.Experimental.UIElements.MouseManipulator;

using ManipulatorActivationFilter = UnityEngine.Experimental.UIElements.ManipulatorActivationFilter;
using MouseButton = UnityEngine.Experimental.UIElements.MouseButton;

namespace UnityEditor.Graphing.Drawing
{
    public enum ClickerState
    {
        Inactive,
        Active
    }

	// TODO JOCE: This is to mimic the behavior of a button. Remove and replace with actual button in TitleBar.
    public class Clicker : MouseManipulator
    {
        public delegate void StateChangeCallback(ClickerState newState);
        public delegate void ClickCallback();

        public StateChangeCallback onStateChange { get; set; }
        public ClickCallback onClick { get; set; }

        VisualElement initialTarget;
        ClickerState state;

		public Clicker()
		{
			activators.Add(new ManipulatorActivationFilter {button = MouseButton.LeftMouse});
		}

		public override EventPropagation HandleEvent(Event evt, VisualElement finalTarget)
        {
            switch (evt.type)
            {
                case EventType.MouseDown:
                    if (CanStartManipulation(evt))
                    {
                        this.TakeCapture();
                        initialTarget = finalTarget;
                        UpdateState(evt);
                    }
                    break;

                case EventType.mouseDrag:
                    UpdateState(evt);
                    break;

                case EventType.MouseUp:
                    if (CanStopManipulation(evt))
                    {
                        this.ReleaseCapture();
                        // withinInitialTarget = initialTarget != null && initialTarget.ContainsPoint(evt.mousePosition);
                        if (initialTarget != null && state == ClickerState.Active && onClick != null)
                            onClick();
                        initialTarget = null;
                        UpdateState(evt);
                    }
                    break;
            }
            return this.HasCapture() ? EventPropagation.Stop : EventPropagation.Continue;
        }

        void UpdateState(Event evt)
        {
            ClickerState newState;
            if (initialTarget != null && initialTarget.ContainsPoint(evt.mousePosition))
                newState = ClickerState.Active;
            else
                newState = ClickerState.Inactive;

            if (onStateChange != null && state != newState)
                onStateChange(newState);
            state = newState;
        }
    }
}
