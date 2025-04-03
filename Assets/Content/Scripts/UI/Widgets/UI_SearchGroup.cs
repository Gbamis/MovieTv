using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

namespace Savana.Movie
{
    public class UI_SearchGroup : MonoBehaviour
    {
        private UIController _controller;
        private Action success;
        private Action error;
        private Action closed;


        [SerializeField] private Transform listView;
        [SerializeField] private UI_MovieCard_Item movieCard_Item;
        [SerializeField] private UI_MovieDetailsPage movieDetailsPage;
        [SerializeField] private TextMeshProUGUI resultCountText;
        [SerializeField] private int maxViewAtOnce;
        [SerializeField] private Button backBtn;

        public GameObject searchLoadingBar;

        [SerializeField] private bool showMenu;
        [SerializeField] private GameObject menuBar;
        [SerializeField] private GameObject headerBar;

        [SerializeField] private GameObject titleText;


        public void Show()
        {
            gameObject.SetActive(true);
            titleText.SetActive(false);
            menuBar.SetActive(showMenu);
            backBtn.gameObject.SetActive(true);
            backBtn.onClick.AddListener(Hide);
            headerBar.SetActive(false);
        }
        public void Hide()
        {
            searchLoadingBar.SetActive(false);

            backBtn.onClick.RemoveListener(Hide);
            closed?.Invoke();
            gameObject.SetActive(false);
        }


        public void Search(string query, Action OnClosed, Action OnSuccess, Action OnError, UIController con)
        {
            success = OnSuccess;
            error = OnError;
            closed = OnClosed;

            _controller = con;
            ClearListView();
            Show();
            searchLoadingBar.SetActive(true);

            NetworkManager.Request_SearchforMovie_ByKeyword(query, PopulateListViewWithMovieItems, OnErrorOccured);
        }

        public void PopulateListViewWithMovieItems(List<Response_MovieDetail> data)
        {
            searchLoadingBar.SetActive(false);

            resultCountText.text = data.Count == 0 ? "No results found" : "Showing (" + data.Count.ToString() + ") Results";
            if (data.Count > 0)
            {
                for (int i = 0; i < Mathf.Min(maxViewAtOnce, data.Count); i++)
                {
                    UI_MovieCard_Item item = Instantiate(movieCard_Item, listView);
                    item.SetData(data[i], movieDetailsPage, OnCardClicked);
                }
            }
            success?.Invoke();
        }
        private void OnErrorOccured()
        {
            error?.Invoke();
            searchLoadingBar.SetActive(false);
        }

        private void OnCardClicked(GameObject button) => _controller.ChangeState<UI_MovieDetailsPage>();


        private void ClearListView()
        {
            if (listView.childCount > 0)
            {
                foreach (Transform child in listView)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

}