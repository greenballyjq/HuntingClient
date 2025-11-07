| ##var   | ID     | RoleType      | Name     | RoleDescription | SkillID      |
| ------- | ------ | ------------- | -------- | --------------- | ------------ |
| ##type  | int    | ERoleType     | string   | string          | int          |
| ##group | c      | c             | c        | c               | c            |
| ##      | 角色ID | 角色类型      | 角色名称 | 角色描述        | 关联丰收技ID |
|         | 1      | Bule          | 小蓝人   | 这位是小蓝人    | 1            |
|         | 2      | Red           | 小红人   | 这位是小红人    | 2            |
|         | 3      | ZiWei         | 紫薇     | 这位是紫薇      | 3            |
|         | 4      | DaMeiLi       | 大美丽   | 这位是大美丽    | 4            |
|         | 5      | JinZhuangYuan | 金状元   | 这位是金状元    | 5            |
|         | 6      | YaKeDong      | 亚克东   | 这位是亚克东    | 6            |

| ##var   | ID     | MapType   | Name           | Description          | SpawnStrategyId | SpeciesByVolume                                              | Comment                   |
| ------- | ------ | --------- | -------------- | -------------------- | --------------- | ------------------------------------------------------------ | ------------------------- |
| ##type  | int    | EMapType  | string         | string               | int             | (map#sep=:\|),EVolumeType,(array#sep=;),HuntingConfig.SpecieWeight | string                    |
| ##group | c      | c         | c              | c                    | c               | c                                                            | c                         |
| ##      | 地图ID | 地图类型  | 地图名称       | 地图描述             | 体型策略ID      | 体型-物种权重映射（每个元素为SpecieWeight的Bean，Bean有SpecieId与Weight，元素之间用";"分隔，键之间用"\|"分隔） | 备注                      |
|         | 1      | Forest    | 皇家猎场森林   | 茂密森林，神秘动物   | 1               | Small:101,0.6;102,0.4\|Medium:201,0.34;202,0.33;203,0.33\|Large:301,1\|Special:401,0.8;402,0.2 | 紫薇主场地图 示例         |
|         | 2      | Beach     | 海滨沙滩度假村 | 阳光沙滩，海洋生物   | 1               | Small:101,0.6;102,0.4\|Medium:201,0.34;202,0.33;203,0.33\|Large:301,1\|Special:401,0.8;402,0.2 | 大美丽主场地图 示例       |
|         | 3      | Garden    | 金状元私家花园 | 精致花园，稀有物种   | 1               | Small:101,0.6;102,0.4\|Medium:201,0.34;202,0.33;203,0.33\|Large:301,1\|Special:401,0.8;402,0.2 | 金状元主场地图 示例       |
|         | 4      | Grassland | 新疆大草原     | 广袤的草原，适合狩猎 | 1               | Small:101,0.6;102,0.4\|Medium:201,0.34;202,0.33;203,0.33\|Large:301,1\|Special:401,0.8;402,0.2 | 亚克东主场地图 示例       |
|         | 5      | Mountain  | 远古雪山       | 寒冷雪山，珍奇动物   | 1               | Small:101,0.6;102,0.4\|Medium:201,0.34;202,0.33;203,0.33\|Large:301,1\|Special:401,0.8;402,0.2 | 隐藏地图，全角色主场 示例 |

| ##var   | ID         | VolumeRatio                                      | StayTime                                 | Comment |
| ------- | ---------- | ------------------------------------------------ | ---------------------------------------- | ------- |
| ##type  | int        | (map#sep=:\|),EVolumeType,float                  | (map#sep=:\|),EVolumeType,int            | string  |
| ##group | c          | c                                                | c                                        | c       |
| ##      | 体型策略ID | 体型比例(小/中/大/特殊)                          | 体型驻场时间(小/中/大/特殊)              | 备注    |
|         | 1          | Small:0.35\|Medium:0.45\|Large:0.2\|Special:0.05 | Small:8\|Medium:12\|Large:15\|Special:10 | 示例    |
|         | 2          | Small:0.25\|Medium:0.35\|Large:0.3\|Special:0.1  | Small:6\|Medium:15\|Large:20\|Special:15 | 示例    |

