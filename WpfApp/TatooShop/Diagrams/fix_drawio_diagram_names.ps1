$ErrorActionPreference = 'Stop'

$telegramDir = 'C:\Users\diana\Downloads\Telegram Desktop'
$files = @(
    (Join-Path $telegramDir 'class_diagram.drawio')
)

$architecture = Get-ChildItem -LiteralPath $telegramDir -Filter '*.drawio' |
    Where-Object { $_.Name -ne 'class_diagram.drawio' -and $_.Length -gt 15000 -and $_.Length -lt 25000 } |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 -ExpandProperty FullName

$files += $architecture
$encoding = New-Object System.Text.UTF8Encoding($false)

foreach ($file in $files) {
    $content = [System.IO.File]::ReadAllText($file, $encoding)
    $content = [regex]::Replace($content, '(<diagram\b[^>]*\bname=")([^"]*)(")', '${1}Page-1${3}', 1)
    [System.IO.File]::WriteAllText($file, $content, $encoding)
}
