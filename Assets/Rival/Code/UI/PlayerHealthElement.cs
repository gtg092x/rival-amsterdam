using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class PlayerHealthElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<PlayerHealthElement, UxmlTraits> { }
    
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        readonly UxmlAssetAttributeDescription<RenderTexture> _avatar = new UxmlAssetAttributeDescription<RenderTexture>()
            {name = "avatar", defaultValue = null};

        readonly UxmlFloatAttributeDescription _health = new UxmlFloatAttributeDescription
            {name = "health", defaultValue = 1f};

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var playerReady = (PlayerHealthElement) ve;
            playerReady.HealthBarProgress = _health.GetValueFromBag(bag, cc);
            playerReady.Avatar = _avatar.GetValueFromBag(bag, cc);
        }
    }

    public RenderTexture Avatar
    {
        get => _avatar.style.backgroundImage.value.renderTexture;
        set => _avatar.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(value));
    }

    public float HealthBarProgress
    {
        get => _healthBar.value;
        set => _healthBar.value = value;
    }

    private readonly ProgressBar _healthBar;
    private readonly VisualElement _avatar;
    private VisualElement _container;
    
    public IEnumerator PerformTaunt()
    {
        yield return new WaitForEndOfFrame();
    }
    
    public PlayerHealthElement()
    {
        var op = Addressables.LoadAssetAsync<VisualTreeAsset>("UI/Components/PlayerHealth.uxml");
        VisualTreeAsset template = op.WaitForCompletion();
        template.CloneTree(this);

        _healthBar = this.Q<ProgressBar>("HealthBar");
        _avatar = this.Q<VisualElement>("CharacterPreview");
        Addressables.Release(op);
    }

    public void Init(RenderTexture renderTexture)
    {
        HealthBarProgress = 1f;
        Avatar = renderTexture;
    }
}
