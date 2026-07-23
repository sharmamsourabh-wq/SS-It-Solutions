@echo off
color 0A
title Auto Push to GitHub
echo ========================================
echo        SS IT SOLUTIONS AUTO PUSH
echo ========================================
echo.

:: Set the correct path for Git
set PATH=%PATH%;C:\Program Files\Microsoft Visual Studio\18\Community\Common7\IDE\CommonExtensions\Microsoft\TeamFoundation\Team Explorer\Git\cmd

echo [1/3] Adding all files to Git...
git add .
echo.

echo [2/3] Committing changes...
git commit -m "Auto Update: Website changes saved"
echo.

echo [3/3] Pushing to GitHub...
git push origin main
echo.

echo ========================================
echo        PUSH COMPLETED!
echo ========================================
pause
