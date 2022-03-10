dotnet publish src/Wiper/Wiper.csproj -c Release -r win-x86 -o dist_win_x86

rm -rf WiperTool_win_x86
mkdir WiperTool_win_x86
mv dist_win_x86 WiperTool_win_x86/src

cp tools/run.bat WiperTool_win_x86/
