using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Game;
using Health;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player
{
	public class PlayerUI : MonoBehaviour
	{
		[SerializeField]private Text ammoText;
		[SerializeField]private Text scoreText;
		[SerializeField]private PlayerObj player;
		[SerializeField]private HealthBar healthBar;
		[SerializeField]private HealthBar staminaBar;
		[SerializeField]private Canvas openCharactersCanvas;
		[SerializeField]private List<Image> openCharactersImages = new List<Image>();
		[SerializeField]private GameObject smallVerticalHealthBarPrefab;
		[SerializeField]private List<HealthBar> smallHealthBars = new List<HealthBar>();
		[SerializeField]private Image WeaponImage;
		private void UpdateAmmoText(object sender, System.EventArgs eventArgs)
		{
			var weapon = player.character.weapon;
			ammoText.text = $"{weapon.CurrentAmmoAmount}/{weapon.MaxAmmoAmount}";
		}

		private void UpdateScoresText(object sender, System.EventArgs eventArgs)
		{
			scoreText.text = GameData.Score.ToString();
		}

		private void ChangeBars(object sender, System.EventArgs eventArgs)
		{
			healthBar.ChangeHealthObj(player.character.health);
			staminaBar.ChangeHealthObj(player.character.stamina);
		}

		private void FixedUpdate()
		{
			foreach (var smallHealthBar in smallHealthBars) 
				smallHealthBar.UpdateBar();
			healthBar.UpdateBar();	//Иначе не обновляет при первой смене персонажа
			staminaBar.UpdateBar();
		}

		private void UpdateCharacters(object sender, System.EventArgs eventArgs)
		{
			foreach (var image in openCharactersImages) 
				Destroy(image.gameObject);
			foreach (var image in smallHealthBars.Select(smallHealthBar => smallHealthBar.GetComponents<Image>()).SelectMany(images => images))
				Destroy(image);
			
			openCharactersImages.Clear();
			smallHealthBars.Clear();

			var yNextPosition = 0f;
			var backgroundColor = new Color(0, 0, 0, 0.5f);
			foreach (var character in player.unlockedCharacters)
			{
				var smallHealthBarBackgroundImage = CreateImage(150, yNextPosition, 150, 150, "SmallHealthBarBackgroundImage");
				smallHealthBarBackgroundImage.color = backgroundColor;
				openCharactersImages.Add(smallHealthBarBackgroundImage);
				
				var smallHealthBar = CreateSmallHealthBar(character, yNextPosition);
				smallHealthBars.Add(smallHealthBar);
				

				var backgroundImage = CreateImage(0, yNextPosition, 150, 150, "BackgroundImage");
				backgroundImage.color = backgroundColor;
				openCharactersImages.Add(backgroundImage);
				
				var characterImage = CreateImage(character, yNextPosition);
				openCharactersImages.Add(characterImage);
				
				if (character != player.character)
				{
					var shadeImage = CreateImage(character, yNextPosition);
					shadeImage.color = backgroundColor;
					openCharactersImages.Add(shadeImage);
				}

				var offset = 0;
				yNextPosition -= 150 + offset;
			}
		}

		private HealthBar CreateSmallHealthBar(Character character, float yNextPosition)
		{
			var smallHealthBarGameObject = Instantiate(smallVerticalHealthBarPrefab); 
			var smallHealthBar = smallHealthBarGameObject.GetComponent<HealthBar>();
			smallHealthBar.UpdateBar();
			smallHealthBar.transform.SetParent(openCharactersCanvas.transform);
			smallHealthBar.transform.localPosition = new Vector3(100, yNextPosition);
			smallHealthBar.SetUp(character.health);

			return smallHealthBar;
		}

		private Image CreateImage(float x, float y, float width, float height, string imageName)
		{
			var imageObj = new GameObject(imageName);

			var rectTransform = imageObj.AddComponent<RectTransform>();
			rectTransform.transform.SetParent(openCharactersCanvas.transform);
			rectTransform.localScale = Vector3.one;
			rectTransform.anchoredPosition = new Vector2(x, y);
			rectTransform.sizeDelta= new Vector2(width, height);

			var image = imageObj.AddComponent<Image>();
			imageObj.transform.SetParent(openCharactersCanvas.transform);
			
			return image;
		}

		private Image CreateImage(Character character, float yNextPosition)
		{
			var sprite = character.sprite.sprite;
			var image = CreateImage(0, yNextPosition, sprite.textureRect.width * 0.8f, sprite.textureRect.height * 0.8f,
				"CharacterImage");

			image.sprite = character.sprite.sprite;

			return image;
		}

		private void UpdateWeaponImage(object sender, System.EventArgs eventArgs)
		{
			if (player.character.weapon.BulletTypeSprite == null)
				return;
			WeaponImage.sprite = player.character.weapon.BulletTypeSprite;
		}

		public void Start()
		{
			UpdateCharacters(this, EventArgs.Empty);
			UpdateScoresText(this, EventArgs.Empty);
			UpdateAmmoText(this, EventArgs.Empty);
			Character.OnDeath += UpdateCharacters;
			player.OnCharacterChange += UpdateCharacters;
			player.OnCharacterChange += ChangeBars;
			player.OnCharacterChange += UpdateAmmoText;
			player.OnCharacterChange += UpdateWeaponImage;
			Weapon.Weapon.OnAmmoChanged += UpdateAmmoText;
			Chest.OnScoreChanged += UpdateScoresText;
			healthBar.SetUp(player.character.health);
			staminaBar.SetUp(player.character.stamina);
		}
	}
}