using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ModestTree;


namespace Savana.Movie
{

    public class UI_HomePage : MonoBehaviour, UIState
    {
        private UIController _controller;
        [SerializeField] private UI_NowTrending nowTrendingPanel; //Displays a list of trending movies
        [SerializeField] private UI_SearchGroup searchPanel; //Encapsulates a movie search functionality

        [Header("Searching")]
        [SerializeField] private TMP_InputField searchInput;
        [SerializeField] private Button searchBtn;



        public void AttachTo(UIController controller)
        {
            _controller = controller;
            _controller.RegisterState(this);

            gameObject.SetActive(false);

            nowTrendingPanel.AttachTo(controller);
            nowTrendingPanel.AttachTo(controller);

        }

        public void Enter()
        {
            gameObject.SetActive(true);
            searchBtn.onClick.AddListener(SearchClicked);

            if (!searchPanel.gameObject.activeSelf)
            {
                nowTrendingPanel.Show(_controller);
            }


        }

        public void Exit()
        {
            searchBtn.onClick.RemoveListener(SearchClicked);
            nowTrendingPanel.Hide();

            gameObject.SetActive(false);
        }

        public void Pause() { }
        public void Resume() { }

        private void SearchClicked()
        {
            string query = searchInput.text.ToLower();
            if (query.IsEmpty())
            {
                return;
            }
            nowTrendingPanel.Hide();
            searchPanel.Show();
            searchPanel.Search(query, () => nowTrendingPanel.Show(_controller), SuccesfulSearch, FailedSearch, _controller);
        }

        private void SuccesfulSearch()
        {
            searchPanel.Show();
            nowTrendingPanel.Hide();
        }

        private void FailedSearch()
        {
            searchPanel.searchLoadingBar.SetActive(false);
        }
    }

}