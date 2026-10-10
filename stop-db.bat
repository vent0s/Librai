@echo off
setlocal EnableExtensions DisableDelayedExpansion
set "result=1"
set "directory_ready="

pushd "%~dp0" >nul 2>&1
if errorlevel 1 (
    echo ERROR: Cannot open the repository directory.
    goto finish
)
set "directory_ready=1"

if not "%~1"=="" if /I not "%~1"=="--no-pause" (
    echo Usage: %~nx0 [--no-pause]
    set "result=2"
    goto finish
)
if not exist "compose.yaml" (
    echo ERROR: compose.yaml is missing beside this script.
    goto finish
)
where docker >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker CLI was not found.
    goto finish
)
docker --context desktop-linux compose version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker Compose is unavailable.
    goto finish
)
docker --context desktop-linux info >nul 2>&1
if errorlevel 1 (
    echo ERROR: The Docker Desktop Linux engine is unavailable.
    goto finish
)

rem Stopping does not use credentials. Satisfy Compose interpolation even
rem when .env is absent; this process-local value never updates the database.
set "LIBRAI_DB_PASSWORD=unused-by-stop"
echo Stopping the LibrAI development database...
docker --context desktop-linux compose --project-name librai-dev --file compose.yaml stop --timeout 30 db
if errorlevel 1 goto finish
echo Database stopped. Its container and data volume have been preserved.
set "result=0"

:finish
if defined directory_ready popd
if /I not "%~1"=="--no-pause" pause
endlocal & exit /b %result%
