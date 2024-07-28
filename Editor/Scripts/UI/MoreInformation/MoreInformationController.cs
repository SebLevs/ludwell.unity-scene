using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Ludwell.Scene.Editor
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
            _view.DocumentationButton.RegisterCallback<ClickEvent>(OpenDocumentation);
            _view.DiscordServerButton.RegisterCallback<ClickEvent>(OpenDiscordJoinLink);
            _view.BrowseProductsButton.RegisterCallback<ClickEvent>(OpenCompanyAssetStorePage);
            _view.RateProductButton.RegisterCallback<ClickEvent>(OpenProductAssetStorePage);
        }

        public void Dispose()
        {
            _view.AboutCompanyButton.UnregisterCallback<ClickEvent>(OpenCompanyWebsite);
            _view.DocumentationButton.UnregisterCallback<ClickEvent>(OpenCompanyWebsite);
            _view.DiscordServerButton.UnregisterCallback<ClickEvent>(OpenDocumentation);
            _view.BrowseProductsButton.UnregisterCallback<ClickEvent>(OpenDiscordJoinLink);
            _view.RateProductButton.UnregisterCallback<ClickEvent>(OpenCompanyAssetStorePage);
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
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }

        private void OpenCompanyWebsite(ClickEvent _)
        {
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }

        private void OpenDiscordJoinLink(ClickEvent _)
        {
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }

        private void OpenCompanyAssetStorePage(ClickEvent _)
        {
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }

        private void OpenProductAssetStorePage(ClickEvent _)
        {
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }
    }
}