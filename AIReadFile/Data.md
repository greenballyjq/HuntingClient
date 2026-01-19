##var	ID	ZoneRadius	DamageAmount	DamageInterval	FireDistance	EffectPrefabPath	Comment
##type	int	float	float	float	float	string	string
##group	c	c	c	c	c	c	c
##	参数ID	范围半径	伤害值	伤害间隔（秒）	开火距离	特效预制体路径	备注
	1	3	20	1	4	Arts/Effects/Prefabs/FX_DG_A	示例


##var	ID	MinLockDistance	MaxLockDistance	EffectPrefabPath	Comment
##type	int	float	float	string	string
##group	c	c	c	c	c
##	参数ID	最小锁定距离	最大锁定距离	特效预制体路径	备注
	1	1	20	Arts/Effects/Prefabs/FX_DG_B	示例


##var	ID	BulletType	Name	Duration	BaseDamage	FireRate	MoveSpeed	EffectParamInt	EffectParamFloat	PrefabResourcePath	IconResourcePath	EffectPrefabPath	Comment
##type	int	HuntingConfig.Enum.EBulletType	string	float	float	int	float	int	float	string	string	string	string
##group	c	c	c	c	c	c	c	c	c	c	c	c	c
##	子弹ID	子弹类型	子弹显示名称	持续时间	基础伤害	射速（发/秒）	飞行速度	整形效果参数	浮点型效果参数	预制体资源路径	图标资源路径	特效资源路径	备注
	1	Normal	普通子弹	-1	10	3	15	-1	-1	Arts/Bullets/Prefabs/Bullet_Normal	Arts/Bullets/UI/Bullet_Normal	Arts/Effects/Prefabs/FX_Hit_01	示例
	2	Explosive	爆炸子弹	10	25	1	12	-1	3	Arts/Bullets/Prefabs/Bullet_Explosive	Arts/Bullets/UI/Bullet_Explosive	Arts/Effects/Prefabs/FX_Hit_03	EffectParamFloat:伤害范围
	3	HighDamage	高伤子弹	10	50	1	15	-1	-1	Arts/Bullets/Prefabs/Bullet_HighDamage	Arts/Bullets/UI/Bullet_HighDamage	Arts/Effects/Prefabs/FX_Hit_02	示例
	4	HighSpeed	高速子弹	10	8	8	20	-1	-1	Arts/Bullets/Prefabs/Bullet_HighSpeed	Arts/Bullets/UI/Bullet_HighSpeed	Arts/Effects/Prefabs/FX_Hit_01	示例


##var	ID	RequiredPerBar	MaxBar	RewardSteps	Comment
##type	int	float	int	(map#sep=:|),int,HuntingConfig.Bean.MeatProgressReward	string
##group	c	c	c	c	c
##	肉条ID	单条所需值	最大条数	奖励阶梯	备注
	1	100	5	0:0,0|1:3,2|2:6,4|3:9,6|4:12,8|5:15,10	示例

##var	full_name	parent	valueType	sep	alias	comment	group	tags	*fields						
##var									name	alias	type	group	comment	tags	variants
##	全名(包含模块和名字)			分割符					字段名	字段别名	类型	分组	注释		字段变体
	HuntingConfig.Bean.SpecieWeight			,					SpecieId		int		物种ID		
									Weight		float		出现权重		
	HuntingConfig.Bean.MeatProgressReward			,					ThreeKPCoin		int		3币		
									Point		int		积分		

##var	ID	ValuePerBar	TotalBar	IncreasePerSecond	Comment
##type	int	float	int	float	string
##group	c	c	c	c	c
##	能量条ID	单条所需值	总条数	每秒增加量	备注
	1	100	5	1	示例
