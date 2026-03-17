---
name: RoundFlow 模式化重构
overview: 将 RoundFlow 中的主地图与隐藏地图玩法拆分为 MainMapMode 与 HiddenMapMode，流程只做编排，玩法规则与表现归 Mode 管理。本 Plan 仅完成架构重构，不添加新功能。
todos:
  - id: 1
    content: 新建 IGameplayMode 接口与 IRoundResettable.ReInit(Map) 签名调整
    status: pending
  - id: 2
    content: 新建 MainMapMode，迁入肉满逻辑、正计时、结算打开
    status: pending
  - id: 3
    content: 新建 HiddenMapMode，迁入 Boss 全死逻辑、雪山进场与退场表现
    status: pending
  - id: 4
    content: RoundFlow 瘦身，改为持 CurrentMode、调用 Mode.Enter/Exit/Update
    status: pending
  - id: 5
    content: RoundFlow 新增 TransitionToNextMode，拆出 EnterHiddenMap 逻辑
    status: pending
  - id: 6
    content: 结算面板统一 onConfirm 注入，Manager ReInit 显式传 Map
    status: pending
  - id: 7
    content: UIGameplay 支持 SwitchToMode，删除 HiddenMapContext
    status: pending
isProject: true
---

# RoundFlow 模式化重构执行清单

**约束：** 保持现有代码风格与命名（#region、/// 注释、PascalCase/_camelCase），不删除注释。本 Plan 仅做架构重构，不实现未来功能（如 Normal 长汇总动画等）。

---

## 当前逻辑快览

| 入口 | 当前行为 |
|------|----------|
| EnterRound | 创建管理器、Init、打开 UIGameplay、倒计时、Playing |
| OnMeatScaleCompleted | 未满提示；满了倒计时→StartSettlement→Open SnowFake/Normal→CalculateReward |
| UIPopupSettlementSnowFake 按钮 | EnterHiddenMapAsync |
| UIPopupSettlementNormal 按钮 | EnterPrepareAsync（内部 EndRound） |
| UIPopupSettlementSnowVictory 按钮 | EnterPrepareAsync + Close UISnowMountainVictory |
| EnterHiddenMapAsync | HiddenMapContext.Init→假结算 Play→爆炸→Cleanup→LoadScene→ReInit→SwitchSnow→Boss 入场等→HiddenMap |
| HiddenMapContext.OnAnimalDeath | Boss 全死→EndHiddenMapAsync |
| EndHiddenMapAsync | 慢镜→停雪→UISnowMountainVictory→SnowVictory 结算→CalculateReward→Release |
| EndRound | Dispose→清池→关 UI→Load PrepareScene |
| OnRoundPlaying | MainMap 时累加 _roundElapsedTime；遍历 IRoundUpdatable.DoUpdate |

---

## 1. 新建 IGameplayMode 与 IRoundResettable 调整

### 1.1 新建 [Assets/Scripts/CoreGameLogic/Game/Round/Modes/IGameplayMode.cs](Assets/Scripts/CoreGameLogic/Game/Round/Modes/IGameplayMode.cs)

```csharp
void Enter();
UniTask Exit(IGameplayMode nextMode);  // nextMode 为 null 表示整局结束；MainMapMode 需 await 假结算动画
void Update(float dt);
```
RoundFlow.TransitionToNextMode 与 EndRound 需 await _currentMode.Exit(...)。

### 1.2 修改 [Assets/Scripts/CoreGameLogic/Game/Round/IRoundManager.cs](Assets/Scripts/CoreGameLogic/Game/Round/IRoundManager.cs) 中 IRoundResettable

将 `void ReInit(RoundContext context)` 改为 `void ReInit(Map activeMapData)`。

### 1.3 修改所有 IRoundResettable 实现类

- [AnimalManager](Assets/Scripts/CoreGameLogic/Managers/RoundManagers/AnimalManager.cs)：ReInit(Map map) → CacheMapSpeciesData(map.ID)
- [SpawnerManager](Assets/Scripts/CoreGameLogic/Managers/RoundManagers/SpawnerManager.cs)：ReInit(Map map) → _currentMapData = map
- [GameplaySceneItemManager](Assets/Scripts/CoreGameLogic/Managers/RoundManagers/GameplaySceneItemManager.cs)：ReInit 签名改为 Map，实现仍为 FindPlayer/FindPlayableArea
- 其余 ReInit 实现类：签名改为 ReInit(Map map)，内部按需用 map 或保持空实现

---

## 2. 新建 MainMapMode

### 2.1 新建 [Assets/Scripts/CoreGameLogic/Game/Round/Modes/MainMapMode.cs](Assets/Scripts/CoreGameLogic/Game/Round/Modes/MainMapMode.cs)

