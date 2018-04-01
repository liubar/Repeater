using App;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ScoreMenu : MonoBehaviour
    {
        public IPlayer Player;
        public Text Score;

        public void Active()
        {
            Score.text = string.Format("{0} Фигур", Player.Score);
            gameObject.SetActive(true);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
