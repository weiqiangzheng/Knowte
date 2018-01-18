﻿using Digimezzo.Utilities.Utils;
using Knowte.Presentation.Contracts.Entities;
using Knowte.Services.Constracts.Dialog;
using Knowte.Services.Contracts.Collection;
using Knowte.Views.Dialogs;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Knowte.ViewModels
{
    public class MainViewModel : BindableBase
    {
        private bool showCollections;
        private ObservableCollection<CollectionViewModel> collections;
        private IUnityContainer container;
        private ICollectionService collectionService;
        private IDialogService dialogService;
        private CollectionViewModel selectedCollection;
        private CollectionViewModel activeCollection;

        public bool CanActivateSelectedCollection
        {
            get
            {
                if (this.selectedCollection == null)
                {
                    return false;
                }

                return !this.selectedCollection.Equals(this.activeCollection);
            }
        }

        public bool ShowCollections
        {
            get { return this.showCollections; }
            set
            {
                SetProperty<bool>(ref this.showCollections, value);
                RaisePropertyChanged(nameof(ContentIndex));
            }
        }
        public ObservableCollection<CollectionViewModel> Collections
        {
            get { return this.collections; }
            set { SetProperty<ObservableCollection<CollectionViewModel>>(ref this.collections, value); }
        }

        public CollectionViewModel SelectedCollection
        {
            get { return this.selectedCollection; }
            set
            {
                SetProperty<CollectionViewModel>(ref this.selectedCollection, value);
                RaisePropertyChanged(nameof(this.CanActivateSelectedCollection));
            }
        }

        public CollectionViewModel ActiveCollection
        {
            get { return this.activeCollection; }
            set
            {
                SetProperty<CollectionViewModel>(ref this.activeCollection, value);
                RaisePropertyChanged(nameof(this.CanActivateSelectedCollection));
            }
        }

        public int ContentIndex
        {
            get { return this.showCollections ? 0 : 1; }
        }

        public DelegateCommand PaneClosedCommand { get; set; }

        public DelegateCommand LoadedCommand { get; set; }

        public DelegateCommand AddCollectionCommand { get; set; }

        public DelegateCommand ActivateCollectionCommand { get; set; }

        public MainViewModel(IUnityContainer container, ICollectionService collectionService, IDialogService dialogService)
        {
            this.container = container;
            this.collectionService = collectionService;
            this.dialogService = dialogService;

            this.PaneClosedCommand = new DelegateCommand(() => this.ShowCollections = false);
            this.LoadedCommand = new DelegateCommand(() => this.GetCollectionsAsync());
            this.AddCollectionCommand = new DelegateCommand(() => this.AddCollectionCommandHandler());
            this.ActivateCollectionCommand = new DelegateCommand(() => this.AddCollectionCommandHandler());
        }

        private void AddCollectionCommandHandler()
        {
            var view = this.container.Resolve<AddCollection>();

            this.dialogService.ShowCustom(
                ResourceUtils.GetString("Language_Add_Collection"),
                view,
                420,
                0,
                false,
                true,
                true,
                true,
                ResourceUtils.GetString("Language_Ok"),
                ResourceUtils.GetString("Language_Cancel"),
                null);
        }

        private async void GetCollectionsAsync()
        {
            // Remember the selected bank account
            string selectedCollectionId = string.Empty;

            if (this.SelectedCollection != null)
            {
                selectedCollectionId = this.SelectedCollection.Collection.Id;
            }

            this.Collections = new ObservableCollection<CollectionViewModel>(await this.collectionService.GetCollectionsAsync());

            this.ActiveCollection = null;
            this.SelectedCollection = null;
            this.ActiveCollection = this.Collections.Where(c => c.IsActive).FirstOrDefault();
            this.SelectedCollection = this.Collections.Where(c => c.Collection.Id.Equals(selectedCollectionId)).FirstOrDefault();
        }
    }
}