using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZBoom.Common.SpatialMap
{
    public class ObjectAR : MonoBehaviour
    {
        public enum State
        {
            IDLE,
            EDIT,
            PREVIEW
        }

        public Action OnIdle;
        public Action OnEdit;
        public Action OnPreview;

        public State CurrentState = State.IDLE;
        
        private void Start()
        {
        }

        private void Update()
        {
        }

        public void SetState(State state)
        {
            CurrentState = state;
            switch (CurrentState)
            {
                case State.IDLE:
                    OnIdle?.Invoke();
                    break;
                case State.EDIT:
                    OnEdit?.Invoke();
                    break;
                case State.PREVIEW:
                    OnPreview?.Invoke();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}