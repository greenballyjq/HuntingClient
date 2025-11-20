##var	ID	BulletType	Name	Duration	BaseDamage	FireRate	MoveSpeed	EffectParamInt	EffectParamFloat	PrefabResourcePath	IconResourcePath	Comment
##type	int	HuntingConfig.Enum.EBulletType	string	float	float	int	float	int	float	string	string	string
##group	c	c	c	c	c	c	c	c	c	c	c	c
##	子弹ID	子弹类型	子弹显示名称	持续时间	基础伤害	射速（发/秒）	飞行速度	整形效果参数	浮点型效果参数	预制体资源路径	图标资源路径	备注
	1	Normal	普通子弹	-1	10	3	60	-1	-1	Arts/Prefabs/Bullets/Bullet_Normal	Arts/UI/Gameplay/Bullets/Bullet_Normal	示例
	2	Explosive	爆炸子弹	10	25	1	40	-1	10	Arts/Prefabs/Bullets/Bullet_Explosive	Arts/UI/Gameplay/Bullets/Bullet_Explosive	EffectParamFloat:伤害范围
	3	HighDamage	高伤子弹	10	50	1	60	-1	-1	Arts/Prefabs/Bullets/Bullet_HighDamage	Arts/UI/Gameplay/Bullets/Bullet_HighDamage	示例
	4	HighSpeed	高速子弹	10	8	8	100	-1	-1	Arts/Prefabs/Bullets/Bullet_HighSpeed	Arts/UI/Gameplay/Bullets/Bullet_HighSpeed	示例


##var	ID	RequiredPerBar	MaxBar	IncreasePerSecond	Comment
##type	int	float	int	float	string
##group	c	c	c	c	c
##	丰收能量条ID	单条所需值	最大条数	每秒增加量	备注
	1	100	5	10	示例

##var	ID	LuckyType	Name	Description	EffectParamFloat	EffectParamInt	 IconResourcePath	Comment
##type	int	HuntingConfig.Enum.ELuckyType	string	string	float	int	string	string
##group	c	c	c	c	c	c	c	c
##	增益ID	增益类型	增益名称	增益描述	通用浮点参数	通用整型参数	图标资源路径	备注
	1	None	无增益	未选择幸运仪式	-1	-1	/	默认值
	2	MoreMeat	爆肉加成	狩猎动物有概率额外掉肉	0.1	2	/	"EffectParamFloat：概率
EffectParamInt：额外掉肉倍率"
	3	SpecialBullet	开局特殊子弹	开局获得特殊子弹	-1	1	/	EffectParamInt：特殊子弹数量
	4	StartEnergy	开局丰收能量	开局获得丰收能量条	-1	1	/	EffectParamInt：丰收能量条条数
	5	DamageBoost	伤害提升	所有子弹伤害提升	10	-1	/	EffectParamFloat：伤害提升倍率
	6	HighTierSpawn	高阶派发加成	大型/特殊动物派发比提升	1.3	-1	/	EffectParamFloat：派发提升倍率

##var	ID	MapType	Name	Description	SpawnStrategyId	SpeciesByVolume	MapImageResourcePath	Comment
##type	int	HuntingConfig.Enum.EMapType	string	string	int	(map#sep=:|),HuntingConfig.Enum.EVolumeType,(array#sep=;),HuntingConfig.Bean.SpecieWeight	string	string
##group	c	c	c	c	c	c	c	c
##	地图ID	地图类型	地图名称	地图描述	体型策略ID	体型-物种权重映射	地图图片资源路径	备注
	1	Forest	皇家猎场森林	茂密森林，神秘动物	1	Small:101,0.6;102,0.4|Medium:201,0.34;202,0.33;203,0.33|Large:301,1|Special:401,0.8;402,0.2	Arts/UI/Prepare/Map_Forest	紫薇主场地图 
	2	Beach	海滨沙滩度假村	阳光沙滩，海洋生物	1	Small:101,0.6;102,0.4|Medium:201,0.34;202,0.33;203,0.33|Large:301,1|Special:401,0.8;402,0.2	Arts/UI/Prepare/Map_Beach	大美丽主场地图 
	3	Garden	金状元私家花园	精致花园，稀有物种	1	Small:101,0.6;102,0.4|Medium:201,0.34;202,0.33;203,0.33|Large:301,1|Special:401,0.8;402,0.2	Arts/UI/Prepare/Map_Garden	金状元主场地图 
	4	Grassland	新疆大草原	广袤的草原，适合狩猎	1	Small:101,0.6;102,0.4|Medium:201,0.34;202,0.33;203,0.33|Large:301,1|Special:401,0.8;402,0.2	Arts/UI/Prepare/Map_Grassland	亚克东主场地图 
	5	Mountain	远古雪山	寒冷雪山，珍奇动物	1	Small:101,0.6;102,0.4|Medium:201,0.34;202,0.33;203,0.33|Large:301,1|Special:401,0.8;402,0.2	Arts/UI/Prepare/Map_Mountain	隐藏地图 

