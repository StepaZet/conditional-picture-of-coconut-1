using System;
using UnityEngine;
using Game;

namespace Player
{
    public class PlayerObj : MonoBehaviour
    {
        public Character character;
        private PlayerController controller;
        public readonly PlayerInput input = new PlayerInput();

        [SerializeField] private LayerMask dashLayerMask;

        private void Awake()
        {
            GameData.Players.Add(this);
            GameData.player = this; //Временно
            controller = new PlayerController(dashLayerMask);
        }

        private void Update()
        {
            input.Update();
            controller.Update(this);
        }

        private void FixedUpdate()
        {
            controller.FixedUpdate(this);
        }
        
        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}
