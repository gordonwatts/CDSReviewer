﻿using Caliburn.Micro.Portable;
using Caliburn.Micro.ReactiveUI;
using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// The home page view model
    /// </summary>
    public class HomePageViewModel : ViewModelBase
    {
        /// <summary>
        /// Initialize with the navigation service.
        /// </summary>
        /// <param name="nav"></param>
        public HomePageViewModel(INavService nav, IInternalPaperDB paperDB)
            : base(nav)
        {
            Observable.FromAsync(paperDB.GetFullInformation)
                .Select(x => new ObservableCollection<Tuple<PaperStub, PaperFullInfo>>(x))
                .Select(x =>
                {
                    _paperListRaw = x;
                    return _paperListRaw.CreateDerivedCollection(t => new PaperTileViewModel(nav, t.Item1, t.Item2));
                })
                .ToPropertyCM(this, x => x.PaperList, out _PaperListOAPH, null);
        }

        private ObservableCollection<Tuple<PaperStub, PaperFullInfo>> _paperListRaw;

        /// <summary>
        /// The list of papers this view model is showing
        /// </summary>
        public IReactiveDerivedList<PaperTileViewModel> PaperList
        {
            get { return _PaperListOAPH.Value; }
        }
        private ObservableAsPropertyHelper<IReactiveDerivedList<PaperTileViewModel>> _PaperListOAPH;

        /// <summary>
        /// Move away to the add new document view.
        /// </summary>
        public void CmdAdd()
        {
            _nav.NavigateToViewModel<AddCDSPaperViewModel>();
        }
    }
}
