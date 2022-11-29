using System;
using P106.Main.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace P106.Main.Pollutant
{
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class PollutantBase : MonoBehaviour
    {
        [SerializeField] protected PollutantData pollutantData;
        [SerializeField] protected bool playerInVicinity;

        [Space, Header("INPUT")]
        [SerializeField] protected bool hasLeftMouseButtonInput;

        protected BoxCollider2D objectCollider;
        protected Camera mainCam;
        protected Vector2 dragVelocity = Vector2.zero;

        public bool PlayerInVicinity { set => playerInVicinity = value; }

        public abstract PollutantType PollutantType { get; }

        public static event Action<PollutantType> OnPollutantCollected;

        void Awake()
        {
            objectCollider = GetComponent<BoxCollider2D>();
            mainCam = Camera.main;
            hasLeftMouseButtonInput = false;
        }

        void OnEnable()
        {
            InputReader.LeftMouseInputEvent += OnLeftMouseInput;
            InputReader.MouseDragEvent += OnMouseDragInput;
        }

        void OnMouseDragInput(Vector2 mouseDelta, float smoothSpeed, float maxSmoothSpeed)
        {
            Vector2 worldMouse2DPos = InputReader.GetScreenToWorldMousePosition();

            if (!hasLeftMouseButtonInput || !objectCollider.bounds.Contains(worldMouse2DPos)) return;

            Vector3 pos = transform.position;
            Vector2 pos2D = new(pos.x, pos.y);

            float dist = Vector2.Distance(pos2D, worldMouse2DPos);
            bool approxZeroDist = Mathf.Approximately(0f, dist);

            while (hasLeftMouseButtonInput && !approxZeroDist)
            {
                Vector2 smoothPos = Vector2.SmoothDamp(
                    current: pos2D,
                    target: worldMouse2DPos,
                    currentVelocity: ref dragVelocity,
                    smoothTime: smoothSpeed,
                    maxSpeed: maxSmoothSpeed);

                (pos.x, pos.y, pos.x) = (smoothPos.x, smoothPos.y, 0f);
                transform.position = pos;
                break;
            }
        }

        void OnDisable()
        {
            InputReader.LeftMouseInputEvent -= OnLeftMouseInput;
            InputReader.MouseDragEvent -= OnMouseDragInput;
        }

        void OnLeftMouseInput(bool hasInput) => hasLeftMouseButtonInput = hasInput;

        void OnMouseDown()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame) return;
            CollectPollutant();
        }

        protected abstract void CollectPollutant();

        protected void RaiseCollectPollutant(PollutantType pollutant)
        {
            OnPollutantCollected?.Invoke(pollutant);
        }
    }
}