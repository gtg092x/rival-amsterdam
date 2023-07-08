using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class BossHealthElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<BossHealthElement, UxmlTraits> { }
    
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        readonly UxmlAssetAttributeDescription<RenderTexture> _enemyAvatar = new UxmlAssetAttributeDescription<RenderTexture>()
            {name = "enemyAvatar", defaultValue = null};

        readonly UxmlFloatAttributeDescription _bossHealth = new UxmlFloatAttributeDescription
            {name = "bossHealth", defaultValue = 1f};

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var playerReady = (BossHealthElement) ve;
            playerReady.BossBarProgress = _bossHealth.GetValueFromBag(bag, cc);
            playerReady.EnemyAvatar = _enemyAvatar.GetValueFromBag(bag, cc);
        }
    }

    public RenderTexture EnemyAvatar
    {
        get => _enemyAvatar.style.backgroundImage.value.renderTexture;
        set => _enemyAvatar.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(value));
    }

    public float BossBarProgress
    {
        get => _bossBar.value;
        set => _bossBar.value = value;
    }

    private ProgressBar _bossBar;
    private VisualElement _enemyAvatar;
    private VisualElement _container;
    
    public IEnumerator PerformTaunt()
    {
        yield return new WaitForEndOfFrame();
    }
    
    public BossHealthElement()
    {
        var op = Addressables.LoadAssetAsync<VisualTreeAsset>("UI/Components/BossHealth.uxml");
        VisualTreeAsset template = op.WaitForCompletion();
        template.CloneTree(this);

        _bossBar = this.Q<ProgressBar>("BossBar");
        _enemyAvatar = this.Q<VisualElement>("Boss");
        Addressables.Release(op);
    }

    public void Init(float health)
    {
        BossBarProgress = health * 100f;
    }
}
