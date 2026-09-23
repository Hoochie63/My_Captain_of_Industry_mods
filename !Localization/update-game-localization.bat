@echo off
setlocal EnableExtensions
chcp 65001 >nul

:: ==========================================================
:: Перезапуск от имени администратора
:: ==========================================================

fltmc >nul 2>&1
if errorlevel 1 (
    echo Требуются права администратора...
    powershell -NoProfile -ExecutionPolicy Bypass ^
        -Command "Start-Process -FilePath '%ComSpec%' -ArgumentList '/c','\"%~f0\"' -Verb RunAs"
    exit /b
)

echo ========================================
echo   Captain of Industry Localization
echo ========================================
echo.

:: ==========================================================
:: Исходный файл
:: ==========================================================

set "SOURCE_FILE=%APPDATA%\Captain of Industry\Mods\!Localization\game\ru.json"
set "ALT_MESSAGE=Updating game localization..."

if not exist "%SOURCE_FILE%" (
    echo [ERROR] Source file not found:
    echo %SOURCE_FILE%
    pause
    exit /b
)

:: ==========================================================
:: Основная версия
:: ==========================================================

call :CreateLink ^
    "F:\Steam\steamapps\common\Captain of Industry\Translations\ru.json" ^
    "Main game (ru)"

:: ==========================================================
:: Альтернативная версия
:: ==========================================================

echo.

set "ALT_GAME="
set "ALT_MESSAGE=Searching alternative version..."
set "ALT_MESSAGE=Updating game localization..."

for /d %%D in ("I:\Download's games\Captain of Industry*") do (
    call set "ALT_GAME=%%~fD"
    goto FoundAlt
)

goto EndAlt

:FoundAlt

call :CreateLink ^
    "%ALT_GAME%\Translations\ru.json" ^
    "Alternative game (ru)"

:EndAlt

:: ==========================================================
:: Changelog
:: ==========================================================

echo.

set "SOURCE_FILE=%APPDATA%\Captain of Industry\Mods\!Localization\changelog\ru.json"
set "ALT_MESSAGE=Updating changelog localization..."

call :CreateLink ^
    "F:\Steam\steamapps\common\Captain of Industry\Translations\Changelog\ru.json" ^
    "Main game (changelog)"

:: ==========================================================
:: English version
:: ==========================================================

echo.

set "SOURCE_FILE=F:\Steam\steamapps\common\Captain of Industry\Translations\en.json"
set "ALT_MESSAGE=Updating link for English localization..."

call :CreateLink ^
    "%APPDATA%\Captain of Industry\Mods\!Localization\game\en.json" ^
    "!Localization (en)"
echo.

echo ========================================
echo Done.
echo ========================================
pause
exit /b

:: ==========================================================
:: CreateLink
:: %1 = Target file
:: %2 = Friendly name
:: ==========================================================

:CreateLink

echo ----------------------------------------
echo %~2
echo ----------------------------------------

if defined ALT_MESSAGE (
    call echo %%ALT_MESSAGE%%
    set "ALT_MESSAGE="
)

if exist "%~1" (
    echo Removing old file...
    del /f /q "%~1" >nul 2>&1
)

echo Creating symbolic link...

mklink "%~1" "%SOURCE_FILE%" >nul

if errorlevel 1 (
    echo [FAILED]
) else (
    echo [OK]
)

goto :eof
