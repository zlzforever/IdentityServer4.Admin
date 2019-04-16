# First stage of multi-stage build
FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app
# copy the contents of agent working directory on host to workdir in container
COPY . ./

# dotnet commands to build, test, and publish
RUN dotnet restore
# RUN dotnet test dotnetcore-tests/dotnetcore-tests.csproj -c Release --logger "trx;LogFileName=testresults.trx"
RUN dotnet publish -c Release -o out

# Second stage - Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/src/IdentityServer4.Admin/out .
RUN mv /etc/apt/sources.list /etc/apt/sources.list.bak && \
    echo "deb http://mirrors.aliyun.com/debian/ jessie main non-free contrib" >/etc/apt/sources.list && \
    echo "deb-src http://mirrors.aliyun.com/debian/ jessie main non-free contrib" >>/etc/apt/sources.list  
# RUN apt-get update && apt-get install -y libfontconfig1 && apt-get install -y fontconfig
RUN apt-get update && apt-get install -y libfontconfig1
ENTRYPOINT ["dotnet","IdentityServer4.Admin.dll"]