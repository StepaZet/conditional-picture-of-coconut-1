using System;
using Health;
using Unity.Mathematics;
using UnityEngine;
using Weapon;

namespace Player
{
	public class Character : MonoBehaviour
	{
		//public GameObject healthObjPrefab;
		public Weapon.Weapon weapon;
		public HealthObj health;
		public StaminaObj stamina;
		[SerializeField] private int maxHealth;
		public Rigidbody2D rb;
		public SpriteRenderer sprite;
		[SerializeField] protected GameObject weaponPrefab;
		[SerializeField] public Collider2D characterCollider;
		public Sprite BulletTypeSprite;
        
        public GameObject StartShadow;
        private bool isStartShadowSpawned;

		public PlayerState State { get; set; }
		public static event EventHandler OnDeath;

        private void OnEnable()
        {
            if (isStartShadowSpawned)
				return;
            isStartShadowSpawned = true;
            Instantiate(StartShadow, 
                transform.position - 
                new Vector3(0, sprite.size.y/2), 
                Quaternion.identity);
        }

		private void Awake()
		{
			health.maxHealthPoints = maxHealth;
			//health = Instantiate(healthObjPrefab, transform).GetComponent<HealthObj>();
			State = PlayerState.Normal;
			sprite = rb.GetComponent<SpriteRenderer>();
			grid = GameObject.Find("GridActualUnity").GetComponent<GridObj>();
			health.OnDeath += Die;
		}
		public void Update()
		{
			if (health == null)
				return;
			if (health.CurrentHealthPoints <= 0) 
				Die(this, EventArgs.Empty);
        }

		private void FixedUpdate()
		{
			//UpdateGrid();
		}

		private void Die(object sender, System.EventArgs eventArgs)
		{
			State = PlayerState.Dead;
			Game.GameData.player.unlockedCharacters.Remove(this);
			OnDeath?.Invoke(this, EventArgs.Empty);
		}
        
		private void UpdateEyeDirection()
		{
			sprite.flipX = (int) Mathf.Sign(-rb.velocity.x) == 1;
		}
		
		
		public GridObj grid;
		//public int2 gridPosition;


		//public void UpdateGrid()
		//{
		//	var newGridPosition = grid.WorldToGridPosition(transform.position);
		//	if (newGridPosition.x == gridPosition.x && newGridPosition.y == gridPosition.y) return;
		//	//grid.UnFillCell(gridPosition);
		//	//grid.FillCell(newGridPosition);
		//	gridPosition = newGridPosition;
		//	grid.PlayerPosition = transform.position;
		//}

	}
}
