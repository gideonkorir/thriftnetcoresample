# THRIFT NETCORE SAMPLE

### Requirements

1. Dotnet 2.0 sdk
2. Thrift.exe (Includes a built one in lib folder)

### Compiling:

All instructions assuming we are at the root of the repository

1. Run ```--gen netcore -r -out client\gen-netcore lib\tutorial.thrift```
2. Run ```--gen netcore -r -out server\gen-netcore lib\tutorial.thrift```
3. Run ```dotnet build``` to build the sln file

### Running:

1. Console 1, ```cd server & dotnet run```
2. Console 2, ```cd client & dotnet run```