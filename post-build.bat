:: %1 $(ConfigurationName)
:: %2 $(ProjectName)
:: %3 $(ProjectDir)
:: %4 $(TargetPath)
:: %5 $(TargetDir)

::echo %%1 %1
::echo %%2 %2
::echo %%3 %3
::echo %%4 %4
::echo %%5 %5

echo %1 | findstr /c:"testbuild" 1>nul && goto :eof

set tld_path=%ProgramFiles(x86)%\Steam\steamapps\common\TheLongDark
if not exist "%tld_path%" goto :eof
echo %tld_path%

set mods_path=%tld_path%\Mods
if not exist "%mods_path%" goto :eof
echo %mods_path%

::set userdata_path=%tld_path%\UserData
::if not exist "%userdata_path%" goto :eof

set C=copy /Y %2.dll "%mods_path%\"
echo %C%
%C%

::mkdir "%userdata_path%\%2"
::xcopy /Y %3\res\*.wav "%userdata_path%\%2"