| ##var   | ID     | Name           | VolumeType  | HP    | MoveSpeed | HitRange | DropType  | DropAmount | DropEnergy     | Comment |
| ------- | ------ | -------------- | ----------- | ----- | --------- | -------- | --------- | ---------- | -------------- | ------- |
| ##type  | int    | string         | EVolumeType | float | float     | float    | EDropType | int        | int            | string  |
| ##group | c      | c              | c           | c     | c         | c        | c         | c          | c              | c       |
| ##      | 物种ID | 物种名称       | 体型        | 血量  | 移动速度  | 受击范围 | 掉落类型  | 掉落数量   | 掉落丰收能量值 | 备注    |
|         | 101    | SmallAnimal1   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 102    | SmallAnimal2   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 103    | SmallAnimal3   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 104    | SmallAnimal4   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 105    | SmallAnimal5   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 106    | SmallAnimal6   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 107    | SmallAnimal7   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 108    | SmallAnimal8   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 109    | SmallAnimal9   | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 110    | SmallAnimal10  | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 111    | SmallAnimal11  | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 112    | SmallAnimal12  | Small       | 100   | 6         | 1        | Meat      | 5          | 10             | 示例    |
|         | 201    | MediumAnimal1  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 202    | MediumAnimal2  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 203    | MediumAnimal3  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 204    | MediumAnimal4  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 205    | MediumAnimal5  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 206    | MediumAnimal6  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 207    | MediumAnimal7  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 208    | MediumAnimal8  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 209    | MediumAnimal9  | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 210    | MediumAnimal10 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 211    | MediumAnimal11 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 212    | MediumAnimal12 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 213    | MediumAnimal13 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 214    | MediumAnimal14 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 215    | MediumAnimal15 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 216    | MediumAnimal16 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 217    | MediumAnimal17 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 218    | MediumAnimal18 | Medium      | 100   | 6         | 2        | Meat      | 15         | 10             | 示例    |
|         | 301    | LargeAnimalC1  | Large       | 100   | 6         | 3        | Meat      | 30         | 10             | 示例    |
|         | 302    | LargeAnimalC2  | Large       | 100   | 6         | 3        | Meat      | 30         | 10             | 示例    |
|         | 303    | LargeAnimalC3  | Large       | 100   | 6         | 3        | Meat      | 30         | 10             | 示例    |
|         | 304    | LargeAnimalC4  | Large       | 100   | 6         | 3        | Meat      | 30         | 10             | 示例    |
|         | 305    | LargeAnimalC5  | Large       | 100   | 6         | 3        | Meat      | 30         | 10             | 示例    |
|         | 306    | LargeAnimalC6  | Large       | 100   | 6         | 3        | Meat      | 30         | 10             | 示例    |
|         | 401    | 弹药怪         | Special     | 100   | 6         | 4        | Bullet    | 25         | 10             | 示例    |
|         | 402    | 金币怪         | Special     | 100   | 6         | 2        | Coin      | 15         | 10             | 示例    |

| ##var   | ID     | BulletType  | Name         | Duration | BaseDamage | FireRate      | DamageRangeType        | DamageRange | MoveSpeed | Comment |
| ------- | ------ | ----------- | ------------ | -------- | ---------- | ------------- | ---------------------- | ----------- | --------- | ------- |
| ##type  | int    | EBulletType | string       | float    | float      | int           | EBulletDamageRangeType | float       | float     | string  |
| ##group | c      | c           | c            | c        | c          | c             | c                      | c           | c         | c       |
| ##      | 子弹ID | 子弹类型    | 子弹显示名称 | 持续时间 | 基础伤害   | 射速（发/秒） | 伤害范围类型           | 伤害范围    | 飞行速度  | 备注    |
|         | 1      | Normal      | 普通子弹     | -1       | 10         | 3             | Single                 | 0           | 40        | 示例    |
|         | 2      | Explosive   | 爆炸子弹     | 10       | 25         | 1             | AOE                    | 2.5         | 20        | 示例    |
|         | 3      | HighDamage  | 高伤子弹     | 10       | 50         | 1             | Single                 | 0           | 40        | 示例    |
|         | 4      | HighSpeed   | 高速子弹     | 10       | 8          | 8             | Single                 | 0           | 80        | 示例    |