- 持有：RoundContext、EventManager、UIManager、RoundFlow 引用（或通过 GetRoundManager 获取）
- Enter：订阅 MeatScaleCompleted，配置 UIGameplay 主地图布局（显隐动物计数、肉条、返回、隐藏 Boss 血条）
- Update：累加 _elapsedTime，供 UIComponentTime 读取（RoundFlow 提供接口或事件）
- Exit(nextMode)：取消订阅 MeatScaleCompleted；若 nextMode 为 HiddenMapMode，执行假结算过渡（调用 SnowFake.PlayWindowShakeAsync/PlayButtonGlowAsync、爆炸、关面板）
- OnMeatScaleCompleted：未满→PlayTip；满了→倒计时→StartSettlement→Open SnowFake 或 Normal→CalculateReward，面板 onConfirm 由 MainMapMode 注入（HasHiddenMap 则 TransitionToNextMode，否则 EndRound）

### 2.2 RoundFlow 暴露计时

RoundFlow 保留 _roundElapsedTime，MainMapMode.Update 中累加，或由 RoundFlow.OnRoundPlaying 在 MainMap 时累加后供 UIComponentTime 读取。UIComponentTime 仍从 RoundFlow.RoundElapsedTime 读。

---

## 3. 新建 HiddenMapMode

### 3.1 新建 [Assets/Scripts/CoreGameLogic/Game/Round/Modes/HiddenMapMode.cs](Assets/Scripts/CoreGameLogic/Game/Round/Modes/HiddenMapMode.cs)

- 持有：RoundContext、EventManager、UIManager、EffectManager、SoundManager、RoundFlow、_snowEffect（或由 RoundFlow 持有后传入）
- Enter：订阅 AnimalEnteredDeath，配置 UIGameplay 雪山布局（SwitchSnow 逻辑迁入）
- Update：暂不推进倒计时（雪山不计时）；后续扩展在此加
- Exit(nextMode)：取消订阅；nextMode 必为 null
- OnBossAllDead：慢镜→停雪→UISnowMountainVictory 光效与全家福→Open UIPopupSettlementSnowVictory→CalculateReward；面板 onConfirm 注入 EndRound

---

## 4. RoundFlow 瘦身

### 4.1 成员调整

- 删除：_currentPhase、_currentHiddenMapContext、HiddenMapContext 类
- 新增：_currentMode（IGameplayMode）、_modeList（List<IGameplayMode>）、_modeIndex
- 保留：RoundFlowState（含 Settlement）、_roundElapsedTime、_roundManagers、_snowEffect（若 HiddenMapMode 不持有）

### 4.2 EnterRound 重写

- RegisterServices、RegisterEvents、CreateRoundManagers
- InitRoundManagers 传 context.MapData（需新增 InitRoundManagers(Map) 或从 context 取）
- 构建 _modeList：HasHiddenMap ? [MainMapMode, HiddenMapMode] : [MainMapMode]
- Open UIGameplay、SetClickable(false)
- Trigger RoundEntered
- 播放倒计时（可保留在 RoundFlow 或迁入 MainMapMode.Enter）
- SetClickable(true)、Trigger RoundStarted
- _currentMode = _modeList[0]、_currentMode.Enter()
- _currentState = Playing

### 4.3 DoUpdate

- Playing 时：若 _currentMode 非 null，调用 _currentMode.Update(dt)；保留 _roundElapsedTime 累加逻辑在 MainMap 阶段（或交由 MainMapMode 维护后由 RoundFlow 转发）
- 遍历 IRoundUpdatable.DoUpdate 保留

### 4.4 删除

- RegisterEvents/UnregisterEvents 中的 MeatScaleCompleted
- OnMeatScaleCompleted 全部逻辑迁入 MainMapMode

---

## 5. RoundFlow.TransitionToNextMode

### 5.1 新增方法

- _currentState = Transitioning
- _currentMode.Exit(_modeList[_modeIndex + 1])
- CleanupManagers、ClearAllEffects、ClearAllSounds、ClearAllPools
- LoadScene("GameplaySnowMountainScene")
- ReInitManagers(context.HiddenMapData)
- UIGameplay.SwitchToMode(HiddenMapMode) 或显式调用 SwitchSnow
- _currentMode = _modeList[++_modeIndex]
- _currentMode.Enter()
- _currentState = Playing
- Trigger HiddenMapEntered（HuntingSoundManager 等仍订阅）

### 5.2 MainMapMode.Exit(HiddenMapMode) 负责

- 获取已打开的 UIPopupSettlementSnowFake
- await PlayWindowShakeAsync、PlayButtonGlowAsync
- await 爆炸特效、音效
- Delay 200ms、Close UIPopupSettlementSnowFake
- 然后调用 RoundFlow.TransitionToNextMode()（或由 RoundFlow 在 Exit 返回后执行后续步骤，具体分工可二选一：要么 Exit 内做完表现再调 TransitionToNextMode，要么 RoundFlow 在 TransitionToNextMode 开头先调 Exit，Exit 内只做表现不调 TransitionToNextMode）

