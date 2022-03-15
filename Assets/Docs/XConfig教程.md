## XConfig教程

### 表头格式

![表头格式](https://github.com/xtom0369/XConfig/blob/main/Assets/Docs/Images/%E8%A1%A8%E5%A4%B4%E6%A0%BC%E5%BC%8F.png)

如图所示，表头由上往下分别为：

- 第一行：**字段名**
- 第二行：**字段注释**
- 第三行：**字段数据类型**
- 第四行：**字段关键字**
- 第五行 ~ 结束：**字段值**

### 关键字

关键字可用于定义列的通用行为，如上图所示，当前的关键字只有**M**和**N**：

- M：主键
- N：不导出的列，一般用于注释

### 表主键

主键用于行的唯一标识，当前仅支持单主键和双主键：

- 单主键：单个列作为唯一标识，不同行的单主键值不允许重复。**注意，当前单主键只开放int和string类型**。
- 双主键：两个列作为唯一标识，不同行的双主键值允许其中一个相同，但不允许两个都相同。

### 基础类型

XConfig存在以下内置的配置类型：

- bool
- byte
- enum：需要提前在C#中添加枚举
- float
- short
- int
- long
- ushort
- uint
- ulong
- Vector2
- Vector3
- Vector4
- Color
- DateTime
- List：列表类型必须特殊，**除了列表类型均可作为列表类型的泛型参数**，比如List\<int>，List\<string>，和List<base_ref_type>（base_ref_type为表文件名）等

### 表关联

XConfig支持不同表之间进行关联，**只需要将类型配置为配置表文件名即可**，值配置为关联表的主键值。

![表关联](https://github.com/xtom0369/XConfig/blob/main/Assets/Docs/Images/%E8%A1%A8%E5%85%B3%E8%81%94.png)

**注意，当前的表关联仅支持单主键的表关联**

### 表继承

#### 痛点

表继承为了解决配置表之间既有共同字段又有各自的特殊字段的情况，即表的“**抽象**”。

比如道具，每个道具都会有名字/图标/价格等字段，则可将这些字段定义于父表中；但宝箱道具有宝箱掉落的特殊字段，可将特殊字段定义于子表中。

#### 添加表继承的步骤

1. 添加表继承只需要在**XConfig\Assets\Example\Resources\InheritSettings**中配置父子关系即可。

![表继承](https://github.com/xtom0369/XConfig/blob/main/Assets/Docs/Images/%E8%A1%A8%E7%BB%A7%E6%89%BF.png)

如图，parent为父表，child1和child2为子表。

**PS：只支持一级继承，即不支持C继承B，B继承A的情况。**

### 热刷新

XConfig支持运行时修改数据，方便策划测试数值或配置，只需要在运行时执行Unity菜单【**XConfig/HotReload**】即可。
