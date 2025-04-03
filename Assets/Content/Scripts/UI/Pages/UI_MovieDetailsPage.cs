using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Text;
using UnityEngine.EventSystems;

namespace Savana.Movie
{
    public class UI_MovieDetailsPage : MonoBehaviour, UIState
    {
        private UIController _controller;
        [SerializeField] private bool showMenu; // should menubar be active on this page
        [SerializeField] private GameObject menuBar; // menubar group
        [SerializeField] private Button backBtn;

        [SerializeField] private Image poster; //display movie poster
        [SerializeField] private TextMeshProUGUI titleTxt; //display movie title
        [SerializeField] private TextMeshProUGUI genreTxt;
        [SerializeField] private TextMeshProUGUI releaseDataTxt;
        [SerializeField] private TextMeshProUGUI overviewTxt;
        [SerializeField] private TextMeshProUGUI voteAvgTxt;

        [SerializeField] private CanvasGroup favButtonAlpha;
        [SerializeField] private CanvasGroup favButtonIcon;

         public EventSystem eventSystem;

        public void AttachTo(UIController controller)
        {
            _controller = controller;
            _controller.RegisterState(this);
            showMenu = false;
            gameObject.SetActive(false);
        }

        public void Enter()
        {
            backBtn.onClick.AddListener(HomePage);

            menuBar.SetActive(showMenu);
            gameObject.SetActive(true);

            Fade().Forget(); // UI Entry animations
            eventSystem.SetSelectedGameObject(backBtn.gameObject);
        }
        public void Exit()
        {
            backBtn.onClick.RemoveListener(HomePage);
            gameObject.SetActive(false);
        }

        public void Pause() { }
        public void Resume() { }

        private void HomePage() => _controller.ChangeState<UI_HomePage>();

        public void SetData(Sprite img, Response_MovieDetail result)
        {
            poster.sprite = img;
            titleTxt.text = result.title;
            releaseDataTxt.text = result.release_date;
            overviewTxt.text = result.overview;
            voteAvgTxt.text = result.vote_average.ToString();

            StringBuilder sb = new();

            foreach (Model_Genre mg in result.genres)
            {
                sb.Append(mg.name);
                sb.Append(" ");
            }
            genreTxt.text = sb.ToString();
        }

        private async UniTaskVoid Fade()
        {
            favButtonAlpha.alpha = 0;
            favButtonIcon.alpha = 0;

            float step = 0;
            while (step < 1)
            {
                step += Time.deltaTime * 2;
                favButtonAlpha.alpha = step;
                favButtonIcon.alpha = step;
                await UniTask.Yield();
            }
        }
    }

}