**推荐：** RoundFlow.TransitionToNextMode 被假结算按钮 onConfirm 调用；内部先调 CurrentMode.Exit(NextMode)，Exit 内执行 shake、glow、爆炸、关面板；然后 RoundFlow 继续 Cleanup、LoadScene、ReInit、NextMode.Enter。

---

## 6. 结算面板 onConfirm 注入

### 6.1 UIPopupSettlementNormal

- 新增 `Action onConfirm` 参数，在 OpenUIAsync 时传入，或在 OnInit(object userData) 中传入
- OnSettlementButtonClicked：Close；调用 onConfirm（由 MainMapMode 注入为 `() => HuntingAppFlow.Instance.EnterPrepareAsync().Forget()`，内部会 EndRound）

### 6.2 UIPopupSettlementSnowFake

- 新增 `Action onConfirm`
- OnButtonConfirmClicked：RemoveListener；调用 onConfirm（由 MainMapMode 注入为 `() => RoundFlow.Instance.TransitionToNextMode()`）
- 删除直接调用 EnterHiddenMapAsync

### 6.3 UIPopupSettlementSnowVictory

- 新增 `Action onConfirm`
- OnSettlementButtonClicked：Close；调用 onConfirm（由 HiddenMapMode 注入为 `() => HuntingAppFlow.Instance.EnterPrepareAsync()`）；Close UISnowMountainVictory 保留

### 6.4 打开面板时传入 onConfirm

MainMapMode 打开 SnowFake：`await _uiManager.OpenUIAsync<UIPopupSettlementSnowFake>(..., onConfirm: () => RoundFlow.Instance.TransitionToNextMode())`  
需确认 UIManager.OpenUIAsync 是否支持 userData 传 onConfirm；若不支持，可在 OnInit 时通过事件或静态/单例设置。

---

## 7. UIGameplay.SwitchToMode 与 HiddenMapContext 删除

### 7.1 UIGameplay

- 新增 `SwitchToMode(IGameplayMode mode)` 或 `SwitchToMode(RoundPhase phase)`：若为 HiddenMap，执行现有 SwitchSnow() 逻辑；若为 MainMap，恢复主地图布局
- 或保留 SwitchSnow()，由 RoundFlow/TransitionToNextMode 直接调用

### 7.2 删除 HiddenMapContext

- 删除 [RoundFlow.cs](Assets/Scripts/CoreGameLogic/Game/Round/RoundFlow.cs) 中的 HiddenMapContext 类定义
- Boss 全死逻辑迁入 HiddenMapMode.OnBossAllDead

---

## 8. RoundFlow.EndRound 调整

- 若 _currentMode 非 null：_currentMode.Exit(null)
- DisposeRoundManagers、Clear、关 UIGameplay、UnregisterEvents、Load PrepareScene
- _currentMode = null、_currentState = None

---

## 9. 引用与事件

- HiddenMapEvents.HiddenMapEntered：保留，在 TransitionToNextMode 内 NextMode.Enter 之后 Trigger
- UIComponentTime：继续从 RoundFlow.RoundElapsedTime 读；RoundFlow 在 MainMap 阶段累加，HiddenMap 阶段不累加（或 HiddenMapMode 维护独立计时，UI 从 Mode 取，本次可先保持从 RoundFlow 读，HiddenMap 时 _roundElapsedTime 不变）
- HuntingSoundManager.OnHiddenMapEntered：不变
- Settlement 状态：保留，StartSettlement 仍设 Settlement；DoUpdate 在 Settlement 时不跑玩法 Update

---

## 10. 执行顺序建议

1. IRoundResettable.ReInit(Map) 签名与实现
2. IGameplayMode 接口
3. MainMapMode（先实现 Enter、OnMeatScaleCompleted、Exit 骨架）
4. RoundFlow 删除 OnMeatScaleCompleted、改为持 Mode、EnterRound 调 MainMapMode.Enter
5. 结算面板 onConfirm 注入，MainMapMode 打开面板时传入
6. HiddenMapMode
7. TransitionToNextMode，MainMapMode.Exit(HiddenMap) 做假结算表现
8. 删除 HiddenMapContext，HiddenMapMode 订阅 AnimalEnteredDeath
9. EndRound 调 CurrentMode.Exit(null)
10. UIGameplay SwitchToMode（可选，或直接保留 SwitchSnow 由 RoundFlow 调）

---

## 11. 不涉及的改动（留给后续）

- Normal 面板长汇总动画
- SnowFake 假汇总动画
- 雪山倒计时规则
- SettlementRewardManager 计算逻辑变更
- 新结算面板类型