| ##var   | ID       | RequiredPerBar | MaxBar   | RewardSteps                                        | Comment |
| ------- | -------- | -------------- | -------- | -------------------------------------------------- | ------- |
| ##type  | int      | float          | int      | (map#sep=:\|),int,HuntingConfig.MeatProgressReward | string  |
| ##group | c        | c              | c        | c                                                  | c       |
| ##      | 血条包ID | 单条所需值     | 最大条数 | 奖励阶梯                                           | 备注    |
|         | 1        | 100            | 5        | 1:3,2\|2:6,4\|3:9,6\|4:12,8\|5:15,10               | 示例    |

| ##var   | ID           | RequiredPerBar | MaxBar   | IncreasePerSecond | Comment |
| ------- | ------------ | -------------- | -------- | ----------------- | ------- |
| ##type  | int          | float          | int      | float             | string  |
| ##group | c            | c              | c        | c                 | c       |
| ##      | 丰收能量条ID | 单条所需值     | 最大条数 | 每秒增加量        | 备注    |
|         | 1            | 100            | 5        | 0.5               | 示例    |

| ##var | full_name                        | parent | valueType | sep    | alias | comment | group | tags | *fields  |          |       |       |            |      |          |
| ----- | -------------------------------- | ------ | --------- | ------ | ----- | ------- | ----- | ---- | -------- | -------- | ----- | ----- | ---------- | ---- | -------- |
| ##var |                                  |        |           |        |       |         |       |      | name     | alias    | type  | group | comment    | tags | variants |
| ##    | 全名(包含模块和名字)             |        |           | 分割符 |       |         |       |      | 字段名   | 字段别名 | 类型  | 分组  | 注释       |      | 字段变体 |
|       | HuntingConfig.SpecieWeight       |        |           | ,      |       |         |       |      | SpecieId |          | int   |       | 物种ID     |      |          |
|       |                                  |        |           |        |       |         |       |      | Weight   |          | float |       | 出现权重   |      |          |
|       | HuntingConfig.MeatProgressReward |        |           | ,      |       |         |       |      | Coin     |          | int   |       | 3币奖励    |      |          |
|       |                                  |        |           |        |       |         |       |      | Mastery  |          | int   |       | 熟练度奖励 |      |          |

| ##var | full_name                      | value_type     | read_schema_from_file                                  | input                   | index                                                        | mode                              | group                                                        | comment          | tags | output                          |
| ----- | ------------------------------ | -------------- | ------------------------------------------------------ | ----------------------- | ------------------------------------------------------------ | --------------------------------- | ------------------------------------------------------------ | ---------------- | ---- | ------------------------------- |
| ##    | 全名(包含模块和名字)           | 记录类名       | 从excel读取定义                                        | 文件列表                | 表id字段                                                     | 模式                              | 分组                                                         | 注释             |      | 输出文件名                      |
| ##    |                                |                | false时取已有定义，true为从excel标题头和属性栏读取定义 | 可以多个，以逗号','分隔 | 为空的话自动取value_type中第一个字段,多主键联合索引为key1+key2,多主键独立索引为"key1,key2" | 取值one\|map\|list，为空自动为map | 取值c\|s\|e，可以有多个，以逗号','分隔。空则表示属于所有分组 |                  |      | 默认为 <module>_<name>.<suffix> |
|       | HuntingConfig.TbBullet         | Bullet         | TRUE                                                   | Bullet.xlsx             |                                                              |                                   |                                                              | 子弹数值表       |      |                                 |
|       | HuntingConfig.TbSpecie         | Specie         | TRUE                                                   | Specie.xlsx             |                                                              |                                   |                                                              | 物种数值表       |      |                                 |
|       | HuntingConfig.TbSpawn          | Spawn          | TRUE                                                   | Spawn.xlsx              |                                                              |                                   |                                                              | 派发数值表       |      |                                 |
|       | HuntingConfig.TbMap            | Map            | TRUE                                                   | Map.xlsx                |                                                              |                                   |                                                              | 地图数值表       |      |                                 |
|       | HuntingConfig.TbRole           | Role           | TRUE                                                   | Role.xlsx               |                                                              |                                   |                                                              | 角色数值表       |      |                                 |
|       | HuntingConfig.TbMeatProgress   | MeatProgress   | TRUE                                                   | MeatProgress.xlsx       |                                                              |                                   |                                                              | 肉度条数值表     |      |                                 |
|       | HuntingConfig.TbEnergyProgress | EnergyProgress | TRUE                                                   | EnergyProgress.xlsx     |                                                              |                                   |                                                              | 丰收能量条数值表 |      |                                 |
|       | HuntingConfig.TbQuest          | Quest          | TRUE                                                   | Quest.xlsx              |                                                              |                                   |                                                              | 任务数值表       |      |                                 |

