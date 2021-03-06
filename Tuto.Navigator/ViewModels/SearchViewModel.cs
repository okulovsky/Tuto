﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Navigator.ViewModels
{

    public enum SortType
    {
        [Description("alphabetically")]
        Alphabetically,
        [Description("creation time")]
        CreationTime,
        [Description("modification time")]
        ModificationTime
    }
    public class SearchViewModel : NotifierModel
    {
        string textSearch;
        public string TextSearch
        {
            get { return textSearch; }
            set
            {
                textSearch = value;
                NotifyPropertyChanged();
            }
        }

        bool onlyWithSource;
        public bool OnlyWithSource
        {
            get { return onlyWithSource; }
            set
            {
                onlyWithSource = value;
                NotifyPropertyChanged();
            }
        }

        public event Action RefreshRequested;

        public RelayCommand Refresh { get; set; }

        public event Action SelectAllRequested;

        public RelayCommand SelectAll { get; set; }

        OptionViewModel<SortType> sortType;
        public OptionViewModel<SortType> SortType
        {
            get { return sortType; }
            set { sortType = value; NotifyPropertyChanged(); }
        }

        public List<OptionViewModel<SortType>> SortTypes { get; private set; }

        public SearchViewModel()
        {
            Refresh = new RelayCommand(() => { if (RefreshRequested != null) RefreshRequested(); });
            SelectAll = new RelayCommand(() => { if (SelectAllRequested != null) SelectAllRequested(); });
            SortTypes = OptionViewModel.FromEnum<SortType>().ToList();
            SortType = SortTypes[0];
        }
    }
}
