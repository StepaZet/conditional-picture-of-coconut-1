using System;
using Game;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Texture2D crossCursor;
        [SerializeField] private Toggle lightingToggle;

        private void OnEnable()
        {
            SetLightToggleAccordingly();
        }

        public void PlayGame()
        {
            Cursor.SetCursor(crossCursor, new Vector2(7,7), CursorMode.ForceSoftware);
            //SceneManager.LoadScene("SampleScene")
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        public void QuitGame()
        {
            Debug.Log("QuitButtonHasBeenPressed");
            Application.Quit();
        }

        public void ChangeLighting()
        {
            GameData.IsLightOff = !lightingToggle.isOn;
        }

        public void SetLightToggleAccordingly()
        {
            lightingToggle.isOn = !GameData.IsLightOff;
        }
    }
}
