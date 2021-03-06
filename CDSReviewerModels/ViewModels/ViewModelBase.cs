﻿using Caliburn.Micro;
using Caliburn.Micro.Portable;

namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// Base class for common items for our view models.
    /// </summary>
    public class ViewModelBase : Screen
    {
        /// <summary>
        /// The navigation service, if anyone below needs to use it!
        /// </summary>
        protected INavService _nav;
        public ViewModelBase(INavService nav)
        {
            _nav = nav;
        }

        public void GoBack()
        {
            _nav.GoBack();
        }

        public bool CanGoBack
        {
            get { return _nav.CanGoBack; }
        }
    }
}
