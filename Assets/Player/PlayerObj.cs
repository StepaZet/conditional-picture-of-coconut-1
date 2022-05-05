using System;
using System.ComponentModel.Design;
using UnityEngine;
using Game;

namespace Player
{
    public class PlayerObj : MonoBehaviour
    {
        public Character character;
        private PlayerController controller;
        public readonly PlayerInput input = new PlayerInput();
        public Collider2D collider;

        [SerializeField] private LayerMask dashLayerMask;

        private void Awake()
        {
            GameData.Players.Add(this);
            GameData.player = this; //Временно
            controller = new PlayerController(dashLayerMask);
        }

        private void Update()
        {
            transform.position = character.transform.position;
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.GetComponent<Rigidbody2D>()) 
                other.gameObject.GetComponent<Rigidbody2D>().WakeUp();
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            //Physics2D.IgnoreCollision(character.characterCollider, GetComponent<Collider2D>());
            if (!other.GetComponent<Character>() || other == collider)
                return;

            if (input.IsChangeCharacter)
            {
                var weaponPosition = character.weapon.transform.localPosition;
                var weaponRotation = character.weapon.transform.localRotation;
                character = other.GetComponent<Character>();
                character.weapon.transform.localPosition = weaponPosition;
                character.weapon.transform.localRotation = weaponRotation;
            }
                
        }
    }
}
