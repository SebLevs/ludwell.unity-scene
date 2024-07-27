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
        }
        
        public void Dispose()
        {
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

        public void OpenDocumentation()
        {
            Application.OpenURL(_model.DocumentationURL);
        }
        
        public void OpenCompanyWebsite()
        {
            UnityEngine.Application.OpenURL(_model.CompanyWebsiteURL);
        }
        
        public void OpenDiscordJoinLink()
        {
            UnityEngine.Application.OpenURL(_model.DocumentationURL);
        }
        
        public void OpenCompanyAssetStorePage()
        {
            UnityEngine.Application.OpenURL(_model.LudwellAssetStorePageURL);  
        }
        
        public void OpenProductAssetStorePage()
        {
            UnityEngine.Application.OpenURL(_model.ProductAssetStorePageURL);
        }
    }
}
