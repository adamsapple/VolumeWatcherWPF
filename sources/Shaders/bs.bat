@echo off
set FXC="c:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc.exe"
set PSVER=ps_2_0
set OUTPUT_DIR=..\VolumeWatcherWPF\resources\
rem set PSVER=ps_3_0

set TARGET=%~n1
set INPUT=%TARGET%.fx
set OUTPUT=%TARGET%.ps


echo on

%FXC% %INPUT% /T %PSVER% /Fo %OUTPUT%

copy %OUTPUT% %OUTPUT_DIR%
