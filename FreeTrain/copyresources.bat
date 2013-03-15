xcopy core\res %1\res /D /E /I /Q /Y /EXCLUDE:excludelist.txt
xcopy plugins %1\plugins /D /E /I /Q /Y /EXCLUDE:excludelist.txt
xcopy doc\*.* %1 /D /E /I /Q /Y /EXCLUDE:excludelist.txt
pause