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
if not exist ".env" (
    echo ERROR: Create .env beside compose.yaml and set LIBRAI_DB_PASSWORD.
    goto finish
)
where docker >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker CLI was not found. Install Docker Desktop first.
    goto finish
)
docker --context desktop-linux compose version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker Compose is unavailable.
    goto finish
)
docker --context desktop-linux info >nul 2>&1
if errorlevel 1 (
    echo ERROR: Start Docker Desktop with the Linux engine, then retry.
    goto finish
)
docker --context desktop-linux compose --project-name librai-dev --file compose.yaml config --quiet
if errorlevel 1 goto finish

echo Starting the LibrAI development database...
docker --context desktop-linux compose --project-name librai-dev --file compose.yaml up --detach db
if errorlevel 1 goto finish

set "attempt=0"
:wait_ready
docker --context desktop-linux compose --project-name librai-dev --file compose.yaml exec -T db pg_isready --quiet --username=librai --dbname=librai >nul 2>&1
if not errorlevel 1 goto ready
set /a attempt+=1 >nul
if %attempt% GEQ 30 (
    echo ERROR: PostgreSQL did not become ready after 30 checks.
    echo Inspect the database logs with: docker compose logs db
    goto finish
)
powershell.exe -NoLogo -NoProfile -NonInteractive -Command "Start-Sleep -Seconds 1"
goto wait_ready

:ready
echo PostgreSQL is ready at 127.0.0.1:15432. Database: librai.
set "result=0"

:finish
if defined directory_ready popd
if /I not "%~1"=="--no-pause" pause
endlocal & exit /b %result%
