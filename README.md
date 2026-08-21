# FatalityVisual

一个纯视觉效果的 Windows WPF 小程序：双击后显示全屏深色毛玻璃窗口，先显示 1.5 秒 `Now Loading...`，再播放居中的红色字标动画，并在约 5.5 秒后自动退出。

程序仅包含窗口绘制、WPF 时间轴动画和自动关闭逻辑；不包含联网、文件写入、进程操作、注入、驱动、反作弊绕过或其他系统修改功能。

## 直接运行

Windows 10/11 x64 用户可下载并双击：

```text
release\windows-x64\FatalityVisual.exe
```

这是无需安装 .NET 运行时的独立单文件版本。由于没有商业代码签名证书，Windows 会将发布者显示为未知。

## 动画顺序

1. Windows 原生 Acrylic 毛玻璃背景立即启用。
2. 居中的 `Now Loading...` 显示 1.5 秒。
3. `F` 从下方上升并渐显。
4. `F` 向左移动。
5. `ATALITY` 在裁剪区域内从下向上揭示并渐显。
6. 完整字标短暂停留，窗口淡出并自动关闭。

动画期间按 `Esc` 也可立即退出。

## 环境

- Windows 10/11 x64
- 源码构建需要 .NET 10 SDK
- 仓库提供的单文件发布版无需另装 .NET 运行时

## 本地构建

在本目录打开 PowerShell：

```powershell
dotnet build .\FatalityVisual.csproj -c Release
```

普通构建结果位于：

```text
bin\Release\net10.0-windows\FatalityVisual.exe
```

## 发布为独立单文件 EXE

```powershell
dotnet publish .\FatalityVisual.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o .\publish
```

发布结果：

```text
publish\FatalityVisual.exe
```

## 调整外观和节奏

主要参数都在 `MainWindow.xaml`：

- 全屏模式：窗口的 `WindowState="Maximized"`
- 毛玻璃深浅：`MainWindow.xaml.cs` 中 `GradientColor` 的 Alpha 值
- 颜色：`Foreground="#E20A17"`
- 字体：`FontFamily="Bahnschrift"`
- 字体大小：`FontSize="118"`
- 动画时间：各个 `KeyTime`
- 整体宽度：字标 Grid 的 `Width="520"`
- F 最初的横向位置：`FTranslate` 的 `X="228"`

应用没有要求管理员权限，清单中明确使用 `asInvoker`。
