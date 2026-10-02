# 打开与运行

[首页](../README.md) · [玩法](gameplay.md) · [架构](architecture.md) · [**打开与运行**](getting-started.md) · [目录导读](project-structure.md)

本篇说明如何在本机打开工程并运行，以及运行前需要补齐哪些内容。

## 环境要求

| 项 | 要求 |
|---|---|
| Unity | 2022.3.7f1c1（见 `ProjectSettings/ProjectVersion.txt`） |
| 网络 | 首次打开需能访问 `github.com`、`gitee.com`、`package.openupm.com`，用于还原包依赖 |

## 取得代码

```bash
git clone https://github.com/greenballyjq/HuntingClient.git
```

克隆后的仓库不含以下内容，运行前需要补齐：

| 路径 | 内容 | 未包含的原因 |
|---|---|---|
| `Assets/Plugins/Demigiant` | DOTween | `.gitignore` 排除 `Assets/Plugins/` |
| `Assets/Plugins/Spine` | Spine 运行时 | 同上 |
| `Assets/GameFramework` | 自研框架源码 | 仓库内是一条符号链接，指向框架仓库的源码目录 |
| `Assets/StreamingAssets/yoo/` | 资源包构建产物 | `.gitignore` 排除 |

## 首次打开

1. 打开 Unity Hub，添加克隆下来的工程目录。
2. 用 2022.3.7f1c1 打开该工程。
3. 首次打开会导入资源，并还原 `Packages/manifest.json` 中的包依赖。其中三个依赖指向 git 仓库，一个依赖来自 openupm 的 registry，都需要联网。
4. 打开完成后检查 Console。若出现找不到 `DG.Tweening` 或 `Spine` 命名空间的报错，说明第三方库还没补齐，见下一节。

## 补齐缺失内容

### 第三方库

工程内有 12 个脚本引用 DOTween，1 个脚本引用 Spine，缺任一都会编译失败。

| 库 | 放置位置 | 版本 |
|---|---|---|
| DOTween | `Assets/Plugins/Demigiant` | 本地文件中未标注版本号 |
| Spine | `Assets/Plugins/Spine` | spine-unity 4.2.112（见 `version.txt`），可读取 Spine Editor 4.2.xx 导出的数据 |

### 框架源码

`Assets/GameFramework` 在版本库中是一条符号链接（git 记录模式 120000），指向路径为作者的本地目录，克隆后为断链。

