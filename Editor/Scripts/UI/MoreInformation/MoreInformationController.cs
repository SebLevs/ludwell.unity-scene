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
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }
        
        public void OpenCompanyWebsite()
        {
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }
        
        public void OpenDiscordJoinLink()
        {
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }
        
        public void OpenCompanyAssetStorePage()
        {
            Application.OpenURL(MoreInformationModel.DocumentationURL);  
        }
        
        public void OpenProductAssetStorePage()
        {
            Application.OpenURL(MoreInformationModel.DocumentationURL);
        }
    }
}
