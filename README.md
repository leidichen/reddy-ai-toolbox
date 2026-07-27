# Reddy AI Toolbox

> 用 AI 辅助开发和改造的个人小工具集合。

这个仓库收集我在真实工作流里用到的小工具、自动化脚本、桌面应用和 Obsidian 插件改装。它们大多不是大型软件，而是面向具体问题的轻量解决方案：能用、顺手、可复用、方便继续迭代。

## 工具列表

| 工具 | 类型 | 状态 | 说明 |
| --- | --- | --- | --- |
| DeepSeek DeskBand | Windows 任务栏工具 | 已整理 | 在 Windows 任务栏实时显示 DeepSeek API 可用余额。 |

## 快速入口

- [DeepSeek DeskBand](tools/deepseek-deskband/README.md)
- [构建记录](docs/build-notes.md)
- [更新日志](docs/changelog.md)
- [许可与来源说明](docs/license-notes.md)

## 项目定位

Reddy AI Toolbox 不是一个追求大而全的软件项目，而是一个个人 AI 工具实验室。

我会把日常使用中遇到的具体问题，整理成独立的小工具或插件改装，并尽量补齐说明、截图、使用方式、已知限制和后续计划。

## 目录结构

```text
reddy-ai-toolbox/
  README.md
  LICENSE
  tools/
    deepseek-deskband/
      README.md
      README.en.md
      DeskBandWidget.csproj
      *.cs
      Controls/
      Interop/
      installer/
  docs/
    build-notes.md
    changelog.md
    license-notes.md
```

## 使用说明

每个工具都有自己的说明文件。进入对应目录后，可以查看：

- 这个工具解决什么问题
- 当前功能和适用平台
- 安装与使用方法
- 截图或演示
- 已知限制
- 后续计划

## 维护原则

- 每个工具尽量保持独立，避免互相绑死。
- README 优先写清楚真实使用场景，而不是只堆技术说明。
- 能截图就截图，能给示例就给示例。
- 改装第三方项目时，必须标注原项目来源和许可证。
- AI 生成代码需要经过人工检查、测试和整理后再发布。

## 许可

本仓库默认使用 MIT License。  
如果某个子工具基于第三方项目改造，请以该工具目录中的说明和原项目许可证为准。



