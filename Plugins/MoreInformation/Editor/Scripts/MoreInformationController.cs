using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.MoreInformation.Editor
{
    public class MoreInformationController : IDisposable
    {
        private readonly MoreInformationView _view;
        private readonly MoreInformationModel _model;

        public MoreInformationController(VisualElement root)
        {
            _view = new MoreInformationView(root);
            _model = new MoreInformationModel();

            _view.AboutCompanyButton.RegisterCallback<ClickEvent>(OpenCompanyWebsite);
            _view.AboutCompanyButton.tooltip = MoreInformationModel.CompanyWebsiteURL;

            _view.DocumentationButton.RegisterCallback<ClickEvent>(OpenDocumentation);
            _view.DocumentationButton.tooltip = _model.ReadmeURL;

            _view.DiscordServerButton.RegisterCallback<ClickEvent>(OpenDiscordJoinLink);
            _view.DiscordServerButton.tooltip = MoreInformationModel.DiscordJoinLinkURL;

            _view.BrowseProductsButton.RegisterCallback<ClickEvent>(OpenCompanyAssetStorePage);
            _view.BrowseProductsButton.tooltip = MoreInformationModel.LudwellAssetStorePageURL;

            _view.RateProductButton.RegisterCallback<ClickEvent>(OpenProductAssetStorePage);
            _view.RateProductButton.tooltip = MoreInformationModel.ProductAssetStorePageURL;
        }

        public void Dispose()
        {
            _view.AboutCompanyButton.UnregisterCallback<ClickEvent>(OpenCompanyWebsite);
            _view.DocumentationButton.UnregisterCallback<ClickEvent>(OpenDocumentation);
            _view.DiscordServerButton.UnregisterCallback<ClickEvent>(OpenDiscordJoinLink);
            _view.BrowseProductsButton.UnregisterCallback<ClickEvent>(OpenCompanyAssetStorePage);
            _view.RateProductButton.UnregisterCallback<ClickEvent>(OpenProductAssetStorePage);
            _view.Dispose();
        }

        public void Show()
        {
            _view.Show();
        }

        public void Hide()
        {
            _view.Hide();
        }

        private void OpenDocumentation(ClickEvent _)
        {
            EditorUtility.RevealInFinder(_model.ReadmeURL);
        }

        private void OpenCompanyWebsite(ClickEvent _)
        {
            Application.OpenURL(MoreInformationModel.CompanyWebsiteURL);
        }

        private void OpenDiscordJoinLink(ClickEvent _)
        {
            Application.OpenURL(MoreInformationModel.DiscordJoinLinkURL);
        }

        private void OpenCompanyAssetStorePage(ClickEvent _)
        {
            Application.OpenURL(MoreInformationModel.LudwellAssetStorePageURL);
        }

        private void OpenProductAssetStorePage(ClickEvent _)
        {
            Application.OpenURL(MoreInformationModel.ProductAssetStorePageURL);
        }
    }
}
