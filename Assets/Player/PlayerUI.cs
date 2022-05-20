using System;
using System.Collections.Generic;
using Health;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Player
{
	public class PlayerUI : MonoBehaviour
	{
		[SerializeField]private Text ammoText;
		[SerializeField]private PlayerObj player;
		[SerializeField]private HealthBar healthBar;
		[SerializeField]private Canvas openCharactersCanvas;
		[SerializeField]private List<Image> openCharactersImages = new List<Image>();
		[SerializeField] private Material OutlineMaterial;
		private Image outlinedCharacterImage;

		private void UpdateAmmoText(object sender, System.EventArgs eventArgs)
		{
			var weapon = player.character.weapon;
			ammoText.text = $"{weapon.CurrentAmmoAmount}/{weapon.MaxAmmoAmount}";
		}

		private void ChangeHealthBar(object sender, System.EventArgs eventArgs)
		{
			healthBar.ChangeHealthObj(player.character.health);
		}

		private void UpdateCharacters(object sender, System.EventArgs eventArgs)
		{
			foreach (var image in openCharactersImages) 
				Destroy(image.gameObject);
			
			Destroy(outlinedCharacterImage);
			openCharactersImages.Clear();

			var yNextPosition = 0f;
			foreach (var character in player.unlockedCharacters)
			{
				var characterImage = CreateImage(character, yNextPosition);
				openCharactersImages.Add(characterImage);
				
				if (character != player.character)
				{
					var shadeImage = CreateImage(character, yNextPosition);
					shadeImage.color = new Color(0, 0, 0, 0.5f);
					openCharactersImages.Add(shadeImage);
				}

				var offset = characterImage.sprite.rect.height * 0.2f;
				yNextPosition -= (characterImage.sprite.rect.height + offset) * 0.5f;
			}
		}

		private Image CreateImage(Character character, float yNextPosition)
		{
			var characterImageObj = new GameObject("CharacterImage");

			var rectTransform = characterImageObj.AddComponent<RectTransform>();
			rectTransform.transform.SetParent(openCharactersCanvas.transform);
			rectTransform.localScale = Vector3.one;
			rectTransform.anchoredPosition = new Vector2(0, yNextPosition);
			var sprite = character.sprite.sprite;
			rectTransform.sizeDelta= new Vector2(sprite.textureRect.width * 0.5f, sprite.textureRect.height * 0.5f);

			var image = characterImageObj.AddComponent<Image>();
			image.sprite = character.sprite.sprite;
			characterImageObj.transform.SetParent(openCharactersCanvas.transform);

			return image;
		}
		

		public void Awake()
		{
			healthBar.SetUp(player.character.health);
			Character.OnDeath += UpdateCharacters;
			player.OnCharacterChange += UpdateCharacters;
			player.OnCharacterChange += ChangeHealthBar;
			player.OnCharacterChange += UpdateAmmoText;
			Weapon.Weapon.OnAmmoChanged += UpdateAmmoText;
		}
	}
}