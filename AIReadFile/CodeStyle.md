我们项目的代码风格规范，这套规范是通用规范，以后还会介绍特有代码风格规范
1.代码注释规范
1)文档注释：对各种字段、属性、方法、类名、枚举名、接口名等使用///文档注释，无论访问修饰符是什么。
2)方法内注释：在方法内部使用//进行适量、必要的注释，确保逻辑清晰，不能不写。
2.调试日志规范
在关键逻辑处使用Debug.Log(), Debug.LogWarning(),Debug.LogError()输出调试信息。
日志消息需包含清晰的上下文，格式通常为：[ClassName]描述性消息。
举例：
Debug.Log("[GameLogic] 开始初始化游戏");
Debug.LogWarning($"[SpeciesSpawner] 物种 {specieId} 配置不存在");

3.代码区域整理
使用#region和#endregion对代码进行分块，使结构清晰。不需要过度使用，一般用个3~4个就可以。
举例：
#region 公共方法
// ... 相关方法
#endregion
#region 私有方法
// ... 相关方法
#endregion
#region 事件相关
// ... 相关方法
#endregion
#region 调试相关
// ... 相关方法
#endregion

4.事件系统规范
1)统一事件中心：所有事件的订阅与触发，必须通过EventManager事件管理器。
2)强类型事件：使用EventKey和EventKey<T>类来定义事件键，实现强类型通信。
3)事件定义文件：
在内部定义该模块的所有事件键（EventKey/EventKey<T>）和事件参数类。
4)命名规范：
事件键（EventKey）：使用PascalCase命名，描述已发生的事，如SpeciesSpawned。
事件参数类：统一命名为XxxEventArgs并继承EventArgs，如SpeciesSpawnEventArgs。
5)触发与订阅命名规范：
a.触发方：在名为TriggerXXX的方法中触发事件，若事件需要参数，则TriggerXXX的方法需要参数。方法名不能是NotifyXXX、RequestXXX等方法名。
举例：
private void TriggerSpeciesSpawn(SpeciesSpawnEventArgs args)
{
    GameServiceLocator.Event.Trigger(SpawnEvents.SpeciesSpawned, args);
}
b.订阅方：根据情况在Awake()、Start()、Init()、OnInit()、OnShow()等初始化方法中订阅事件；在OnDestroy()、Release()、OnClose()、OnHide()、CleanUp()等清理方法中取消订阅。
UI订阅和取消订阅事件，如果是UI事件适合在Awake()、OnDestory()等方法中，如果是自定义事件，适合在类似Init()、OnInit()、OnShow()、OnClose()、OnHide()、CleanUp()等中
订阅方在订阅事件时传入的回调函数名需要叫OnXXX()
7.服务访问规范
统一访问点：除GameLogic及其子类内部外，所有服务与管理器均应通过 GameServiceLocator获取。
推荐使用方式：在类的私有属性中定义快捷访问方式，以提高代码可读性和编写效率。
private EventManager Event => GameServiceLocator.Event;
private ResourceManager Resource => GameServiceLocator.Resource;

下面是UI的特有代码风格规范以及命名规范（字段、属性、方法名等标识符的取名风格）
请先看@UIBase.cs和@UIManager.cs以及@IUIComponent.cs两个文件
UIBase挂载在代表整个UI预制体的游戏对象上，由UIManager管理，界面级别的复杂的UI会使用。
UIBase上的复杂组件，实现IUIComponent接口后被UIBase管理。

UI事件一般在Awake() Destory()订阅和取消，如果是频繁关闭打开的UIBase，自定义事件在OnInit() 与OnClose()订阅，反之在OnShow() OnHide()订阅

UIBase中定义各控件以及组件字段/属性的命名风格是：控件类型/_uiComponent+名字
如果字段/属性是private修饰，举例:_buttonStartGame _uiComponentSkillShow