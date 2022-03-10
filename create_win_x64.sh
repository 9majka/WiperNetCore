dotnet publish src/Wiper/Wiper.csproj -c Release -r win-x64 -o dist_win_x64

rm -rf WiperTool_win_x64
mkdir WiperTool_win_x64
mv dist_win_x64 WiperTool_win_x64/src

cp tools/run.bat WiperTool_win_x64/
