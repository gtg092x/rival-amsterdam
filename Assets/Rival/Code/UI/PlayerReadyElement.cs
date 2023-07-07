using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class PlayerReadyElement : VisualElement
{
    public new class UxmlFactory : UxmlFactory<PlayerReadyElement, UxmlTraits> { }
    
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        readonly UxmlStringAttributeDescription _playerName = new UxmlStringAttributeDescription
            {name = "playerName", defaultValue = "Player 1"};
        
        readonly UxmlAssetAttributeDescription<RenderTexture> _playerAvatar = new UxmlAssetAttributeDescription<RenderTexture>()
            {name = "playerAvatar", defaultValue = null};
        
        readonly UxmlAssetAttributeDescription<RenderTexture> _enemyAvatar = new UxmlAssetAttributeDescription<RenderTexture>()
            {name = "enemyAvatar", defaultValue = null};

        readonly UxmlIntAttributeDescription _countdown = new UxmlIntAttributeDescription
            {name = "countdown", defaultValue = 3};

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var playerReady = (PlayerReadyElement) ve;
            playerReady.PlayerName = _playerName.GetValueFromBag(bag, cc);
            playerReady.PlayerAvatar = _playerAvatar.GetValueFromBag(bag, cc);
            playerReady.EnemyAvatar = _enemyAvatar.GetValueFromBag(bag, cc);
            playerReady.Countdown = _countdown.GetValueFromBag(bag, cc);
        }
    }
    
    public int Countdown
    {
        get => int.Parse(_countDown.text);
        set => _countDown.text = $"{value}";
    }

    public RenderTexture EnemyAvatar
    {
        get => _enemyAvatar.style.backgroundImage.value.renderTexture;
        set => _enemyAvatar.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(value));
    }

    public RenderTexture PlayerAvatar
    {
        get => _playerAvatar.style.backgroundImage.value.renderTexture;
        set => _playerAvatar.style.backgroundImage = new StyleBackground(Background.FromRenderTexture(value));
    }

    public string PlayerName
    {
        get => _playerName.text;
        set => _playerName.text = value;
    }

    private Label _playerName;
    private Label _countDown;
    private VisualElement _enemyAvatar;
    private VisualElement _playerAvatar;

    public IEnumerator PerformCountdown(float duration = 1f)
    {
        var dur = duration - 0.25f;
        yield return new WaitForEndOfFrame();
        while (Countdown > 0)
        {
            _countDown.RemoveFromClassList("opacity-in");
            _countDown.RemoveFromClassList("scale-down-in");
            yield return new WaitForSeconds(duration - 0.25f);
            _countDown.AddToClassList("opacity-in");
            _countDown.AddToClassList("scale-down-in");
            yield return new WaitForSeconds(duration - dur);
            Countdown -= 1;
            yield return new WaitForSeconds(0.01f);
        }
        
    }
    
    public PlayerReadyElement()
    {
        var op = Addressables.LoadAssetAsync<VisualTreeAsset>("UI/Components/RuntimePlayerReady.uxml");
        VisualTreeAsset template = op.WaitForCompletion();
        template.CloneTree(this);

        _playerName = this.Q<Label>("Player");
        _countDown = this.Q<Label>("Countdown");
        _playerAvatar = this.Q<VisualElement>("PlayerAvatar");
        _enemyAvatar = this.Q<VisualElement>("EnemyAvatar");
        Addressables.Release(op);
    }

    public void Init(int countdown)
    {
        Countdown = countdown;
        _countDown.AddToClassList("opacity-in");
        _countDown.AddToClassList("scale-down-in");
    }
}
