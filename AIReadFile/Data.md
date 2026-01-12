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
						Hidden	远古雪山	4
HuntingConfig.Enum.EVolumeType	FALSE	TRUE	c	物种体型类型		Small	小体型	0
						Medium	中体型	1
						Large	大体型	2
						Special	特殊体型	3
						Boss	Boss体型	4
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
HuntingConfig.Enum.ELuckyBuffType	FALSE	TRUE	c	幸运仪式增益类型		MoreMeat	更多大肉	0
						StartSpecialBullet	开局特殊子弹	1
						StartEnergy	开局丰收能量	2
						DamageBoost	伤害提升	3
						HighTierSpawn	高阶派发	4
HuntingConfig.Enum.ELuckyBuffStrengthType	FALSE	TRUE	c	幸运仪式增益强度类型		Weak	弱档	0
						Normal	中档	1
						Strong	强档	2
HuntingConfig.Enum.ELuckyGiftType	FALSE	TRUE	c	幸运仪式礼包类型 		Basic	初级礼包	0
						Standard	中级礼包	1
						Advanced	高级礼包	2
						Special	特殊礼包	3
HuntingConfig.Enum.ELuckyGiftCostType	FALSE	TRUE	c	礼包付费方式		ThreeKPCoin	3KP金币	0
						Ad	广告	1
HuntingConfig.Enum.EQuestType	FALSE	TRUE	c	任务类型		KillLargeAnimal	击杀大型动物	0
						CollectMeat	收集回血肉	1
						UsePaidItem 	使用付费道具	2
						Settle	进行结算	3
HuntingConfig.Enum.EPropType	FALSE	TRUE	c	道具类型		Bombardment	炮火轰炸	0
						AimAssist	指哪打哪	1
						Trap	智能诱捕陷阱	2

##var	ID	Name	VolumeType	HP	MoveSpeed	DropRewards	Comment
##type	int	string	HuntingConfig.Enum.EVolumeType	float	float	(map#sep=:|),HuntingConfig.Enum.EDropType,int	string
##group	c	c	c	c	c	c	c
##	物种ID	物种名称	体型	血量	移动速度	掉落奖励	备注
	101	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	102	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	103	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	104	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	105	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	106	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	107	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	108	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	109	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	110	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	111	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	112	兔子	Small	30	2	Meat:1|Bullet:0|Coin:0|Energy:1	/
	201	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	202	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	203	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	204	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	205	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	206	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	207	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	208	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	209	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	210	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	211	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	212	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	213	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	214	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	215	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	216	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	217	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	218	兔子	Medium	60	1.5	Meat:5|Bullet:0|Coin:0|Energy:2	/
	301	兔子	Large	120	1	Meat:10|Bullet:0|Coin:0|Energy:5	/
	302	兔子	Large	120	1	Meat:10|Bullet:0|Coin:0|Energy:5	/
	303	兔子	Large	120	1	Meat:10|Bullet:0|Coin:0|Energy:5	/
	304	兔子	Large	120	1	Meat:10|Bullet:0|Coin:0|Energy:5	/
	305	兔子	Large	120	1	Meat:10|Bullet:0|Coin:0|Energy:5	/
	306	兔子	Large	120	1	Meat:10|Bullet:0|Coin:0|Energy:5	/
	401	弹药怪	Special	60	1	Meat:5|Bullet:1|Coin:0|Energy:5	/
	402	金币怪	Special	120	1	Meat:10|Bullet:0|Coin:10|Energy:5	/
	501	可怕大兔子	Boss	1000	6	Meat:200|Bullet:0|Coin:100|Energy:0	/
