using Caliburn.Micro;
using CDSReviewerCore.PaperDB;
using CDSReviewerCore.ServiceInterfaces;
using CDSReviewerCore.Services.CDS;
using CDSReviewerWS.Views;
using CDSReviewewrModels.ViewModels;
using System;
using System.Reflection;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace CDSReviewerWS
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        /// <summary>
        /// The main app container for IOC injection, specific for WinRT services.
        /// </summary>
        private WinRTContainer _container;

        /// <summary>
        /// The Caliburn.Micro version of the navigation service
        /// </summary>
        /// <remarks>
        /// Most of the code uses the PCL version of this, so this never really leaves this object.
        /// Not even clear we need to hold onto it.
        /// </remarks>
        private INavigationService _navigationService;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Declare views, finding paths, etc.
        /// </summary>
        protected override void Configure()
        {
            base.Configure();

#if DEBUG
            // Turn on logging.
            LogManager.GetLog = t => new DebugLog(t);
#endif

            // Singletons that help us out
            _container = new WinRTContainer();
            _container.RegisterWinRTServices();
            _container.Singleton<ISearchStringParser, CDSSearchStringParser>();
            _container.Singleton<IInternalPaperDB, IsolatedStorageDB>();

            // The various views, explicitly declared
            _container
                .PerRequest<HomePageViewModel>()
                .PerRequest<AddCDSPaperViewModel>()
                .PerRequest<PaperViewModel>()
                ;

            // View models and views are in different locations, so we can be more "cross-platform".
            // The following allows Caliburn to navagate to the proper places for the views.
            AssemblySource.Instance.Add(typeof(HomePageViewModel).GetTypeInfo().Assembly);
            ViewLocator.AddNamespaceMapping("CDSReviewerModels.ViewModels", "CDSReviewerWS.Views");
            ViewModelLocator.AddNamespaceMapping("CDSReviewerWS.Views", "CDSReviewerModels.ViewModels");

            // Use the frame in OnLaunched, so we have to set up the first view we will be seeing here.
            PrepareViewFirst();
        }

        /// <summary>
        /// Setup firest view, and cache the PCL safe version of the Calibrun navigation service.
        /// </summary>
        /// <param name="rootFrame"></param>
        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _navigationService = _container.RegisterNavigationService(rootFrame);
            Caliburn.Micro.Portable.WS.NavService.RegisterINavService(_navigationService, _container);
        }

        /// <summary>
        /// Use the container for an IOC-like lookup.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        protected override object GetInstance(Type service, string key)
        {
            var inst = _container.GetInstance(service, key);
            if (inst != null)
                return inst;

            throw new ArgumentException(string.Format("Unable to get instance of type {0} with key {1}", service.Name, key));
        }

        /// <summary>
        /// Use our service container to get a list of everything of a particular type
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        protected override System.Collections.Generic.IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        /// <summary>
        /// Use our IOC stuff to buidl up an object instance.
        /// </summary>
        /// <param name="instance"></param>
        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Initialize();

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var resumed = false;
            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                resumed = _navigationService.ResumeState();
            }

            if (!resumed)
                DisplayRootView<HomePageView>();
        }
    }
}
