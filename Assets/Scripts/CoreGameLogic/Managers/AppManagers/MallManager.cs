using System.Collections.Generic;
using System.Linq;
using CoreGameLogic.Net;
using Cysharp.Threading.Tasks;
using GameFramework.Mall;
using GameFramework.Network.Models.Dto;
using GameFramework.Network.Models.Vo;
using GameFramework.Network.Proxy;
using LitJson;

namespace CoreGameLogic.Managers.AppManagers
{
    public class MallManager : BaseAppMallManager<MallItemDto, MallItemVo>
    {
        /// <summary>
        /// 物品dto列表（从 MallItemDto 转换）
        /// </summary>
        private List<ItemDto> _cachedItemDtoList;

        /// <summary>
        /// 物品id-物品dto字典
        /// </summary>
        private Dictionary<string, ItemDto> _cachedItemDtoDict;

        /// <summary>
        /// 物品vo列表
        /// </summary>
        private List<ItemVo> _cachedItemVoList;

        /// <summary>
        /// 道具物品vo列表
        /// </summary>
        private List<ItemVo> _cachedPropItemVoList;

        private HuntingGameServiceProxy _proxy;

        public override void Init()
        {
            _proxy = HuntingGameServiceProxy.Instance;
            base.Init();
        }
        
        public override async UniTask FetchRemoteMallItemData()
        {
            await base.FetchRemoteMallItemData();

            if (FetchedMallItemDtoList == null) return;

            // 转换为 ItemDto
            _cachedItemDtoList = GetVirtualMallItemDtoList().Select(dto => new ItemDto(dto)).ToList();

            // 转换为 ItemVo
            _cachedItemVoList = GetVirtualMallItemVoList()?.Select(vo => new ItemVo(vo)).ToList();

            // 转换为道具物品vo列表
            _cachedPropItemVoList = GetVirtualMallItemVoList()?.Where(vo => !vo.IsVideoReward)
                .Select(vo => new ItemVo(vo)).ToList();
            
            IsFetchedMallItemSuccess = true;
        }

        public List<ItemDto> GetItemDtoList() => _cachedItemDtoList;
        public List<ItemVo> GetItemVoList() => _cachedItemVoList;

        public List<ItemVo> GetPropItemVoList() => _cachedPropItemVoList; 

        public List<MallItemDto> GetVirtualMallItemDtoList() =>
            FetchedMallItemDtoList?.Where(vo => vo.SubType == "Virtual").OrderBy(vo => vo.SortOrder).ToList();

        public List<MallItemVo> GetVirtualMallItemVoList() =>
            CachedMallItemVoList?.Where(vo => vo.SubType == "Virtual").OrderBy(vo => vo.SortOrder).ToList();

        public List<MallItemVo> GetPhysicalMallItemVoList() =>
            CachedMallItemVoList?.Where(vo => vo.SubType == "Physical").OrderBy(vo => vo.SortOrder).ToList();

        protected override List<MallItemDto> DeserializeMallItems(string json)
        {
            JsonData jsonData = JsonMapper.ToObject(json);
            List<MallItemDto> mallDataList = new List<MallItemDto>();
            for (int i = 0; i < jsonData.Count; i++)
            {
                JsonData itemData = jsonData[i];
                MallItemDto dto = new MallItemDto
                {
                    ItemID = (string)itemData["ItemID"],
                    Description = (string)itemData["Description"],
                    DisplayStatus = (bool)itemData["DisplayStatus"],
                    Icon = (string)itemData["Icon"],
                    IsVideoReward = itemData.ContainsKey("IsVideoReward") ? (bool)itemData["IsVideoReward"] : false,
                    MainType = (string)itemData["MainType"],
                    SubType = (string)itemData["SubType"],
                    Name = (string)itemData["Name"],
                    PriceCoin = (int)itemData["PriceCoin"],
                    SortOrder = (int)itemData["SortOrder"],
                };
                mallDataList.Add(dto);
            }

            return mallDataList;
        }
        
        protected override async UniTask<(string address, string phone)?> GetShippingInfo()
        {
            List<AddressInfo> addressInfos = await UserProxy.Instance.GetAddresses();
            if (addressInfos == null || addressInfos.Count == 0)
            {
                return null;
            }

            AddressInfo addressInfo = addressInfos[0];
            return (addressInfo.Address, addressInfo.Phone);
        }

        protected override async UniTask<MallItemVo> ConvertDtoToVo(MallItemDto dto)
        {
            var vo = new MallItemVo(dto);
            var sprite = await LoadSpriteFromUrl(dto.Icon);
            vo.Sprite = sprite;
            return vo;
        }

        protected override bool IsClientLoggedIn()
        {
            return _proxy.IsClientLoggedIn;
        }
        
        // public List<ItemVo> GetVirtualItemVoList() => _cachedItemVoList.Where(x => x.);
    }
}