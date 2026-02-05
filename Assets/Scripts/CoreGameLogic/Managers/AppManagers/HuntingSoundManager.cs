using System;
using System.Collections.Generic;
using cfg.HuntingConfig.Enum;
using Cysharp.Threading.Tasks;
using GameFramework.Core.Audio;
using GameFramework.Game;
using GameFramework.Manager;
using Hunting.Events;
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
        /// <returns></returns>
        public AudioCallback PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType audioType, AudioChannel channel = AudioChannel.Sound
            , int loop = 1)
        {
            var audioRef = _huntingAudioRefSo.GetAudioRefFromType(audioType);
            var audioClip = audioRef.clip;
            var volume = audioRef.volume;
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
            _eventManager.AddListener(RoundEvents.RoundEntered, OnRoundEntered);
            _eventManager.AddListener(BulletEvents.BulletHit, OnBulletHit);
            _eventManager.AddListener(AnimalEvents.AnimalDied, OnAnimalDied);
            _eventManager.AddListener(PropEvents.TrapTriggered, OnTrapTriggered);
            _eventManager.AddListener(HiddenMapEvents.HiddenMapEntered, OnHiddenMapEntered);
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
            _eventManager.RemoveListener(RoundEvents.RoundEntered, OnRoundEntered);
            _eventManager.RemoveListener(BulletEvents.BulletHit, OnBulletHit);
            _eventManager.RemoveListener(AnimalEvents.AnimalDied, OnAnimalDied);
        }

        /// <summary>
        /// 角色选择结束音效播放事件
        /// </summary>
        /// <param name="args"></param>
        private void OnRoleSelectedEnd(RoleSelectedOverEventArgs args)
        {
            var roleType = args.RoleType;
            PlayIPOpening(roleType);
        }
        
        /// <summary>
        /// 开局播放开场白事件
        /// </summary>
        /// <param name="args"></param>
        private void OnRoundEntered(RoundEnteredEventArgs args)
        {
            var roundContext = args.RoundContext;
            var roleType = roundContext.RoleData.RoleType;
            var hasLinkage = roundContext.HasLinkage;

            PlayMapEnvSound(roundContext);
            PlayRoleOpening(roleType, hasLinkage);
        }
        
        /// <summary>
        /// 子弹命中播放音效事件
        /// </summary>
        /// <param name="args"></param>
        private void OnBulletHit(BulletHitEventArgs args)
        {
            var bulletType = args.BulletData.BulletType;
            PlayBulletHitSound(bulletType);
            PlayAnimalHitSound();
        }
        
        /// <summary>
        /// 动物死亡音效播放事件
        /// </summary>
        /// <param name="args"></param>
        private void OnAnimalDied(AnimalDiedEventArgs args)
        {
            PlayAnimalDeadSound();
        }
        
        /// <summary>
        /// 陷阱触发播放音效事件
        /// </summary>
        /// <param name="args"></param>
        private void OnTrapTriggered(TrapTriggeredEventArgs args)
        {
            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Props_TrapCatch);
        }
        
        private void OnHiddenMapEntered()
        {
            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.MapEnv_SnowMountain);
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

            PlayPropSoundAndIPVoice(propType, roleType);
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

            PlayCommonSkillTriggerSound();
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
        /// 播放地图环境应
        /// </summary>
        /// <param name="roundContext"></param>
        private void PlayMapEnvSound(RoundContext roundContext)
        {
            var mapType = roundContext.MapData.MapType;
            switch (mapType)
            {
                case EMapType.Beach:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.MapEnv_Beatch);
                    break;
                case EMapType.Forest:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.MapEnv_RoyalForest);
                    break;
                case EMapType.Garden:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.MapEnv_PersonalGarden);
                    break;
                case EMapType.Grassland:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.MapEnv_GrassLand);
                    break;
                case EMapType.Hidden:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.MapEnv_SnowMountain);
                    break;
            }
        }

        /// <summary>
        /// 播放角色开场白
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="hasLinkage"></param>
        private void PlayRoleOpening(ERoleType roleType, bool hasLinkage)
        {
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
                    else
                    {
                        PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Skill_ZiWei);
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
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Skill_YaKeDong);
                    if (hasLinkage)
                    {
                        PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Skill_YaKeDong_UniqueMap);
                    }

                    break;
                case ERoleType.Bule:
                case ERoleType.Red:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Skill_BlueRed);
                    break;
            }
        }
        
         /// <summary>
        /// 播放道具使用音效（包括IP人物语音）
        /// </summary>
        /// <param name="propType"></param>
        /// <param name="roleType"></param>
        private void PlayPropSoundAndIPVoice(EPropType propType, ERoleType roleType)
        {
            PlayPropSound(propType);
            PlayPropVoiceSound(propType, roleType);
        }

        /// <summary>
        /// 播放道具使用音效
        /// </summary>
        private void PlayPropSound(EPropType propType)
        {
            switch (propType)
            {
                case EPropType.Bombardment:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Props_Bombardment);
                    break;
                case EPropType.AimAssist:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Props_AimAssist);
                    break;
                case EPropType.Trap:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Props_Trap);
                    break;
            }
        }
        
        private void PlayPropVoiceSound(EPropType propType, ERoleType roleType)
        {
            switch (propType)
            {
                case EPropType.Bombardment:
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
        /// 播放IP人物开场白
        /// </summary>
        /// <param name="roleType"></param>
        private void PlayIPOpening(ERoleType roleType)
        {
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
        /// 播放子弹命中音效
        /// </summary>
        /// <param name="bulletType"></param>
        private void PlayBulletHitSound(EBulletType bulletType)
        {
            switch (bulletType)
            {
                case EBulletType.Normal:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.BulletHit_Default);
                    break;
                case EBulletType.Explosive:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.BulletHit_Explosive);
                    break;
                case EBulletType.HighDamage:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.BulletHit_HighDamage);
                    break;
                case EBulletType.HighSpeed:
                    PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.BulletHit_HighSpeed);
                    break;
            }
        }
        
        /// <summary>
        /// 播放动物受击音效
        /// </summary>
        private void PlayAnimalHitSound()
        {
            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Animal_Hit);
        }
        
        /// <summary>
        /// 播放动物死亡音效
        /// </summary>
        private void PlayAnimalDeadSound()
        {
            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Animal_Death);
        }

        /// <summary>
        /// 播放通用丰收技触发音效
        /// </summary>
        private void PlayCommonSkillTriggerSound()
        {
            PlaySound2D(HuntingAudioRefSo.HuntingGameAudioType.Skill_Use);
        }
        
        #endregion
    }
}