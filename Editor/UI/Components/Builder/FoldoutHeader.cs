using UnityEngine;
using UnityEngine.UIElements;

public class FoldoutHeader : Foldout
{
    public new class UxmlFactory : UxmlFactory<FoldoutHeader, UxmlTraits> { }

    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription m_headerText = new() { name = "headerText", defaultValue = "Foldout Name" };
        UxmlColorAttributeDescription m_headerColor = new() { name = "headerColor" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var self = ve as FoldoutHeader;

            self.HeaderText = m_headerText.GetValueFromBag(bag, cc);
            self.headerColor = m_headerColor.GetValueFromBag(bag, cc);
        }
    }

    private const string UnityCheckmarkName = "unity-checkmark";
    private const string UnityContentName = "unity-content";
    private const string HeaderName = "header-text";
    private TextElement _headerTextElement;
    private Toggle _headerElement;
    private string _headerText;
    private Color _headerColor;
    
    public string HeaderText
    {
        get => _headerText;
        set
        {
            _headerText = value;
            _headerTextElement.text = _headerText;
        }
    }
    
    public Color headerColor
    {
        get => _headerColor;
        set
        {
            _headerColor = value;
            _headerElement.style.backgroundColor = new StyleColor(_headerColor);
        }
    }

    public FoldoutHeader()
    {
        AddHeaderText();
        InitToggle();
        InitUnityContent();
    }
    
    private void InitToggle()
    {
        _headerElement = this.Q<Toggle>();
        _headerElement.style.marginTop = 0;
        _headerElement.style.marginBottom = 0;
        _headerElement.style.marginLeft = 0;
        _headerElement.style.marginRight = 0;
    
        _headerElement.style.flexBasis = 22.5f;
    }
    
    private void InitUnityContent()
    {
        this.Q<VisualElement>(UnityContentName).style.marginLeft = 0;
    }
    
    private void AddHeaderText()
    {
        _headerTextElement = new TextElement
        {
            name = HeaderName,
            text = _headerText
        };
    
        this.Q(UnityCheckmarkName).parent.Add(_headerTextElement);
    }
}