##var	ID	RequiredPerBar	MaxBar	RewardSteps	Comment
##type	int	float	int	(map#sep=:|),int,HuntingConfig.Bean.MeatProgressReward	string
##group	c	c	c	c	c
##	血条包ID	单条所需值	最大条数	奖励阶梯	备注
	1	100	5	1:3,2|2:6,4|3:9,6|4:12,8|5:15,10	示例

##var	ID	PropType	Name	Description	Duration	IconResourcePath	ParamTableID	Comment
##type	int	HuntingConfig.Enum.EPropType	string	string	float	string	int	string
##group	c	c	c	c	c	c	c	c
##	道具ID	道具类型	道具名称	道具描述	持续时间（秒）	图标资源路径	参数子表ID	备注
	1	Bombardment	炮火轰炸	在范围内持续造成伤害	6	/	1	示例
	2	AimAssist	指哪打哪	自动锁定并跟随目标	10	/	1	示例
	3	Trap	智能诱捕陷阱	生成陷阱吸引并击杀动物	10	/	1	示例

##var	ID	MaxLockDistance	Comment
##type	int	float	string
##group	c	c	c
##	参数ID	最大锁定距离	备注
	1	20	示例

##var	ID	ZoneRadius	DamageAmount	DamageInterval	Comment
##type	int	float	float	float	string
##group	c	c	c	c	c
##	参数ID	圆形范围半径	每次伤害数值	伤害间隔（秒）	备注
	1	8	80	1	示例

##var	ID	TrapCount	AttractRadius	SpawnRadius	TrapPrefabResourcePath	Comment
##type	int	int	float	float	string	string
##group	c	c	c	c	c	c
##	参数ID	陷阱数量	吸引半径	生成半径	陷阱预制体资源路径	备注
	1	6	12	15	Arts/Prefabs/Props/Traps/Trap_Bomb	示例

##var	ID	QuestType	TargetRange	Duration	RewardRange	Comment
##type	int	HuntingConfig.Enum.EQuestType	(array#sep=,),int	float	(array#sep=,),int	string
##group	c	c	c	c	c	c
##	任务ID	 任务类型	任务目标数值范围	任务持续时间	任务奖励数值范围	备注
	1	KillLargeAnimal	1,2	20	5,10	示例
	2	CollectMeat	10,20	20	8,15	示例
	3	UsePaidItem	1,2	20	3,8	示例
	4	Settle	1,1	20	10,20	示例

##var	ID	RoleType	RoleProfile							LinkedSkillId	LinkedMapId	RoleImageResourcePath	Comment
##type	int	HuntingConfig.Enum.ERoleType	HuntingConfig.Bean.RoleProfile							int	int	string	string
##group	c	c	c							c	c	c	c
##	角色ID	角色类型	角色档案							关联技能ID	关联地图ID	角色图片资源路径	注释
	1	Bule	小蓝人	小蓝星	170	男	小蓝人的特征描述	小蓝人的个性描述	小蓝人的背景故事	1	-1	Arts/UI/Prepare/Role_Blue	示例
	2	Red	小红人	小红星	170	女	小红人的特征描述	小红人的个性描述	小红人的背景故事	1	-1	Arts/UI/Prepare/Role_Red	示例
	3	ZiWei	紫薇	紫薇家乡	170	女	紫薇的特征描述	紫薇的个性描述	紫薇的背景故事	2	1	Arts/UI/Prepare/Role_ZiWei	示例
	4	DaMeiLi	大美丽	大美丽家乡	170	女	大美丽的特征描述	大美丽的个性描述	大美丽的背景故事	3	2	Arts/UI/Prepare/Role_DaMeiLi	示例
	5	JinZhuangYuan	金状元	金状元家乡	170	男	金状元的特征描述	金状元的个性描述	金状元的背景故事	4	3	Arts/UI/Prepare/Role_JinZhuangYuan	示例
	6	YaKeDong	亚克东	亚克东家乡	170	男	亚克东的特征描述	亚克东的个性描述	亚克东的背景故事	5	4	Arts/UI/Prepare/Role_YaKeDong	示例

