FROM microsoft/dotnet:2.1-sdk AS build
LABEL Name=fluksocore Version=0.0.1
COPY . ./fluksocore  
WORKDIR /fluksocore/ 
RUN dotnet build -c Release -o output 

FROM microsoft/dotnet:2.1-runtime AS runtime
COPY --from=build /fluksocore/output .
ENTRYPOINT ["dotnet", "FluksoCore.dll"]