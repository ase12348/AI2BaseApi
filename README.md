# 极简搭 AI 应用开发基座（jcode.framework）

基于 .NET 10 的 AI 应用开发基座，提供从数据访问、权限、缓存、任务调度到文件存储、消息队列、Office 预览的一整套基础能力，用于快速搭建企业级 API 服务与业务应用。

[官方网站](https://www.jcodesoft.com) · [Web 前端](https://gitee.com/program_cat/ai2-base-web) · [移动端](https://gitee.com/program_cat/ai2-base-app)

| 组成 | 仓库 | 技术栈 | 说明 |
| --- | --- | --- | --- |
| Web 前端 | [AI2BaseWeb](https://gitee.com/program_cat/ai2-base-web) | Vue 3 / Vite / TS | PC 端管理界面 |
| 后端服务 | 当前仓库 | .NET 10 / ASP.NET Core | API 宿主、ORM、权限、缓存、任务调度 |
| 移动端 | [AI2BaseApp](https://gitee.com/program_cat/ai2-base-app) | uni-app / Vue 3 | 同一套表单渲染到小程序 / App / 鸿蒙 |

> 官网与产品介绍：**<https://www.jcodesoft.com>**

---

## 目录结构

```
jcode.core/
├── base/                  # 01 基础模块
│   ├── jcode.autofac      # 依赖注入扩展
│   ├── jcode.cache        # 缓存（MemoryCache / Redis）
│   ├── jcode.exchange     # Exchange 邮件与日程
│   ├── jcode.mq           # 消息队列（RabbitMQ）
│   ├── jcode.office       # Office 文档转换与预览
│   ├── jcode.operat       # 操作者上下文、日志与权限
│   ├── jcode.orm          # ORM、仓储与多租户
│   ├── jcode.oss          # 对象存储（本地/阿里云/腾讯云/华为云/MinIO/七牛）
│   ├── jcode.qiniu        # 七牛 SDK
│   ├── jcode.timer        # 定时器
│   ├── jcode.util         # 通用工具集
│   └── jcode.web.util     # Web 层工具集
├── core/                  # 应用服务：iapplication / application / controllers
├── language/              # 多语言：ilanguage / language(+controllers)
└── quartz/                # 任务调度：iquartz / quartz(+controllers)
jcode.webapi/              # API 宿主（启动、配置、SignalR、Quartz 接入）
```

## 技术栈

- .NET 10 / ASP.NET Core Web API
- Autofac（依赖注入）、Quartz（任务调度）、SignalR（实时通信）
- Swashbuckle（Swagger 接口文档）、Newtonsoft.Json
- 数据库：SqlServer / Oracle / MySql / Sqlite（可配置，支持读写分离）
- 缓存：MemoryCache / Redis
- NPOI、Magick.NET（文档渲染与图像处理）

## 快速开始

前置条件：安装 .NET 10 SDK（Docker 部署则仅需 Docker）。

```bash
# 还原与编译
dotnet build jcode.framework.sln

# 启动 API（默认使用 wwwroot/database/jcode.db 的 SQLite 本地库）
dotnet run --project jcode.webapi
```

配置文件：[jcode.webapi/appsettings.json](jcode.webapi/appsettings.json)，默认以 SQLite 本地文件库启动，切换数据库只需修改 `ServerOp.dbType` 与 `ServerOp.dbConn`。生产环境请务必修改 `ServerOp.JwtSecret` 等密钥项。

### Docker

```bash
docker build .              # 默认使用官方 aspnet:10.0 运行时，监听 8080
docker build --build-arg RUNTIME_IMAGE=<带 LibreOffice 的运行时镜像> .   # 需要服务端文档转换时
```

## 参与贡献

1. Fork 本仓库并新建特性分支；
2. 提交前确保 `dotnet build jcode.framework.sln` 通过；
3. 提交 Pull Request 并说明改动背景与影响范围。

依据 Apache License 2.0 第 5 条，除你另有明确声明外，你有意提交以纳入本项目的贡献均按本许可证条款授权。

## 社区交流

扫码加入 QQ 交流群，或添加微信联系作者（请注明来意）：

<table>
  <tr>
    <td align="center">
      <b>QQ 交流群 · 1132049639</b><br>
      <img src="docs/images/qq-group-qr.png" width="220" alt="QQ 交流群二维码">
    </td>
    <td align="center">
      <b>微信 · tobin</b><br>
      <img src="docs/images/wechat-qr.png" width="220" alt="微信二维码">
    </td>
  </tr>
</table>

---

## 许可证

本项目基于 [Apache License 2.0](LICENSE) 开源发布，版权归 苏州极简搭信息技术有限公司 所有。

- 完整许可条款：[LICENSE](LICENSE)
- 归属声明：[NOTICE](NOTICE)

在遵守许可证的前提下，你可以自由使用、修改、分发本项目（包括商业用途与闭源集成），但需保留版权、专利、商标与归属声明，并在修改过的文件中标注变更。本项目按「现状」提供，不附带任何明示或暗示的担保。