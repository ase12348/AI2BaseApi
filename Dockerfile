# 容器构建以本文件为准：CI（.cnb.yml）在 dotnet-api 根目录执行 `docker build .`，容器监听 8080（部署映射 8110:8080）。
# 运行时口径：默认官方 aspnet:10.0，net10 产物直接可跑；附件里的 doc/ppt/dxf 预览会因缺 LibreOffice 退成“下载查看”。
# 需要服务端 LibreOffice 转换时，用团队私有 10.0-office 镜像覆盖（旧的 ...:8.0.1-office 是 .NET 8，跑不了 net10 产物）：
#   docker build --build-arg RUNTIME_IMAGE=jcodeteam-docker.pkg.coding.net/jcodelowcode/net/runtime:<10.0-office tag> .
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