把框架仓库 [GameFrame](https://github.com/greenballyjq/GameFrame) 中的框架源码放到 `Assets/GameFramework`，或在本机建立一条指向该仓库源码目录的符号链接。

### 资源包

资源加载模式在 `BootstrapScene` 的框架组件上设置，值为 YooAsset。加载器按平台分支，是否需要资源包构建产物由此决定：

| 平台 | 初始化模式 | 是否需要构建产物 |
|---|---|---|
| 编辑器 | EditorSimulateMode | 否，直接读取工程内的资源 |
| WebGL | WebPlayMode | 是 |
| 其他平台 | OfflinePlayMode | 是，读取内置资源包 |

因此在编辑器中运行不需要 `Assets/StreamingAssets/yoo/`。构建播放器时需要的资源包由 `Assets/Resources/AssetBundleCollectorSetting.asset` 与 `Assets/Resources/YooAssetSettings.asset` 配置，输出目录为 `Assets/StreamingAssets/yoo/`。

## 运行游戏

1. 打开 `Assets/Scenes/BootstrapScene.unity`。
2. 点 Play。框架初始化完成后进入准备阶段界面。

操作方式：

| 操作 | 方式 |
|---|---|
| 界面点击 | 指针（鼠标左键或触摸） |
| 移动 | 摇杆。进入玩法后控制方式切为摇杆 |
| 射击 | 按住指针持续开火，松开停火 |

部分玩法会切换到"选择目标"模式，此模式下不带"按住射击"的语义，改为点选目标。

## 第三方依赖

完整清单见 `Packages/manifest.json`。

| 包 | 版本 / 来源 | 是否随仓库 |
|---|---|---|
| `com.unity.render-pipelines.universal` | 14.0.8 | 是（由 Package Manager 还原） |
| `com.unity.inputsystem` | 1.6.3 | 是（由 Package Manager 还原） |
| `com.unity.2d.animation` | 9.0.4 | 是（由 Package Manager 还原） |
| `com.unity.2d.sprite` | 1.0.0 | 是（由 Package Manager 还原） |
| `com.unity.textmeshpro` | 3.0.6 | 是（由 Package Manager 还原） |
| `com.unity.postprocessing` | 3.5.0 | 是（由 Package Manager 还原） |
| `com.unity.timeline` | 1.7.5 | 是（由 Package Manager 还原） |
| `com.unity.recorder` | 4.0.1 | 是（由 Package Manager 还原） |
| `com.unity.visualscripting` | 1.8.0 | 是（由 Package Manager 还原） |
| `com.unity.test-framework` | 1.1.33 | 是（由 Package Manager 还原） |
| `com.tuyoogame.yooasset` | 2.3.14（registry `package.openupm.com`） | 是（由 Package Manager 还原） |
| `com.cysharp.unitask` | git `Cysharp/UniTask` | 否，还原时联网拉取 |
| `com.code-philosophy.luban` | git `focus-creative-games/luban_unity` | 否，还原时联网拉取 |
| `com.coplaydev.unity-mcp` | git `CoplayDev/unity-mcp` v10.1.2 | 否，还原时联网拉取。开发期工具，不参与运行时 |
| DOTween | 本地文件 | 否，需自行补齐，见上文 |
| Spine | spine-unity 4.2.112 | 否，需自行补齐，见上文 |

## 数据与文件位置

| 内容 | 位置 |
|---|---|
| 配表数据 | `Assets/Resources/Bundle/Raw/Configs/bytes/`，20 张表。运行时用 Unity 的 `Resources.Load` 读取，与资源加载模式无关。这些表的定义与生成工具在独立仓库 [HuntingConfig](https://github.com/greenballyjq/HuntingConfig)，运行游戏不需要它 |
| 输入配置 | `Assets/InputSystem/GameInputActions.inputactions`、`Assets/InputSystem/InputSystem.inputsettings.asset` |
| 存档 | `Application.persistentDataPath` 下的 `player_save.json`，明文 JSON。Windows 下该目录按 companyName 与 productName 拼为 `%USERPROFILE%\AppData\LocalLow\com.sanqianpan\Hunting\` |
| 资源包 | `Assets/StreamingAssets/yoo/DefaultPackage/`（不进版本库） |
| 场景 | `Assets/Scenes/`，共 4 个 |

## 常见问题

### Unity 提示版本不一致，或工程打不开

原因：本机编辑器版本不是 2022.3.7f1c1。

解决：在 Unity Hub 中安装 2022.3.7f1c1，再打开工程。

### Console 报找不到 `DG.Tweening` 或 `Spine`

原因：`Assets/Plugins` 下的第三方库未补齐，工程内有 12 个脚本引用前者、1 个引用后者。

解决：按"补齐缺失内容"一节放回 DOTween 与 Spine。

### 包还原失败，拉不到 `com.cysharp.unitask` 等依赖

原因：`Packages/manifest.json` 中有三个依赖指向 git 仓库（`github.com`、`gitee.com`），另有一个依赖来自 `package.openupm.com`。网络不通时还原失败。

解决：确认这三个域名可访问，或配置代理后重新打开工程。

### `Assets/GameFramework` 目录为空或提示缺少脚本

原因：该路径是符号链接，克隆后断链。

解决：按"补齐缺失内容"一节放入框架源码。

### 编辑器里能跑，构建出来的包黑屏或资源加载失败

原因：非编辑器平台走 OfflinePlayMode，需要内置资源包；仓库不含 `Assets/StreamingAssets/yoo/`。

解决：先打包生成资源包，再构建播放器。

### 想重置进度

原因：进度存在 `player_save.json` 中。

解决：删除该文件。位置见"数据与文件位置"。
