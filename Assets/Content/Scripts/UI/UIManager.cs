using UnityEngine;

namespace Savana.Movie
{
    /// <summary>
    /// Holds references to all screens which will be mamanged by a controller class
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        private UIController controller;

        [SerializeField] private UI_AuthenticatePage authenticatePage;
        [SerializeField] private UI_HomePage homePage;
        [SerializeField] private UI_MovieDetailsPage movieDetailsPage;

        private void Awake() => controller = new();

        private void Start()
        {
            // 
            // plugs ui screen into the controller 
            authenticatePage.AttachTo(controller);
            homePage.AttachTo(controller);
            movieDetailsPage.AttachTo(controller);


            //load the authentication screen on application start.
            //app entry point
            controller.ChangeState<UI_AuthenticatePage>();
        }
    }

}