using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Texture2D crossCursor;
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
    }
}
