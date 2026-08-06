@echo off
chcp 65001 >nul
setlocal enabledelayedexpansion

echo ============================================
echo   Проверка и обновление файлов по дате
echo ============================================
echo.

REM Читаем список пар "источник|назначение" из конца этого же файла
for /f "usebackq tokens=1,2,3 delims=|" %%A in (`findstr /b /C:"PAIR|" "%~f0"`) do (
    call :process "%%B" "%%C"
)

echo.
echo ============================================
echo   Готово
echo ============================================
pause
exit /b

:process
set "SRC=%~1"
set "DST=%~2"

REM Проверка, что источник существует
if not exist "!SRC!" (
    echo [ОШИБКА]    Источник не найден:    !SRC!
    goto :eof
)

REM Если назначения нет - просто предупреждаем, ничего не копируем
if not exist "!DST!" (
    echo [ВНИМАНИЕ]  Назначение НЕ найдено:  !DST!
    goto :eof
)

REM Сравнение дат изменения через PowerShell (надёжнее, чем строками в batch)
for /f %%T in ('powershell -NoProfile -Command "if ((Get-Item -LiteralPath '!SRC!').LastWriteTime -gt (Get-Item -LiteralPath '!DST!').LastWriteTime) {'NEWER'} else {'OLDER'}"') do set "RESULT=%%T"

if "!RESULT!"=="NEWER" (
    echo [КОПИРУЮ]   Источник новее:         !SRC!
    echo             -^> !DST!
    copy /Y "!SRC!" "!DST!" >nul
) else (
    echo [ПРОПУСК]   Назначение свежее/равно: !DST!
)

echo.
goto :eof

REM ================================================================
REM  СПИСОК ПАР ФАЙЛОВ — редактируй только эту часть.
REM  Формат каждой строки:  PAIR|путь_к_источнику|путь_к_назначению
REM  Чтобы добавить файл - допиши новую строку PAIR|...|...
REM  Чтобы удалить - удали строку или закомментируй (:: в начале)
REM ================================================================
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Adaptive-mining-tower-ui-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\adaptive-mining-tower-ui\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Adaptive-mining-tower-ui.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\adaptive-mining-tower-ui\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\AutoForestryDesignations.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\AutoForestryDesignations\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\AutoTerrainDesignations.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\AutoTerrainDesignations\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\BetterLife_Assemblies.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\BetterLife_Assemblies\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\BetterLife_Buildings.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\BetterLife_Buildings\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\BetterLife_RoadsAndSigns.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\BetterLife_RoadsAndSigns\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\BetterLife_Transports.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\BetterLife_Transports\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\BetterLife_Walls.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\BetterLife_Walls\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Boost-plus-plus-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\boost-plus-plus\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Boost-plus-plus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\boost-plus-plus\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CAP-PierWall-LiquidDump.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CAP-PierWall-LiquidDump\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CAP-PierWall-SeawaterPump.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CAP-PierWall-SeawaterPump\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Carbon.ShowTerrainGrid.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\Carbon.ShowTerrainGrid\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CargoHelicopter-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CargoHelicopter\config.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CargoHelicopter.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CargoHelicopter\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Cheat-plus-plus-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\cheat-plus-plus\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Cheat-plus-plus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\Cheat-plus-plus\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\COIExtended-Cheats.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\COIExtended-Cheats\translations\ru.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\COIExtended-Common.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\COIExtended-Common\translations\ru.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\COIExtended-Difficulty.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\COIExtended-Difficulty\translations\ru.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\COIExtended-RecipeMaker.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\COIExtended-RecipeMaker\translations\ru.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\COIExtended-Sanitizer.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\COIExtended-Sanitizer\translations\ru.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\COIExtended-VehicleMaker.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\COIExtended-VehicleMaker\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ColibriIndustries.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ColibriIndustries\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ConfigurableMachineOutputs-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ConfigurableMachineOutputs\config.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ConfigurableMachineOutputs.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ConfigurableMachineOutputs\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CustomAssets.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CustomAssets\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CustomAssets_AirFiltering.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CustomAssets_AirFiltering\Translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CustomAssets_LiquifiedCoal.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CustomAssets_LiquifiedCoal\Translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CustomPipes-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CustomPipes\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CustomPipes-manifest.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CustomPipes\manifest.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\CustomPipes.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\CustomPipes\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\DesignerToolkit-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\DesignerToolkit\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\DesignerToolkit.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\DesignerToolkit\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ElevationPP-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ElevationPP\config.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ElevationPP.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ElevationPP\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ExtraTransports.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ExtraTransports\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\FaFOptimiser.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\FaFOptimiser\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\FGWY.Logistics-manifest.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\FGWY.Logistics\manifest.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\FGWY.Logistics.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\FGWY.Logistics\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Fusion_Horizon.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\Fusion_Horizon\Translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Gameplay-plus-plus-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\gameplay-plus-plus\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Gameplay-plus-plus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\gameplay-plus-plus\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\H4uklotz.VerticalBalancers.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\H4uklotz.VerticalBalancers\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\IndustrialExpansion.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\IndustrialExpansion\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\IserikFlowMeter.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\IserikFlowMeter\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Keybind-framework.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\keybind-framework\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\MechanicalShafts.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\MechanicalShafts\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\MetallurgyPlus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\MetallurgyPlus\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\MiningDumpingMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\MiningDumpingMod\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ModularRamp.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ModularRamp\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Mor_Decor.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\Mor_Decor\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\MyAirshipMod-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\MyAirshipMod\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\MyAirshipMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\MyAirshipMod\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\OverclockingMod-manifest.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\OverclockingMod\manifest.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\OverclockingMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\OverclockingMod\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\PierWallMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\PierWallMod\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\PillarSpacingMod-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\PillarSpacingMod\config.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\PillarSpacingMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\PillarSpacingMod\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\PlaceResourceMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\PlaceResourceMod\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\PriorityDesignations-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\PriorityDesignations\config.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ProductionCalculator.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ProductionCalculator\Translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\RateCalculator.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\RateCalculator\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Recipes-plus-plus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\recipes-plus-plus\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ResearchQueue.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ResearchQueue\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ShipAutoExplore-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ShipAutoExplore\config.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ShipAutoExplore.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ShipAutoExplore\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\ShippingPP.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\ShippingPP\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\SmartFlareMod-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\SmartFlareMod\config.json
::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\SmartFlareMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\SmartFlareMod\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Speed-plus-plus-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\speed-plus-plus\config.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Storage-plus-plus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\storage-plus-plus\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\TruckParking.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\TruckParking\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Tweaks-plus-plus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\tweaks-plus-plus\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\UndergroundPipes-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\UndergroundPipes\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\UndergroundPipes.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\UndergroundPipes\translations\ru.json

::PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\UndergroundTransportMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\UndergroundTransportMod\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Utilities-plus-plus-config.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\utilities-plus-plus\config.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Utilities-plus-plus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\utilities-plus-plus\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\WindPower.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\WindPower\Translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\Worldgen-plus-plus.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\worldgen-plus-plus\translations\ru.json

PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\WorldMineMod-manifest.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\WorldMineMod\manifest.json
PAIR|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\!Localization\mods\WorldMineMod.json|C:\Users\www\AppData\Roaming\Captain of Industry\Mods\WorldMineMod\translations\ru.json
