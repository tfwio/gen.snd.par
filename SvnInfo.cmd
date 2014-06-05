set path=%windir%\microsoft.net\framework\v4.0.30319;%path%
set msb=%windir%\microsoft.net\framework\v4.0.30319\msbuild
msbuild svninfo.proj /t:build
pause