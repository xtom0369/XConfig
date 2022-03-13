## 关于

XConfig 是一个基于Unity引擎、支持csv => bytes的轻量的配置表系统，提供了以下的特性：

- **自动生成表解析代码**：一键生成配置表解析代码，无需手撸解析代码；
- **基本数据类型**：支持byte，bool，int，long，float，Vector2，Color，List等基础数据类型；
- **双主键**：支持配置两个主键，比如英雄等级表需要英雄id和英雄level两个主键共同决定配置行；
- **表关联**：支持不同配置表之间进行关联；
- **表继承**：支持建立表继承关系，比如道具分多种类型的道具，通用道具字段放父表，不同道具的特殊字段放子表；
- **自定义类型**：支持快速添加自定义数据类型，支持后自动支持List的泛型类型
- **表/行的代码类扩展**：支持在扩展类中对表/行的字段数据进行二次处理或组织
- **热刷新**：支持运行时刷新配置表；（当前只支持对已有字段进行修改，不支持增减行或列，避免影响运行时已引用的数据）

## 快速开始

1. 可以使用命令行把仓库下载到本地：
```sh
git clone https://github.com/xtom0369/XConfig.git
```

2. 直接用Unity打开工程

3. 将配置导出为二进制文件，执行Unity菜单【**XConfig/Generate Binary**】

4. 打开场景Assets/Example/BaseType.unity，运行即可开始

## 文件夹结构

    |-- Example
        |-- Config：配置表，按照例子分文件夹
        |-- Generate：生成内容
            |-- Bin：生成的二进制文件
            |-- Code：生成的代码
        |-- Resources：框架配置
        |-- Script：脚本，按照例子分文件夹

    |-- StreamingAssets：例子配置表导出的assetbundle

    |-- XConfig
        |-- Editor：框架编辑器代码
        |-- Runtime：框架运行时代码

## 更多示例

- [BaseType](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/BaseType.cs)：所有支持的基础类型例子

- [DoubleKey](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/DoubleKey.cs)：双主键例子

- [ParentChildren](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/ParentChildren.cs)：表继承例子

- [CustomConfigType](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/CustomConfigType.cs)：自定义类型例子

- [PartialTable](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/PartialTable.cs)：表/行的代码类扩展例子

- [CustomLoader](https://github.com/xtom0369/XConfig/blob/main/Assets/Example/Script/PartialTable.cs)：自定义加载例子，用于ab等自定义加载

## 社区

- Q群：975919763（入群密码是https://github.com/xtom0369/XConfig）