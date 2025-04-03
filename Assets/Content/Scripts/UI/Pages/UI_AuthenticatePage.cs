using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using ModestTree;
using UnityEngine.EventSystems;

namespace Savana.Movie
{
    /// <summary>
    /// - Handles the authentication of the app with the api using an API Key
    /// - Fetches a list of movies  ahead for use by the Home screen
    /// </summary>
    public class UI_AuthenticatePage : MonoBehaviour, UIState
    {
        private UIController _controller;
        [SerializeField] private bool showMenu; // should menubar be active on this page
        [SerializeField] private GameObject menuBar; // menubar group
        [SerializeField] private GameObject apiInputPanel; // panel for API key input 
        [SerializeField] private TMP_InputField apiInputKey; // input field for API key

        [SerializeField] private Button getStartedBtn; // button which triggers the panel for API key inputfield
        [SerializeField] private Button proceedBtn;
        [SerializeField] private GameObject loadingBar;
        [SerializeField] private RectTransform continueRect;


        [SerializeField] private Image backgroundImage;
        [SerializeField] List<Sprite> collages;// random images to display when application loads the authenticate scree

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
            int rand = Random.Range(0, collages.Count);
            backgroundImage.sprite = collages[rand];

            menuBar.SetActive(showMenu);
            apiInputPanel.SetActive(false);
            gameObject.SetActive(true);
            loadingBar.SetActive(false);

            getStartedBtn.onClick.AddListener(GetStartedBtnClicked);
            proceedBtn.onClick.AddListener(ContinueBtnClicked);

            eventSystem.SetSelectedGameObject(getStartedBtn.gameObject);

        }
        public void Exit()
        {
            getStartedBtn.onClick.RemoveListener(GetStartedBtnClicked);
            proceedBtn.onClick.RemoveListener(ContinueBtnClicked);
            gameObject.SetActive(false);
        }

        public void Pause() { }
        public void Resume() { }

        private void GetStartedBtnClicked()
        {
            apiInputPanel.SetActive(true);
            proceedBtn.gameObject.SetActive(true);

            eventSystem.SetSelectedGameObject(apiInputKey.gameObject);
        }

        private void ContinueBtnClicked()
        {
            //ensures the api key input is valid

            string key = apiInputKey.text;
            key = key.Trim();

            if (key.IsEmpty())
            {
                apiInputKey.text = "Fill in an api key";
                return;
            }
            loadingBar.SetActive(true);
            proceedBtn.gameObject.SetActive(false);
            PlayerPrefs.SetString("TM_KEY", key);

            //make a network request using a Network service
            //pass a success callback and a failure callback to handle network response
            NetworkManager.Request_Get_NowPlaying(key, LoadHomePage, ErrorConnecting);
        }

        private void ErrorConnecting()
        {
            apiInputKey.text = "Error Connecting";
            proceedBtn.gameObject.SetActive(true);
            loadingBar.SetActive(false);
        }

        private void LoadHomePage() => _controller.ChangeState<UI_HomePage>();


    }

}