using Cysharp.Threading.Tasks;
using GameFramework.Network;
using GameFramework.Network.Proxy;
using UnityEngine;

namespace CoreGameLogic.Net
{
    /// <summary>
    /// 打猎特有的上传结算结果 Proxy - 处理上传结算结果
    /// </summary>
    public class HuntingSettlementProxy : BaseProxy<HuntingSettlementProxy>
    {

        /// <summary>
        /// 上传结算结果
        /// </summary>
        /// <param name="point">积分</param>
        /// <param name="threeKp">三币获取</param>
        /// <param name="adCustomId">广告验证id</param>
        public async UniTask UploadSettlement(int point, int threeKp, string adCustomId = "")
        {
            Debug.Log($"[HuntingSettlementProxy] 开始上传结算结果：point: {point}, threeKp: {threeKp}, adCustomId: {adCustomId}");
            
            S2CGame resp = await SendSettlementResult(point, threeKp, adCustomId);
            
            if (resp != null && resp.Code == S2COpcode.OpWxadVerifyPending)
            {
                Debug.Log($"[HuntingSettlementProxy] 广告验证未就绪，1s 后重试");
                await UniTask.Delay(1000, ignoreTimeScale: true);
                resp = await SendSettlementResult(point, threeKp, adCustomId);
            }
            
            if (resp != null && resp.Code == S2COpcode.OpSuccess && resp.WaveResult != null)
            {
                UserProxy.Instance.SetThreekp(resp.WaveResult.ThreekpTotal);
                Debug.Log($"[HuntingSettlementProxy] 上传波次结果成功, pointTotal: {resp.WaveResult.PointTotal}, threekpTotal: {resp.WaveResult.ThreekpTotal}");
                await GameProxy.Instance.GetItemData();
            }
            else
            {
                Debug.LogError($"[HuntingSettlementProxy] 上传波次结果失败, code: {resp?.Code}");
            }
        }

        private async UniTask<S2CGame> SendSettlementResult(int point, int threeKp, string adCustomId)
        {
            var root = new C2SGame
            {
                Session = ServerClient.NextSession(),
                WaveResult = new C2SWaveResult
                {
                    // WaveId = (uint)waveId,
                    PointGain = (ulong)point,
                    ThreekpGain = (ulong)threeKp,
                    // Win = win,
                    AdCustomId = adCustomId ?? "",
                }
            };
            int delayMs = string.IsNullOrEmpty(adCustomId) ? 0 : ServerClient.AD_VERIFY_DELAY_MS;
            return await ServerClient.PostDomainAsync<S2CGame>("/game", root, delayMs);
        }
    }
}