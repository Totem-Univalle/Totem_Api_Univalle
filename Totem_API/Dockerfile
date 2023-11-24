FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR webapp

EXPOSE 5001
EXPOSE 5000
#CAMBIAR PUERTO DESPUES

COPY ./*.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /webapp
COPY --from=build /webapp/out .
ENTRYPOINT ["dotnet", "Totem_API.dll", "--urls", "http://*:5001"]
#CAMBIAR EL DLL