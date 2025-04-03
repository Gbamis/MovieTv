using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Savana.Movie
{
    public class UI_NowTrending : MonoBehaviour
    {
        [SerializeField] private bool isFetching;
        private int currentPage = 1;
        private UIController _controller;
        private GameObject lastSelectedObject;

        public EventSystem eventSystem;

        [SerializeField] private UI_MovieDetailsPage movieDetailsPage;
        [SerializeField] private bool showMenu;
        [SerializeField] private GameObject headerBar;
        [SerializeField] private GameObject menuBar;
        [SerializeField] private GameObject titleText;
        [SerializeField] private GameObject updatingProgress;
        [SerializeField] private TextMeshProUGUI contentHeader;

        [Header("Optimized")]
        public RecycleScrollRect recycleScrollRect;


        public void AttachTo(UIController controller) => _controller = controller;

        public void Show(UIController con)
        {
            _controller = con;

            gameObject.SetActive(true);
            menuBar.SetActive(showMenu);
            titleText.SetActive(true);
            headerBar.SetActive(true);
            OnNewContentDownloadCompleted();

            recycleScrollRect.CreatePool(NetworkManager.response_NowPlaying,
             movieDetailsPage, OnCardClicked, OnFetchMoreContent);

            contentHeader.text = "Now Playing (" + NetworkManager.response_NowPlaying.Count + ")";

            lastSelectedObject = lastSelectedObject == null ? recycleScrollRect.Selected : lastSelectedObject;
            eventSystem.SetSelectedGameObject(lastSelectedObject);
        }

        public void Hide() => gameObject.SetActive(false);
        //private void OnCardClicked() => _controller.ChangeState<UI_MovieDetailsPage>();
        private void OnCardClicked(GameObject button)
        {
            _controller.ChangeState<UI_MovieDetailsPage>();
            lastSelectedObject = button;
        }

        private void OnFetchMoreContent(int scrollIndex)
        {
            int count = NetworkManager.response_NowPlaying.Count;
            if (scrollIndex >= count - 5 && !isFetching)
            {
                string key = PlayerPrefs.GetString("TM_KEY");
                currentPage++;
                updatingProgress.SetActive(true);
                NetworkManager.Request_Get_NowPlaying(key, OnNewContentDownloadCompleted, OnNewContentDownloadCompleted, currentPage);
                isFetching = true;
                Debug.Log("fetching" + scrollIndex);
            }

        }
        private void OnNewContentDownloadCompleted()
        {
            updatingProgress.SetActive(false);
            isFetching = false;
            contentHeader.text = "Now Playing (" + NetworkManager.response_NowPlaying.Count + ")";
        }

    }
}