##var	ID	SkillType	Name	Description	Duration	IconResourcePath	ParamTableID	Comment
##type	int	HuntingConfig.Enum.ESkillType	string	string	float	string	int	string
##group	c	c	c	c	c	c	c	c
##	技能ID	技能类型	技能名称	技能描述	持续时间	图标资源路径	参数子表ID	注释
	1	_3KPSkill	火力全开	色块人的技能	10	/	1	示例
	2	ZiWeiSkill	帮帮我嘛	紫薇的技能	10	/	1	示例
	3	DaMeiLiSkill	爱心牢笼	大美丽的技能	10	/	1	示例
	4	JinZhuangYuanSkill	火箭投喂	金状元的技能	10	/	1	示例
	5	YaKeDongSkill	视钱如命	亚克东的技能	10	/	1	示例

##var	ID	DamageMultiplier	FireRateMultiplier	Comment
##type	int	float	float	string
##group	c	c	c	c
##	参数ID	伤害倍率	射速倍率	备注
	1	1.5	2	示例

##var	ID	ZoneRadius	DamageAmount	DamageInterval	Comment
##type	int	float	float	float	string
##group	c	c	c	c	c
##	参数ID	牢笼半径	伤害数值	伤害间隔（秒）	备注
	1	6	20	1	示例

##var	ID	MeatPercent	IncreasePerSecond	Comment
##type	int	float	float	string
##group	c	c	c	c
##	参数ID	肉量百分比	每秒增加量	备注
	1	0.2	5	示例

##var	ID	SpawnAnimalID	SpawnCount	Comment
##type	int	int	(array#sep=,),int	string
##group	c	c	c	c
##	参数ID	派发动物ID	生成数量	备注
	1	402	3,6	派发随机数量ID为402的物种

##var	ID	GunCountPerSide	GunOffsetX	FireInterval	Comment
##type	int	float	float	float	string
##group	c	c	c	c	c
##	参数ID	每侧猎枪数量	水平偏移	射击间隔（秒）	备注
	1	2	2.5	0.5	示例

##var	ID	VolumeRatio	StayTime	Comment
##type	int	(map#sep=:|),HuntingConfig.Enum.EVolumeType,float	(map#sep=:|),HuntingConfig.Enum.EVolumeType,int	string
##group	c	c	c	c
##	体型策略ID	体型比例(小/中/大/特殊)	体型驻场时间(小/中/大/特殊)	备注
	1	Small:0.25|Medium:0.35|Large:0.3|Special:0.1	Small:8|Medium:12|Large:15|Special:10	示例
	2	Small:0.25|Medium:0.35|Large:0.3|Special:0.1	Small:6|Medium:15|Large:20|Special:15	示例

##var	ID	Name	VolumeType	HP	MoveSpeed	HitRange	DropRewards	PrefabResourcePath	Comment
##type	int	string	HuntingConfig.Enum.EVolumeType	float	float	float	(map#sep=:|),HuntingConfig.Enum.EDropType,int	string	string
##group	c	c	c	c	c	c	c	c	c
##	物种ID	物种名称	体型	血量	移动速度	受击范围	掉落奖励	预制体资源路径	备注
	101	SmallAnimal1	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_101	示例
	102	SmallAnimal2	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_102	示例
	103	SmallAnimal3	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_103	示例
	104	SmallAnimal4	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_104	示例
	105	SmallAnimal5	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_105	示例
	106	SmallAnimal6	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_106	示例
	107	SmallAnimal7	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_107	示例
	108	SmallAnimal8	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_108	示例
	109	SmallAnimal9	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_109	示例
	110	SmallAnimal10	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_110	示例
	111	SmallAnimal11	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_111	示例
	112	SmallAnimal12	Small	30	6	1	Meat:5|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Small_112	示例
	201	MediumAnimal1	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_201	示例
	202	MediumAnimal2	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_202	示例
	203	MediumAnimal3	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_203	示例
	204	MediumAnimal4	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_204	示例
	205	MediumAnimal5	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_205	示例
	206	MediumAnimal6	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_206	示例
	207	MediumAnimal7	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_207	示例
	208	MediumAnimal8	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_208	示例
	209	MediumAnimal9	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_209	示例
	210	MediumAnimal10	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_210	示例
	211	MediumAnimal11	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_211	示例
	212	MediumAnimal12	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_212	示例
	213	MediumAnimal13	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_213	示例
	214	MediumAnimal14	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_214	示例
	215	MediumAnimal15	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_215	示例
	216	MediumAnimal16	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_216	示例
	217	MediumAnimal17	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_217	示例
	218	MediumAnimal18	Medium	30	6	2	Meat:15|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Medium_218	示例
	301	LargeAnimalC1	Large	30	6	3	Meat:30|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Large_301	示例
	302	LargeAnimalC2	Large	30	6	3	Meat:30|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Large_302	示例
	303	LargeAnimalC3	Large	30	6	3	Meat:30|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Large_303	示例
	304	LargeAnimalC4	Large	30	6	3	Meat:30|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Large_304	示例
	305	LargeAnimalC5	Large	30	6	3	Meat:30|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Large_305	示例
	306	LargeAnimalC6	Large	30	6	3	Meat:30|Bullet:0|Coin:0|Energy:20	Arts/Prefabs/Animals/Animal_Large_306	示例
	401	弹药怪	Special	30	6	4	Meat:20|Bullet:1|Coin:0|Energy:50	Arts/Prefabs/Animals/Animal_Special_401	示例
	402	金币怪	Special	30	6	2	Meat:20|Bullet:0|Coin:10|Energy:50	Arts/Prefabs/Animals/Animal_Special_402	示例

