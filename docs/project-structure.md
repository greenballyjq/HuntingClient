# 目录导读

本篇说明各目录放什么，以及想改某处该去哪个文件。读者是准备在这个工程上动手改东西的人；结构本身见 [架构](architecture.md)。

## 工程顶层

| 目录 / 文件 | 放什么 |
|---|---|
| `Assets/Scenes/` | 4 个场景。运行起点是 `BootstrapScene` |
| `Assets/Scripts/` | 全部游戏脚本，见下一节 |
| `Assets/Arts/` | 美术资源：音频、通用、特效、环境、模型、预制体、界面 |
| `Assets/Arts/SO/` | 配表用到的资源表：`Refs` 是各张表到美术资源的引用，`Cues` 是音频线索，`Settings` 是音频与界面设置 |
| `Assets/ArtsTemp/` | 与 `Arts` 同构的临时美术目录 |
| `Assets/Resources/` | 走 Unity 资源系统的内容。配表数据在 `Bundle/Raw/Configs/bytes`；打包与资源加载的设置也在这一层 |
| `Assets/InputSystem/` | 输入动作资产与它生成的脚本 |
| `Assets/Plugins/` | 第三方库（DOTween、Spine）。不随仓库分发，需自行补齐 |
| `Assets/GameFramework/` | 框架仓库的符号链接，本仓库不含其源码 |
| `Assets/StreamingAssets/` | 资源包构建产物。不随仓库分发 |
| `Packages/` | 包依赖清单 |
| `ProjectSettings/` | 工程设置。场景清单与顺序在 `EditorBuildSettings.asset` |
| `Bundles/` | 本地构建出的资源包，按平台分目录。不进版本库 |
| `docs/` | 这几篇文档与截图 |

## 脚本

```text
Assets/Scripts/
├── CoreGameLogic/          玩法与流程
│   ├── Events/             事件键与事件参数；RoundEvents/ 是一局内各系统的事件
│   ├── Game/               应用流程与回合流程的入口
│   │   ├── Jobs/           三次转场的任务
│   │   └── Round/          一局内的玩法，见下
│   ├── Managers/           管理器
│   │   ├── AppManagers/    跨局存在：配置、存档、玩家数据、输入、相机、转场
│   │   └── RoundManagers/  一局内存在：各玩法系统的宿主
│   ├── Save/               存档结构与序列化
│   └── UI/                 界面：Prepare/ 准备、Gameplay/ 局内、Transition/ 转场
├── GameConfig/             配表
│   ├── gen/                生成的表类型（由 Luban 生成，不要手改）
│   └── SO/                 资源引用表的类型定义
└── Tools/                  与玩法无关的工具
    ├── AreaShape/          区域形状
    ├── AreaTrigger/        区域触发器
    └── Math/               弹道计算
```

根目录下的两个文件不属于任何模块：`GameServiceLocator` 是三层管理器的取用入口，`AudioWait` 是等一条音效播完的工具。

## 一局内的玩法

`CoreGameLogic/Game/Round/` 下按玩法系统分目录，一个目录就是一套系统：

| 目录 | 放什么 |
|---|---|
| `Round` 根 | 回合流程本体，以及规则接口（计时规则、界面可见性规则） |
| `Modes/` | 主地图与隐藏地图两种模式的流程与计时 |
| `AnimalRefactor/` | 动物：行为、状态机、移动策略、生命、刷怪器、表现 |
| `Weapons/` | 玩家武器与技能武器 |
| `Bullets/` | 子弹运动，`Effects/` 是命中效果 |
| `Skills/` | 技能 |
| `Props/` | 道具，含陷阱 |
| `Quests/` | 任务 |
| `Luckys/` | 幸运增益 |
| `PlayerControls/` | 三种操作方式 |
| `Numeric/` | 数值修改层 |

每套系统都是同一个形状：一个基类定义阶段与生命周期，一个工厂按类型创建具体实现。加一种新技能、道具、任务或增益，都是加一个实现类再在工厂里登记。

## 想改某处，去哪

| 想做的事 | 去哪 |
|---|---|
| 改一局的进入与退出步骤 | `Game/Round/RoundFlow.cs` |
| 改主地图或隐藏地图的流程、计时、结束条件 | `Game/Round/Modes/` |
| 加一次新的转场 | 在 `Game/Jobs/` 加一个任务，并在转场入口 `Managers/AppManagers/TransitionGate.cs` 之外提供调用 |
| 改存档里存什么 | `Save/PlayerSaveData.cs` 与 `Managers/AppManagers/PlayerDataManager.cs` |
| 改某个数值的算法，或加一项可被增益修改的数值 | `Game/Round/Numeric/` |
| 改动物行为、状态或移动方式 | `Game/Round/AnimalRefactor/` |
| 改子弹命中效果 | `Game/Round/Bullets/Effects/` |
| 加一个技能 / 道具 / 任务 / 增益 | `Game/Round/` 下对应目录，加实现类并在该目录的工厂里登记；数值在配表 |
| 改操作方式，或加一种 | `Game/Round/PlayerControls/` |
| 改某个界面 | `UI/` 下对应阶段的目录 |
| 加一个界面 | `UI/` 下对应阶段，界面类上用特性声明层级与生命周期 |
| 改开局前棋子走格的规则 | `UI/Prepare/UIComponentRollRole.cs` |
| 改每帧驱动的顺序 | 各管理器自己的 `DoUpdate`，回合流程只负责按能力接口分派 |
| 改配表结构或数值 | 配表仓库 HuntingConfig；生成后把结果复制回本工程的代码目录与 `Assets/Resources/Bundle/Raw/Configs/bytes`。流程见 [README](../README.md) |
| 改资源打包或加载方式 | `Assets/Resources/` 下的打包与资源设置，以及启动场景上的资源加载模式 |
| 改键位与输入动作 | `Assets/InputSystem/` |
| 改区域形状或弹道算法 | `Assets/Scripts/Tools/` |
| 换框架 | `Assets/GameFramework`（符号链接）指向的仓库 |
