using System;
using System.Collections.Generic;
using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core.Audio;
using GameFramework.Game;
using GameFramework.Manager;
using UnityEngine;

namespace CoreGameLogic.Managers.AppManagers
{
    /// <summary>
    /// 打猎音效管理器
    /// </summary>
    public class HuntingSoundManager : IAppManager
    {
        private ResourceManager _resourceManager => GameServiceLocator.ResourceManager;
        private SoundManager _soundManager => GameServiceLocator.GetFrameworkManager<SoundManager>();
        private EventManager _eventManager => GameServiceLocator.EventManager;
        
        private HuntingAudioRefSo _huntingAudioRefSo;

        #region IAppManager生命周期函数

        public async void Init()
        {
            _huntingAudioRefSo = await _resourceManager.LoadAssetAsync<HuntingAudioRefSo>("Assets/Arts/Audio/HuntingAudioRefSo");

            if (_huntingAudioRefSo ==null)
            {
                Debug.LogError($"[HuntingSoundManager] HuntingAudioRefSo 资源加载失败");
                return;
            }

            var audioClip = _huntingAudioRefSo.GetAudioFromType(HuntingAudioRefSo.HuntingGameAudioType.MapEnv_Beatch);
            if (audioClip == null)
            {
                Debug.LogError($"[HuntingSoundManager] audioClip 资源加载失败");
                return;
            }

            SubscribeAudioEvents();

            Debug.Log($"[HuntingSoundManager] 初始化成功");
        }

        public void Dispose()
        {
            // todo 释放so和音频文件
            
            
            UnsubscribeAudioEvents();
        }

        #endregion
        
        #region 公共方法

        /// <summary>
        /// 播放指定2D音效（通过音频文件）
        /// </summary>
        /// <param name="audioType">打猎音频类型</param>
        /// <param name="channel">音频通道，默认为Sound</param>
        /// <param name="loop">播放次数（1表示播放一次，2表示播放2次，-1表示无限循环）</param>
        /// <param name="volume">音量（0-1），默认为1</param>
        /// <returns></returns>
        public AudioCallback PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType audioType, AudioChannel channel = AudioChannel.Sound
            , int loop = 1, float volume = 1f)
        {
            var audioClip = _huntingAudioRefSo.GetAudioFromType(audioType);
            return _soundManager.PlaySound2D(audioClip, channel, loop, volume);
            
        }

        #endregion

        #region 事件处理

        /// <summary>
        /// 订阅事件
        /// </summary>
        private void SubscribeAudioEvents()
        {
            _eventManager.AddListener(PrepareEvents.RoleSelectedEnd, OnRoleSelectedEnd);
            _eventManager.AddListener(PropEvents.PropStarted, OnPropStarted);
            _eventManager.AddListener(SkillEvents.SkillStarted, OnSkillStarted);
            _eventManager.AddListener(SettlementEvents.SettlementStarted, OnSettlementStarted);
            HuntingAppFlow.Instance.OnRoundEntered += OnRoundEntered;
        }

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        private void UnsubscribeAudioEvents()
        {
            _eventManager.RemoveListener(PrepareEvents.RoleSelectedEnd, OnRoleSelectedEnd);
            _eventManager.RemoveListener(PropEvents.PropStarted, OnPropStarted);
            _eventManager.RemoveListener(SkillEvents.SkillStarted, OnSkillStarted);
            _eventManager.RemoveListener(SettlementEvents.SettlementStarted, OnSettlementStarted);
            HuntingAppFlow.Instance.OnRoundEntered -= OnRoundEntered;
        }

