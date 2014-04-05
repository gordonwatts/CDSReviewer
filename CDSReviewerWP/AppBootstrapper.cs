namespace CDSReviewerWP
{
    using Caliburn.Micro;
    using CDSReviewewrModels.ViewModels;
    using Microsoft.Phone.Controls;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows.Controls;

    public class AppBootstrapper : PhoneBootstrapperBase
    {
        PhoneContainer container;

        public AppBootstrapper()
        {
            Start();
        }

        /// <summary>
        /// Configure everything for running
        /// </summary>
        protected override void Configure()
        {
            // Debugging log - very helpful for tracking down
            // bad views!
            if (Debugger.IsAttached)
                LogManager.GetLog = t => new DebugLog(t);

            container = new PhoneContainer();
            if (!Execute.InDesignMode)
                container.RegisterPhoneServices(RootFrame);

            // Register our custom navigation service.
            var nav = container.GetInstance<INavigationService>();
            Caliburn.Micro.Portable.WP8.NavService.RegisterINavService(container);

            // Declare the pages we are going to be visiting
            container
                .PerRequest<HomePageViewModel>()
                .PerRequest<AddCDSPaperViewModel>()
                ;

            // We are crossing projects for views and view models. Set that up for the
            // type resolver. Also make sure that it loads the types we are interested in!
            AssemblySource.Instance.Add(typeof(HomePageViewModel).Assembly);
            ViewLocator.AddNamespaceMapping("CDSReviewerModels.ViewModels", "CDSReviewerWP.Views");
            ViewModelLocator.AddNamespaceMapping("CDSReviewerWP.Views", "CDSReviewerModels.ViewModels");

            // Conventions based behaviors.
            AddCustomConventions();
        }

        /// <summary>
        /// Look up a service from our container
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetInstance(Type service, string key)
        {
            var instance = container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        /// <summary>
        /// Get a list of objects that satisfy a certain constraint.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        /// <summary>
        /// Build an object up with whatever services it needs.
        /// </summary>
        /// <param name="instance"></param>
        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }

        /// <summary>
        /// Some helpers for various actions.
        /// </summary>
        static void AddCustomConventions()
        {
            ConventionManager.AddElementConvention<Pivot>(Pivot.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Pivot.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Pivot.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };

            ConventionManager.AddElementConvention<Panorama>(Panorama.ItemsSourceProperty, "SelectedItem", "SelectionChanged").ApplyBinding =
                (viewModelType, path, property, element, convention) =>
                {
                    if (ConventionManager
                        .GetElementConvention(typeof(ItemsControl))
                        .ApplyBinding(viewModelType, path, property, element, convention))
                    {
                        ConventionManager
                            .ConfigureSelectedItem(element, Panorama.SelectedItemProperty, viewModelType, path);
                        ConventionManager
                            .ApplyHeaderTemplate(element, Panorama.HeaderTemplateProperty, null, viewModelType);
                        return true;
                    }

                    return false;
                };
        }
    }
}