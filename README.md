# WeChatJs.NET #


微信 JsSDK 签名过程的 .NET 实现，可供普通 WebForm/MVC/Web API 或 WebPage 等类型的项目使用。

----------

微信于 2015 年初升级了其 JavaScript SDK 的机制，开放了更多可用的 JavaScript Api 供 Web App 调用微信的内部功能，如分享和卡券等。具体详细情况请 [移步其官方文档](http://mp.weixin.qq.com/wiki/7/aaa137b55fb2e0456bf8dd9148dd613f.html "微信JS-SDK说明文档")。


根据官方文档描述，在调用 JavaScript 操作接口前，必须使用公众账号的 AppId 等凭据完成签名过程。

此项目是根据该官方文档所述而完成的一个 .NET 上的实现，您可以使用此类型生成一个类库来完成您的签名过程，此实现已按文档要求内置了必要的缓存功能。

目前尚未发布到 nuget，请您自行编译后使用，需要 .NET 4 运行环境；推荐使用 [Visual Studio 社区版](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx)。