| 全名(包含模块和名字)   | 是否为位标记枚举（即每个枚举项为位标记数据，例如System.IO.FileMode,填数据时可以为READ\|WRITE这样) | 枚举项是否唯一 | 分组 | 类型             |      | 枚举名          | 别名         | 值   |
| ---------------------- | ------------------------------------------------------------ | -------------- | ---- | ---------------- | ---- | --------------- | ------------ | ---- |
| ERoleType              | FALSE                                                        | TRUE           | c    | 角色类型         |      | Bule            | 小蓝人       | 0    |
|                        | Red                                                          | 小红人         | 1    |                  |      |                 |              |      |
|                        | ZiWei                                                        | 紫薇           | 2    |                  |      |                 |              |      |
|                        | DaMeiLi                                                      | 大美丽         | 3    |                  |      |                 |              |      |
|                        | JinZhuangYuan                                                | 金状元         | 4    |                  |      |                 |              |      |
|                        | YaKeDong                                                     | 亚克东         | 5    |                  |      |                 |              |      |
| EMapType               | FALSE                                                        | TRUE           | c    | 地图类型         |      | Forest          | 皇家猎场森林 | 0    |
|                        | Beach                                                        | 海滨沙滩度假村 | 1    |                  |      |                 |              |      |
|                        | Garden                                                       | 金状元私家花园 | 2    |                  |      |                 |              |      |
|                        | Grassland                                                    | 新疆大草原     | 3    |                  |      |                 |              |      |
|                        | Mountain                                                     | 远古雪山       | 4    |                  |      |                 |              |      |
| EVolumeType            | FALSE                                                        | TRUE           | c    | 物种体型类型     |      | Small           | 小体型       | 0    |
|                        | Medium                                                       | 中体型         | 1    |                  |      |                 |              |      |
|                        | Large                                                        | 大体型         | 2    |                  |      |                 |              |      |
|                        | Special                                                      | 特殊体型       | 3    |                  |      |                 |              |      |
| EDropType              | FALSE                                                        | TRUE           | c    | 物种掉落物类型   |      | Meat            | 回血肉       | 0    |
|                        | Bullet                                                       | 子弹           | 1    |                  |      |                 |              |      |
|                        | Coin                                                         | 3KP金币        | 2    |                  |      |                 |              |      |
| EBulletType            | FALSE                                                        | TRUE           | c    | 子弹类型         |      | Normal          | 普通子弹     | 0    |
|                        | Explosive                                                    | 爆炸子弹       | 1    |                  |      |                 |              |      |
|                        | HighDamage                                                   | 高伤子弹       | 2    |                  |      |                 |              |      |
|                        | HighSpeed                                                    | 高速子弹       | 3    |                  |      |                 |              |      |
| EBulletDamageRangeType | FALSE                                                        | TRUE           | c    | 子弹伤害范围类型 |      | Single          | 单体伤害     | 0    |
|                        | AOE                                                          | 范围伤害       | 1    |                  |      |                 |              |      |
| EQuestType             | FALSE                                                        | TRUE           | c    | 任务类型         |      | KillLargeAnimal | 击杀大型动物 | 0    |
|                        | CollectMeat                                                  | 收集回血肉     | 1    |                  |      |                 |              |      |
|                        | UsePaidItem                                                  | 使用付费道具   | 2    |                  |      |                 |              |      |
|                        | Settle                                                       | 进行结算       | 3    |                  |      |                 |              |      |