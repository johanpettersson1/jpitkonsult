kill $(ps aux | grep 'jpitkonsult.dll' | awk '{print $2}')
dotnet publish
dotnet bin/Debug/netcoreapp3.1/publish/jpitkonsult.dll &
