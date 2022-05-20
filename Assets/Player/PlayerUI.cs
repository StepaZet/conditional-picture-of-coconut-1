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
			openCharactersImages.Clear();

			var yNextPosition = 0f;
			foreach (var character in player.unlockedCharacters)
			{
				if (character == player.character)
				{
					var outlineImageObj = new GameObject("CharacterImage");
					
					var rectTransform1 = outlineImageObj.AddComponent<RectTransform>();
					rectTransform1.transform.SetParent(openCharactersCanvas.transform);
					rectTransform1.localScale = Vector3.one;
					rectTransform1.anchoredPosition = new Vector2(0, yNextPosition);
					rectTransform1.sizeDelta= new Vector2(110, 110);
					
					var outlineImage = outlineImageObj.AddComponent<Image>();
					outlineImage.sprite = character.sprite.sprite;
					outlineImage.material = OutlineMaterial;
					outlineImage.transform.SetParent(openCharactersCanvas.transform);

					openCharactersImages.Add(outlineImage);
				}
				
				
				var characterImageObj = new GameObject("CharacterImage");

				var rectTransform = characterImageObj.AddComponent<RectTransform>();
				rectTransform.transform.SetParent(openCharactersCanvas.transform);
				rectTransform.localScale = Vector3.one;
				rectTransform.anchoredPosition = new Vector2(0, yNextPosition);
				rectTransform.sizeDelta= new Vector2(100, 100);
				yNextPosition -= rectTransform.rect.height;

				var image = characterImageObj.AddComponent<Image>();
				image.sprite = character.sprite.sprite;
				characterImageObj.transform.SetParent(openCharactersCanvas.transform);

				openCharactersImages.Add(image);
			}
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