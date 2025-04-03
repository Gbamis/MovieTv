using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Savana.Movie
{
    public enum ITEM_BOUNDARY { OUT_LEFT, OUT_RIGHT, MID }

    [RequireComponent(typeof(ScrollRect))]
    public class RecycleScrollRect : MonoBehaviour
    {
        private Action<int> _OnFetchNewData;

        private ScrollRect scrollRect;
        private RectTransform spaceElementRect;

        private float leftMargin;
        private float rightMarginPrtrait = 2200f;
        private float rightMarginLandscape = 550f;
        private float spaceElementWidth = 310f;

        private int topDataIndex;
        private int bottomDataIndex;
        private int total;
        public int lastScrollIndex;

        private RectTransform scrollRectComp;
        private List<UI_MovieCard_Item> itemPool = new();
        private Dictionary<Transform, UI_MovieCard_Item> poolHash = new();
        private UI_MovieDetailsPage _movieDetailsPage;


        private bool createdPool;
        private bool initialized;
        private int maxViewAtOnce;

        public bool isPortrait;
        public GameObject Selected { get => scrollRect.content.GetChild(1).gameObject; }

        [SerializeField] private UI_MovieCard_Item itemPrefab;


        private void Awake()
        {
            scrollRect = GetComponent<ScrollRect>();
            scrollRectComp = scrollRect.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            scrollRect.onValueChanged.AddListener(OnScroll);
            if (!initialized)
            {
                CreateSpaceElement();
                initialized = true;
            }
        }
        private void OnDisable() => scrollRect.onValueChanged.RemoveListener(OnScroll);

        private void CreateSpaceElement()
        {
            GameObject space = new GameObject("Space");
            space.SetActive(true);
            space.transform.SetParent(scrollRect.content);

            spaceElementRect = space.AddComponent<RectTransform>();

            RectTransform itemRect = itemPrefab.GetComponent<RectTransform>();

            if (scrollRect.horizontal)
            {
                float height = itemRect.sizeDelta.y;
                float width = 0;
                spaceElementRect.sizeDelta = new Vector2(width, height);

            }
            else
            {
                float height = 0;
                float width = itemRect.sizeDelta.x;
                spaceElementRect.sizeDelta = new Vector2(width, height);
            }
        }

        public void CreatePool(List<Response_MovieDetail> dataSource,
        UI_MovieDetailsPage movieDetailsPage,
        Action<GameObject> OnItemClicked,
        Action<int> OnFetch)
        {
            if (createdPool) { return; }
            _movieDetailsPage = movieDetailsPage;
            _OnFetchNewData = OnFetch;

            total = dataSource.Count;

            maxViewAtOnce = 6;
            for (int i = 0; i < Mathf.Min(maxViewAtOnce, dataSource.Count); i++)
            {
                UI_MovieCard_Item item = Instantiate(itemPrefab, scrollRect.content);
                item.SetData(dataSource[i], _movieDetailsPage, OnItemClicked);
                itemPool.Add(item);
                poolHash.Add(item.transform, item);
            }
            createdPool = true;

            topDataIndex = 0;
            bottomDataIndex = maxViewAtOnce - 1;
        }

        private void OnScroll(Vector2 scrollPos)
        {
            float width = Screen.width;
            float height = Screen.height;
            isPortrait = height > width;

            GetScrollIndex(scrollPos);
            Trim_Min();
            Trim_Max();
        }

        private void GetScrollIndex(Vector2 pos)
        {
            float scrollPos;
            if (scrollRect.horizontal)
            {
                scrollPos = 1 - scrollRect.horizontalNormalizedPosition; // 1 = Top, 0 = Bottom
            }
            else
            {
                scrollPos = 1 - scrollRect.verticalNormalizedPosition; // 1 = Top, 0 = Bottom
            }

            int totalItems = NetworkManager.response_NowPlaying.Count;

            int firstVisibleIndex = Mathf.FloorToInt((1 - scrollPos) * totalItems);
            firstVisibleIndex = Mathf.Clamp(firstVisibleIndex, 0, totalItems - 1);

            if (lastScrollIndex != firstVisibleIndex)
            {
                lastScrollIndex = firstVisibleIndex;
                if (lastScrollIndex >= totalItems - 5)
                {
                    _OnFetchNewData?.Invoke(lastScrollIndex);
                }
            }
        }

        private void Trim_Min()
        {
            int max = scrollRect.content.childCount;
            Transform child = scrollRect.content.GetChild(1);
            var item = poolHash[child];

            ITEM_BOUNDARY bound = IsOutsideHorizontally(item.rect, scrollRectComp, isPortrait ? rightMarginPrtrait : rightMarginLandscape);

            if (bound == ITEM_BOUNDARY.OUT_LEFT && bottomDataIndex < NetworkManager.response_NowPlaying.Count - 2)
            {
                bottomDataIndex++;
                child.SetSiblingIndex(max - 1);

                float height = spaceElementRect.sizeDelta.y;
                float width = spaceElementRect.sizeDelta.x;
                width += spaceElementWidth;

                spaceElementRect.sizeDelta = new Vector2(width, height);

                item.UpdateData(NetworkManager.response_NowPlaying[bottomDataIndex]);
            }
        }

        private void Trim_Max()
        {
            int max = scrollRect.content.childCount;

            Transform child = scrollRect.content.GetChild(max - 1);
            var item = poolHash[child];
            ITEM_BOUNDARY bound = IsOutsideHorizontally(item.rect, scrollRectComp, isPortrait ? rightMarginPrtrait : rightMarginLandscape);

            if (bound == ITEM_BOUNDARY.OUT_RIGHT && topDataIndex > -1)
            {
                topDataIndex = bottomDataIndex - maxViewAtOnce;
                bottomDataIndex--;
                topDataIndex = Mathf.Clamp(topDataIndex, 0, 10000);

                child.SetSiblingIndex(1);

                float height = spaceElementRect.sizeDelta.y;
                float width = spaceElementRect.sizeDelta.x;
                width -= spaceElementWidth;

                spaceElementRect.sizeDelta = new Vector2(width, height);
                item.UpdateData(NetworkManager.response_NowPlaying[topDataIndex]);
            }
        }

        ITEM_BOUNDARY IsOutsideHorizontally(RectTransform child, RectTransform parent, float rMargin)
        {
            Vector3[] childCorners = new Vector3[4];
            Vector3[] parentCorners = new Vector3[4];


            child.GetWorldCorners(childCorners);
            parent.GetWorldCorners(parentCorners);


            float childLeft = childCorners[0].x;
            float childRight = childCorners[3].x;

            float parentLeft = parentCorners[0].x;
            float parentRight = parentCorners[3].x;


            if (childRight < parentLeft)
            {
                return ITEM_BOUNDARY.OUT_LEFT;
            }
            else if (childLeft > parentRight + rMargin)
            {
                return ITEM_BOUNDARY.OUT_RIGHT;
            }

            return ITEM_BOUNDARY.MID;
        }

        /*private void GetSVisibleItemWithinScroll(Vector2 pos)
        {
            float scrollPos = 1 - scrollRect.horizontalNormalizedPosition; // 1 = Top, 0 = Bottom
                                                                           //int totalItems = now_playingData.Count;
            int totalItems = maxViewAtOnce;

            int firstVisibleIndex = Mathf.FloorToInt((1 - scrollPos) * totalItems);
            firstVisibleIndex = Mathf.Clamp(firstVisibleIndex, 0, totalItems - 1);

            if (lastScrollIndex != firstVisibleIndex)
            {
                lastScrollIndex = firstVisibleIndex;
                DisplayBakdrop(firstVisibleIndex).Forget();
            }
        }*/

    }

}