##var	full_name	parent	valueType	sep	alias	comment	group	tags	*fields						
##var									name	alias	type	group	comment	tags	variants
##	全名(包含模块和名字)			分割符					字段名	字段别名	类型	分组	注释		字段变体
	HuntingConfig.Bean.SpecieWeight			,					SpecieId		int		物种ID		
									Weight		float		出现权重		
	HuntingConfig.Bean.MeatProgressReward			,					Coin		int		3币奖励		
									Mastery		int		熟练度奖励		
	HuntingConfig.Bean.RoleProfile								Name		string		角色名称		
									Birthplace		string		出生地		
									Height		float		身高		
									Gender		string		性别		
									Traits		string		特征描述		
									Personality		string		个性描述		
									BackgroundStory		string		背景故事		

##var	full_name	flags	unique	group	comment	tags	*items				
##var							name	alias	value	comment	tags
##	全名(包含模块和名字)	是否为位标记枚举（即每个枚举项为位标记数据，例如System.IO.FileMode,填数据时可以为READ|WRITE这样)	枚举项是否唯一	分组	类型		枚举名	别名	值	注释	
	HuntingConfig.Enum.ERoleType	FALSE	TRUE	c	角色类型		Bule	小蓝人	0		
							Red	小红人	1		
							ZiWei	紫薇	2		
							DaMeiLi	大美丽	3		
							JinZhuangYuan	金状元	4		
							YaKeDong	亚克东	5		
	HuntingConfig.Enum.EMapType	FALSE	TRUE	c	地图类型		Forest	皇家猎场森林	0		
							Beach	海滨沙滩度假村	1		
							Garden	金状元私家花园	2		
							Grassland	新疆大草原	3		
							Mountain	远古雪山	4		
	HuntingConfig.Enum.EVolumeType	FALSE	TRUE	c	物种体型类型		Small	小体型	0		
							Medium	中体型	1		
							Large	大体型	2		
							Special	特殊体型	3		
	HuntingConfig.Enum.EDropType	FALSE	TRUE	c	物种掉落物类型		Meat	回血肉	0		
							Bullet	子弹	1		
							Coin	3KP金币	2		
							Energy	丰收能量	3		
	HuntingConfig.Enum.EBulletType	FALSE	TRUE	c	子弹类型		Normal	普通子弹	0		
							Explosive	爆炸子弹	1		
							HighDamage	高伤子弹	2		
							HighSpeed	高速子弹	3		
	HuntingConfig.Enum.EBulletDamageRangeType	FALSE	TRUE	c	子弹伤害范围类型		Single	单体伤害	0		
							AOE	范围伤害	1		
	HuntingConfig.Enum.ESkillType	FALSE	TRUE	c	技能类型		_3KPSkill	色块人技能	0		
							ZiWeiSkill	紫薇技能	1		
							DaMeiLiSkill	大美丽技能	2		
							JinZhuangYuanSkill	金状元技能	3		
							YaKeDongSkill	亚克东技能	4		
	HuntingConfig.Enum.ELuckyType	FALSE	TRUE	c	幸运仪式类型		None	无增益	0		
							MoreMeat	爆肉加成	1		
							SpecialBullet	开局特殊子弹	2		
							StartEnergy	开局丰收能量	3		
							DamageBoost	伤害提升	4		
							HighTierSpawn	高阶派发加成	5		
	HuntingConfig.Enum.EQuestType	FALSE	TRUE	c	任务类型		KillLargeAnimal	击杀大型动物	0		
							CollectMeat	收集回血肉	1		
							UsePaidItem 	使用付费道具	2		
							Settle	进行结算	3		
	HuntingConfig.Enum.EPropType	FALSE	TRUE	c	道具类型		Bombardment	炮火轰炸	0		
							AimAssist	指哪打哪	1		
							Trap	智能诱捕陷阱	2		
