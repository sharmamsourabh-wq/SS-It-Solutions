@echo off
echo ===================================================
echo Auto Push Website Updates to GitHub
echo ===================================================
echo.
echo Adding changes to git...
git add .
echo.
echo Committing changes...
git commit -m "Auto-update: New version of Solar ERP software added"
echo.
echo Pushing to GitHub...
git push origin main
echo.
echo ===================================================
echo Process Completed! Check the output above for any errors.
echo ===================================================
pause
