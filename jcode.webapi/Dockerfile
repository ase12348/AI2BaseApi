# 口径与仓库根目录的 Dockerfile 一致（CI 只用根目录那份，本文件供 IDE 按项目构建），除本行注释外两份必须逐字同步。
# 运行时口径：默认官方 aspnet:10.0；需要服务端 LibreOffice 预览时用 --build-arg RUNTIME_IMAGE=<团队 10.0-office 镜像> 覆盖。
# 构建上下文仍是 dotnet-api 根目录（两个 COPY 都按 jcode.webapi/... 取路径），端口 8080。
ARG RUNTIME_IMAGE=mcr.microsoft.com/dotnet/aspnet:10.0

FROM ${RUNTIME_IMAGE} AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["jcode.webapi/jcode.webapi.csproj", "jcode.webapi/"]
RUN dotnet restore "jcode.webapi/jcode.webapi.csproj"
COPY . .
WORKDIR "/src/jcode.webapi"
RUN dotnet build "jcode.webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "jcode.webapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone && \
    sed -i 's/TLSv1.2/TLSv1/g' /etc/ssl/openssl.cnf
ENTRYPOINT ["dotnet", "jcode.webapi.dll"]
