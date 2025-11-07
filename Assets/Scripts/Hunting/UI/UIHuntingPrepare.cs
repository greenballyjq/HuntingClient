using System;
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
        [SerializeField] private Button characterMailButton;
        [SerializeField] private Button characterFemailButton;
        [SerializeField] private Button characterIpButton;
        [SerializeField] private Image characterIpImage;
        [SerializeField] private TextMeshProUGUI characterNameText;
        [SerializeField] private TextMeshProUGUI characterDescText;
        [SerializeField] private Image mapImage;
        [SerializeField] private TextMeshProUGUI mapNameText;
        [SerializeField] private TextMeshProUGUI mapDescText;

        [SerializeField] private Button luckButton;
        [SerializeField] private Button startButton;

        private GameLogic _gameLogic;

        private UIManager UI => GameServiceLocator.UI;
        
        private void Start()
        {
            characterMailButton.onClick.AddListener(() => ChooseCharacter(CharacterData.CharacterType.Base_Mail));
            characterFemailButton.onClick.AddListener(() => ChooseCharacter(CharacterData.CharacterType.Base_Femail));
            characterIpButton.onClick.AddListener(ChooseRandomCharacter);
            
            startButton.onClick.AddListener(async() =>
            {
                _gameLogic.StartGame();
                await UI.OpenUIAsync<UIHuntingGamePlay>("UIHuntingGamePlay");
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
            characterNameText.text = characterInfo.characterName;
            characterDescText.text = characterInfo.characterDescription;
        }

        private void ChooseRandomCharacter()
        {
            CharacterData.CharacterInfo randomCharacterInfo = characterData.GetRandomCharacterInfo();
            characterNameText.text = randomCharacterInfo.characterName;
            characterDescText.text = randomCharacterInfo.characterDescription;
            characterIpImage.sprite = randomCharacterInfo.characterSprite;
        }

        private void ChooseRandomMap()
        {
            HuntMapData.HuntMapInfo randomMapInfo = huntMapData.GetRandomMapInfo();
            mapNameText.text = randomMapInfo.mapName;
            mapDescText.text = randomMapInfo.mapDescription;
            mapImage.sprite = randomMapInfo.mapSprite;
        }
    }
}