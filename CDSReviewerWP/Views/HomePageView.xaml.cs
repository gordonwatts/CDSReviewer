﻿using CDSReviewerCore.ViewModels;
using CDSReviewerModels.ViewModels;
using Microsoft.Phone.Controls;

namespace CDSReviewerWP.Views
{
    public partial class HomePageView : PhoneApplicationPage, IHomePageViewCallback
    {
        // Constructor
        public HomePageView()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}

        /// <summary>
        /// We are called when the view model is all hooked up.
        /// </summary>
        /// <param name="homePageViewModel"></param>
        public void FinalizeVMWiring(HomePageViewModel myVM)
        {
            PaperList.SelectionChanged += (s, args) =>
            {
                var myItem = PaperList.SelectedItem as PaperTileViewModel;
                if (myItem != null)
                    myVM.NavigateToPaperTile.Execute(myItem);
            };
        }
    }
}