using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

namespace Savana.Movie
{
    public class UI_MovieCard_Item : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        //private Model_Result _result;
        private Response_MovieDetail _result;
        private UI_MovieDetailsPage detailsPage;


        private Action<GameObject> OnClicked;
        [SerializeField] private Image poster;
        [SerializeField] private TextMeshProUGUI titleTxt;
        [SerializeField] private TextMeshProUGUI releaseDataTxt;
        [SerializeField] private Button button;
        [SerializeField] private GameObject selector;
        public RectTransform rect;


        public Sprite GetPoster() => poster.sprite;


        public void SetData(Response_MovieDetail result, UI_MovieDetailsPage details, Action<GameObject> clicked)
        {
            _result = result;
            detailsPage = details;
            OnClicked = clicked;
            rect = GetComponent<RectTransform>();

            titleTxt.text = result.original_title;
            releaseDataTxt.text = result.release_date;

            Texture2D texture = new(2, 2);
            texture.LoadImage(result.imageData);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            poster.sprite = sprite;
            gameObject.SetActive(true);

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ShowDetails);
        }

        public void UpdateData(Response_MovieDetail result)
        {
            _result = result;

            titleTxt.text = result.original_title;
            releaseDataTxt.text = result.release_date;

            Texture2D texture = new(2, 2);
            texture.LoadImage(result.imageData);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            poster.sprite = sprite;
        }

        private void ShowDetails()
        {
            detailsPage.SetData(poster.sprite, _result);
            OnClicked?.Invoke(gameObject);
        }

        public void OnSelect(BaseEventData eventData) => selector.SetActive(true);
        public void OnDeselect(BaseEventData eventData) => selector.SetActive(false);


    }

}