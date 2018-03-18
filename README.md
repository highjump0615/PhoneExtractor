# PhoneExtractor
======

> Windows Application for extracting data from phone, .NET Framework + WPF

## Overview

### 1. 主要功能
- 手机提取    
  - 安卓提取  
自动连接、手动连接、密码破解
  - 苹果提取  
同步获取、密码文件绕过
- 案件管理  
案件列表、案件导入、案件创建、删除 ...
- 物证管理  
物证列表
- 工具管理  
系统工具、安卓工具、苹果相关、附件工具
- 设置  
系统设置、环境检测、在线更新、用户反馈 ...
 
### 2. 技术内容
整体开发模式为MVVM, WPF框架 
#### 2.1 UI Implementation
- 使用MergedDictionaries分开颜色、大小等常用值  
```xml  
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="/Resources\Values.xaml" />
    <ResourceDictionary Source="/Resources\Colors.xaml" />
</ResourceDictionary.MergedDictionaries>
```  
- Window style
  - 窗口边缘拖拽调整大小   
```xml
<WindowChrome x:Key="SubWindowChromeKey" CaptionHeight="0">
    <WindowChrome.ResizeBorderThickness>
        <Thickness>5</Thickness>
    </WindowChrome.ResizeBorderThickness>
</WindowChrome>
```
- 多语言处理  
  - String.zh-CN.xaml
  - String.en-US.xaml  
代码设置语言  
```C#
//
// 加载语言
//
ResourceDictionary langRd = null;
try
{
    langRd = Application.LoadComponent(
            new Uri(@"Resources/Strings/String." + User.LoginUser.USER_LANGUAGE + ".xaml", 
            UriKind.Relative)
        ) as ResourceDictionary;
}
catch
{
}

if (langRd != null)
{
    // 删除已设置的语言
    if (Application.Current.Resources.MergedDictionaries.Count() > 2)
    {
        Application.Current.Resources.MergedDictionaries.RemoveAt(2);
    }

    // 添加新的语言
    Application.Current.Resources.MergedDictionaries.Add(langRd);
}
```

#### 2.2 Function Implementation
- iOS提取  
- 安卓提取: 使用第三方库
  - Adb连接
  - busyBox, service.apk, mydos2unix, AdbWinUsbApi.dll, libintl3.dll, libiconv2.dll, sms.tp, contact.tp, Devinfo.tp, appInfo.tp, calllog.tp, Data.mdb, Rar.exe

#### 2.3 Code tricks  
- ``CommonUtil.ToDerived()`` 基础对象转换成继承对象  
[https://www.codeproject.com/Articles/34105/Convert-Base-Type-to-Derived-Type](https://www.codeproject.com/Articles/34105/Convert-Base-Type-to-Derived-Type)  
- ``CommonUtil.GetParentWindow()`` 获取控件对象所在的窗口对象  
通过 ``VistualTreeHelper(PresentationCore)`` 实现  

#### 2.4 Third-Party Libraries
- [FontAwesome](https://github.com/soarcn/BottomSheet) v4.7.0.9    
- [log4net](https://www.nuget.org/packages/log4net/) v2.0.8  
- [SQLite](https://www.nuget.org/packages/System.Data.SQLite/) v2.0.0-rc2    
- [Manzana](https://github.com/ipfans/Manzana)   
C# Library to connect to your iDevice  
依赖于iTunesMobileDevice.dll
- [System.Runtime.Serialization.Plists](https://github.com/ChadBurggraf/plists-cs)  
Plist serialization and de-serialization for C# and .NET  
- [ZipStorer](https://github.com/jaime-olivares/zipstorer) v3.4.0
- [WPF Animated GIF](https://github.com/XamlAnimatedGif/WpfAnimatedGif) v1.4.15 

## Need to Improve
- 优化界面 & 完善功能