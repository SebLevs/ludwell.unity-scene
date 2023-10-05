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
            self.HeaderColor = m_headerColor.GetValueFromBag(bag, cc);
        }
    }

    public FoldoutHeader()
    {
        AddHeaderText();
        InitToggle();
        InitUnityCheckMark();
        InitUnityContent();
    }

    protected const string UnityCheckmarkName = "unity-checkmark";
    protected const string UnityContentName = "unity-content";
    protected const string HeaderName = "header-text";
    protected TextElement HeaderTextElement;
    protected Toggle HeaderElement;
    private string _headerText;
    private Color _headerColor;

    public string HeaderText
    {
        get => _headerText;
        set
        {
            _headerText = value;
            HeaderTextElement.text = _headerText;
        }
    }

    public Color HeaderColor
    {
        get => _headerColor;
        set
        {
            _headerColor = value;
            HeaderElement.style.backgroundColor = new StyleColor(_headerColor);
        }
    }

    private void InitToggle()
    {
        HeaderElement = this.Q<Toggle>();
        HeaderElement.style.marginTop = 0;
        HeaderElement.style.marginBottom = 0;
        HeaderElement.style.marginLeft = 0;
        HeaderElement.style.marginRight = 0;

        HeaderElement.style.flexBasis = 22.5f;
    }

    private void InitUnityCheckMark()
    {
        var unityCheckMarkElement = this.Q<VisualElement>(UnityCheckmarkName);
        unityCheckMarkElement.style.marginTop = 0;
        unityCheckMarkElement.style.marginBottom = 0;
        unityCheckMarkElement.style.marginLeft = 8;
        unityCheckMarkElement.style.marginRight = 8;
    }

    private void InitUnityContent()
    {
        this.Q<VisualElement>(UnityContentName).style.marginLeft = 0;
    }

    private void AddHeaderText()
    {
        HeaderTextElement = new TextElement
        {
            name = HeaderName,
            text = _headerText,
            style = { unityTextAlign = TextAnchor.MiddleLeft}
        };

        this.Q(UnityCheckmarkName).parent.Add(HeaderTextElement);
    }
}