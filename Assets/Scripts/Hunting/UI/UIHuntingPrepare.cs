using GameFramework.Core;
using Hunting.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hunting.UI
{
    public class UIHuntingPrepare : UIBase
    {
        [SerializeField] private CharacterData characterData;
        [SerializeField] private HuntMapData huntMapData;


        [SerializeField] private Button _buttonRoleMail;
        [SerializeField] private Button _buttonRoleFemail;
        [SerializeField] private Button _buttonRoleRandom;
        [SerializeField] private TextMeshProUGUI _textRoleName;
        [SerializeField] private TextMeshProUGUI _textRoleProfile;
        [SerializeField] private Image _imageRole;
        [SerializeField] private TextMeshProUGUI _textMapName;
        [SerializeField] private TextMeshProUGUI _textMapDescription;
        [SerializeField] private Image _imageMap;
        [SerializeField] private Button _buttonLuckyRitual;
        [SerializeField] private Button _buttonStartGame;

        private GameLogic _gameLogic;

        private UIManager UI => GameServiceLocator.UI;
        
        private void Start()
        {
            _buttonRoleMail.onClick.AddListener(() => ChooseCharacter(CharacterData.CharacterType.Base_Mail));
            _buttonRoleFemail.onClick.AddListener(() => ChooseCharacter(CharacterData.CharacterType.Base_Femail));
            _buttonRoleRandom.onClick.AddListener(ChooseRandomCharacter);
            
            _buttonStartGame.onClick.AddListener(async() =>
            {
                _gameLogic.StartGame();
                await UI.OpenUIAsync<UIHuntingGameplay>("UIHuntingGamePlay");
                Hide();
            });
            
            ChooseRandomMap();

        }

        public void SetGameLogic(GameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        private void ChooseCharacter(CharacterData.CharacterType characterType)
        {
            CharacterData.CharacterInfo characterInfo = characterData.GetCharacterInfoFromType(characterType);
            _textRoleName.text = characterInfo.characterName;
            _textRoleProfile.text = characterInfo.characterDescription;
        }

        private void ChooseRandomCharacter()
        {
            CharacterData.CharacterInfo randomCharacterInfo = characterData.GetRandomCharacterInfo();
            _textRoleName.text = randomCharacterInfo.characterName;
            _textRoleProfile.text = randomCharacterInfo.characterDescription;
            _imageRole.sprite = randomCharacterInfo.characterSprite;
        }

        private void ChooseRandomMap()
        {
            HuntMapData.HuntMapInfo randomMapInfo = huntMapData.GetRandomMapInfo();
            _textMapName.text = randomMapInfo.mapName;
            _textMapDescription.text = randomMapInfo.mapDescription;
            _imageMap.sprite = randomMapInfo.mapSprite;
        }
    }
}