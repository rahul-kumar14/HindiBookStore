# FTP Deployment Script for Somee.com
# This script uploads all files from publish folder to Somee.com via FTP

param(
    [string]$FtpServer = "ftp://anchor.somee.com",
    [string]$Username = "anchor",
    [string]$Password = "Rahul@123",
    [string]$LocalPath = ".\publish",
    [string]$RemotePath = "/"
)

Write-Host "🚀 Starting FTP Deployment to Somee.com..." -ForegroundColor Cyan

# Function to upload a file
function Upload-File {
    param($LocalFile, $RemoteFile)
    
    try {
        $uri = "$FtpServer$RemoteFile"
        $webclient = New-Object System.Net.WebClient
        $webclient.Credentials = New-Object System.Net.NetworkCredential($Username, $Password)
        
        Write-Host "  Uploading: $($LocalFile.Name)..." -NoNewline
        $webclient.UploadFile($uri, $LocalFile.FullName)
        Write-Host " ✅" -ForegroundColor Green
        
        $webclient.Dispose()
        return $true
    }
    catch {
        Write-Host " ❌ Error: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Function to create directory
function Create-FtpDirectory {
    param($RemoteDir)
    
    try {
        $uri = "$FtpServer$RemoteDir"
        $request = [System.Net.FtpWebRequest]::Create($uri)
        $request.Credentials = New-Object System.Net.NetworkCredential($Username, $Password)
        $request.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory
        $request.GetResponse() | Out-Null
        Write-Host "  Created directory: $RemoteDir" -ForegroundColor Yellow
        return $true
    }
    catch {
        # Directory might already exist, ignore error
        return $false
    }
}

# Get all files from publish folder
$files = Get-ChildItem -Path $LocalPath -Recurse -File

Write-Host "`n📊 Total files to upload: $($files.Count)" -ForegroundColor Cyan
Write-Host "📁 Source: $LocalPath" -ForegroundColor Cyan
Write-Host "🌐 Destination: $FtpServer" -ForegroundColor Cyan
Write-Host "`n"

$uploaded = 0
$failed = 0

# Create directory structure first
$directories = Get-ChildItem -Path $LocalPath -Recurse -Directory | 
    ForEach-Object { $_.FullName.Replace((Resolve-Path $LocalPath).Path, "").Replace("\", "/") }

foreach ($dir in $directories) {
    Create-FtpDirectory -RemoteDir $dir
}

# Upload files
foreach ($file in $files) {
    $relativePath = $file.FullName.Replace((Resolve-Path $LocalPath).Path, "").Replace("\", "/")
    
    if (Upload-File -LocalFile $file -RemoteFile $relativePath) {
        $uploaded++
    }
    else {
        $failed++
    }
}

Write-Host "`n" -NoNewline
Write-Host "=" * 50 -ForegroundColor Cyan
Write-Host "✅ Uploaded: $uploaded files" -ForegroundColor Green
if ($failed -gt 0) {
    Write-Host "❌ Failed: $failed files" -ForegroundColor Red
}
Write-Host "=" * 50 -ForegroundColor Cyan
Write-Host "`n🎉 Deployment Complete!" -ForegroundColor Green
Write-Host "🌐 Visit: http://anchor.somee.com" -ForegroundColor Cyan
