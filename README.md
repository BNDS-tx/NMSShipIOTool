![C# 13.0](https://img.shields.io/badge/C%23-13.0-239120)
![.NET 9.0](https://img.shields.io/badge/.NET-9.0-512BD4)
![Windows Forms 9](https://img.shields.io/badge/WinForms-9-0078D6)

# NMSShipIOTool 无人深空飞船导入导出工具

**NMSShipIOTool** 是一个基于 **.NET 9** 框架的 **WinForm 应用**，用于管理《无人深空 (No Man's Sky)》游戏存档中的玩家飞船数据。  
本工具支持 **查看、导出、导入** 各类玩家可控飞船及其 **技术、模块与库存** 数据，包括：  

- 普通种子飞船  | 及其技术、模块与库存数据  
- 普通拼装飞船  | 及其技术、模块与库存数据  
- 自定义护卫舰  | 及其技术、模块与库存数据  

并兼容以下飞船文件格式：  

- `.sh0` —— [NMS Save Editor](https://github.com/goatfungus/NMSSaveEditor) 使用的飞船文件格式  
- `.nmsship` —— [ō.Ó 游戏社区 | 无人深空建模导出入工具](https://oogc.cc/plugin.php?id=one_market&action=item&sid=82) 使用的飞船完整包格式
- `.tech` —— 工具首创转门保存飞船 **技术、模块与库存** 信息的文件格式

本项目基于 [libNOM.io](https://github.com/zencq/libNOM.io) 及其依赖项目开发。

## ✨ 功能特性

- 读取并展示存档中所有玩家可控飞船  
- 导出飞船为 `.json`，`.sh0` 或 `.nmsship` 文件  
- 从 `.json`，`.sh0` 或 `.nmsship` 文件导入飞船数据
- 导出飞船技术、模块与库存数据为 `.json`，`.tech` 文件
- 从 `.json`，`.tech` 文件导入飞船技术、模块与库存数据
- 提供完整飞船包与建模文件操作支持  
- 与常见存档修改器工具保持兼容  

## 📦 安装与使用

1. 从 [Releases 页面](../../releases) 下载最新版本。  
2. 解压后运行 `NMSShipIOTool_Release_X64_[依赖外部运行时或独立].exe`。  
3. 在程序中选择你的《无人深空》存档路径。  
4. 使用界面进行飞船查看、导入、导出操作。  

> ⚠️ **注意**：请在修改前备份存档，以防止存档损坏。  

## 🔗 相关工具兼容性

- **NMS Save Editor**: 提供 `.sh0` 飞船文件导入导出支持  
- **NMS Model IO Tool**: 提供 `.nmsship` 飞船完整包兼容性  

## 📖 基于的开源项目

本工具基于以下开源项目开发：  

- [libNOM.io](https://github.com/zencq/libNOM.io) 
  - 及其依赖项目  

特别感谢上述开源项目的作者和社区。  

## 📜 许可证

本项目采用 GNU GPLv3 许可证，详情见 [LICENSE](./LICENSE)。  
在使用或分发本项目时，请遵守相关依赖项目的许可证要求。  
对于商业许可证，请联系作者以获取更多许可。  
For commercial licensing options, please contact the author.

## 💡 鸣谢

- 《无人深空》玩家社区  
- [ō.Ó 游戏社区](https://oogc.cc)  
- [NMS Save Editor 项目](https://github.com/goatfungus/NMSSaveEditor)  
- [libNOM.io](https://github.com/zencq/libNOM.io)  及其依赖项目  