        /// <summary>
        /// 角色选择结束音效播放事件
        /// </summary>
        /// <param name="args"></param>
        private void OnRoleSelectedEnd(RoleSelectedOverEventArgs args)
        {
            var roleType = args.RoleType;
            switch (roleType)
            {
                case ERoleType.ZiWei:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_Selected, AudioChannel.Voice);
                    break;
                case ERoleType.DaMeiLi:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_Selected, AudioChannel.Voice);
                    break;
                case ERoleType.JinZhuangYuan:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_Selected, AudioChannel.Voice);
                    break;
                case ERoleType.YaKeDong:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_Selected, AudioChannel.Voice);
                    break;
            }
        }
        
        /// <summary>
        /// 开局播放开场白事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRoundEntered(object sender, HuntingAppFlow.OnRoundEnteredEventArgs e)
        {
            var roundContext = e.RoundContext;
            var roleType = roundContext.RoleData.RoleType;
            var hasLinkage = roundContext.HasLinkage;

            switch (roleType)
            {
                case ERoleType.ZiWei:
                    PlaySound2D(hasLinkage ? HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UniqueMap_Opening : HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_Opening, AudioChannel.Voice);
                    break;
                case ERoleType.DaMeiLi:
                    PlaySound2D(hasLinkage ? HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_UniqueMap_Opening : HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_Opening, AudioChannel.Voice);
                    break;
                case ERoleType.JinZhuangYuan:
                    PlaySound2D(hasLinkage ? HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_UniqueMap_Opening : HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_Opening, AudioChannel.Voice);
                    break;
                case ERoleType.YaKeDong:
                    PlaySound2D(hasLinkage ? HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_UniqueMap_Opening : HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_Opening, AudioChannel.Voice);
                    break;
            }
        }
        
        /// <summary>
        /// 道具使用播放音效事件
        /// </summary>
        /// <param name="args"></param>
        private void OnPropStarted(PropStartedEventArgs args)
        {
            var propType = args.PropData.PropType;
            var roundContext = HuntingAppFlow.Instance.GetCurrentRoundFlow().CurrentRoundContext;
            var roleType = roundContext.RoleData.RoleType;

            switch (propType)
            {
                case EPropType.Bombardment:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UseProps_Bombardment);
                    switch (roleType)
                    {
                        case ERoleType.ZiWei:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UseProps_Bombardment, AudioChannel.Voice);
                            break;
                        case ERoleType.DaMeiLi:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_UseProps_Bombardment, AudioChannel.Voice);
                            break;
                        case ERoleType.JinZhuangYuan:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_UseProps_Bombardment, AudioChannel.Voice);
                            break;
                        case ERoleType.YaKeDong:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_UseProps_Bombardment, AudioChannel.Voice);
                            break;
                    }
                    break;
                case EPropType.AimAssist:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UseProps_AimAssist);
                    switch (roleType)
                    {
                        case ERoleType.ZiWei:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UseProps_AimAssist, AudioChannel.Voice);
                            break;
                        case ERoleType.DaMeiLi:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_UseProps_AimAssist, AudioChannel.Voice);
                            break;
                        case ERoleType.JinZhuangYuan:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_UseProps_AimAssist, AudioChannel.Voice);
                            break;
                        case ERoleType.YaKeDong:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_UseProps_AimAssist, AudioChannel.Voice);
                            break;
                    }
                    break;
                case EPropType.Trap:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UseProps_Trap);
                    switch (roleType)
                    {
                        case ERoleType.ZiWei:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UseProps_Trap, AudioChannel.Voice);
                            break;
                        case ERoleType.DaMeiLi:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_UseProps_Trap, AudioChannel.Voice);
                            break;
                        case ERoleType.JinZhuangYuan:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_UseProps_Trap, AudioChannel.Voice);
                            break;
                        case ERoleType.YaKeDong:
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_UseProps_Trap, AudioChannel.Voice);
                            break;
                    }
                    break;
            }
        }
        
        /// <summary>
        /// 丰收技使用音效播放音效
        /// </summary>
        /// <param name="args"></param>
        private void OnSkillStarted(SkillStartedEventArgs args)
        {
            var roundContext = HuntingAppFlow.Instance.GetCurrentRoundFlow().CurrentRoundContext;
            var roleType = roundContext.RoleData.RoleType;
            var hasLinkage = roundContext.HasLinkage;
            
            PlayRoleSkillAudioType(roleType, hasLinkage);   
            PlayRoleSkillVoiceAudioType(roleType);
        }
        
        /// <summary>
        /// 结算音效播放事件
        /// </summary>
        private void OnSettlementStarted()
        {
            var roundContext = HuntingAppFlow.Instance.GetCurrentRoundFlow().CurrentRoundContext;
            var roleType = roundContext.RoleData.RoleType;

            switch (roleType)
            {
                case ERoleType.ZiWei:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_Settlement, AudioChannel.Voice);
                    break;
                case ERoleType.DaMeiLi:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_Settlement, AudioChannel.Voice);
                    break;
                case ERoleType.JinZhuangYuan:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_Settlement, AudioChannel.Voice);
                    break;
                case ERoleType.YaKeDong:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_Settlement, AudioChannel.Voice);
                    break;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 根据角色播放对应角色使用丰收技语音
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        private void PlayRoleSkillVoiceAudioType(ERoleType roleType)
        {
            var random = UnityEngine.Random.Range(1, 3);
            HuntingAudioRefSo.HuntingGameAudioType audioType = default;
            switch (roleType)
            {
                case ERoleType.ZiWei:
                    audioType = random == 1
                        ? HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UseSkill_1
                        : HuntingAudioRefSo.HuntingGameAudioType.IP_ZiWei_UseSkill_2;
                    break;
                case ERoleType.DaMeiLi:
                    audioType = random == 1
                        ? HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_UseSkill_1
                        : HuntingAudioRefSo.HuntingGameAudioType.IP_DaMeiLi_UseSkill_2;
                    break;
                case ERoleType.JinZhuangYuan:
                    audioType = random == 1
                        ? HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_UseSkill_1
                        : HuntingAudioRefSo.HuntingGameAudioType.IP_JinZhuangYuan_UseSkill_2;
                    break;
                case ERoleType.YaKeDong:
                    audioType = random == 1
                        ? HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_UseSkill_1
                        : HuntingAudioRefSo.HuntingGameAudioType.IP_YaKeDong_UseSkill_2;
                    break;
            }
            PlaySound2D(audioType, AudioChannel.Voice);
        }

        /// <summary>
        /// 根据角色播放对应角色使用丰收技使用音效
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <param name="hasLinkage">是否为专属地图</param>
        /// <returns></returns>
        private void PlayRoleSkillAudioType(ERoleType roleType, bool hasLinkage)
        {
            switch (roleType)
            {
                case ERoleType.ZiWei:
                    if (hasLinkage)
                    {
                        var callback = PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Skill_ZiWeiUniqueMap_Before);
                        callback.AddCallback(async () =>
                        {
                            await UniTask.Delay(500);
                            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Skill_ZiWeiUniqueMap_After);
                        });
                    }
                    break;
                case ERoleType.DaMeiLi:
                    PlaySound2D(hasLinkage
                        ? HuntingAudioRefSo.HuntingGameAudioType.Skill_DaMeiLiUniqueMap
                        : HuntingAudioRefSo.HuntingGameAudioType.Skill_DaMeiLi);
                    break;
                case ERoleType.JinZhuangYuan:
                    PlaySound2D(hasLinkage
                        ? HuntingAudioRefSo.HuntingGameAudioType.Skill_JinZhuangYuan_UniqueMap
                        : HuntingAudioRefSo.HuntingGameAudioType.Skill_JinZhuangYuan);
                    break;
                case ERoleType.YaKeDong:
                    PlaySound2D(hasLinkage
                        ? HuntingAudioRefSo.HuntingGameAudioType.Skill_YaKeDong_UniqueMap
                        : HuntingAudioRefSo.HuntingGameAudioType.Skill_YaKeDong);
                    break;
            }
        }

        #endregion
    }
}