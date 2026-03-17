using Cysharp.Threading.Tasks;
using GameFramework.Network.Proxy;

namespace CoreGameLogic.Net
{
    /// <summary>
    /// 打猎项目 GameProxy
    /// </summary>
    public class HuntingGameServiceProxy : BaseGameServiceProxy
    {
        public static HuntingGameServiceProxy Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HuntingGameServiceProxy();
                }

                return _instance;
            }
        }

        private static HuntingGameServiceProxy _instance;
        
        protected override string GameId { get; } = "Hunting";
        protected override string GetServerUrl()
        {
            // 开发域 https://dev.sanqianpan.com:10888
            // 生产域 https://dev.sanqianpan.com:10000
            // if (GooseCatcherGame.Instance.IsDev)
            // {
            //     return "https://dev.sanqianpan.com:10888";
            // }
            return "https://dev.sanqianpan.com:10888";
        }

        protected override void InitProxies()
        {
            UserProxy.Instance.Init();
            GameProxy.Instance.Init();
            GameConfigProxy.Instance.Init();
            PaymentProxy.Instance.Init();
            // GoosePaymentSetup.Init();
            HuntingConfigSetup.Init();
            SocialProxy.Instance.Init();
            // GooseWaveProxy.Instance.Init();
        }

        protected override async UniTask OnLoginSuccess(S2CLogin loginResp)
        {
            // 登录成功后立即获取用户资料和资产
            await UserProxy.Instance.GetProfile();
            await UserProxy.Instance.GetCurrencyData();
            await UserProxy.Instance.GetAddresses();
            await GameProxy.Instance.GetItemData();
            await PaymentProxy.Instance.RequestMallItems();
            await GameConfigProxy.Instance.FetchAllConfigs();

            UserProxy.Instance.StartPingTimer();
        }
        
        public override void Dispose()
        {
            UserProxy.Instance.Dispose();
            base.Dispose();
        }
    }
}