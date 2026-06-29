$ErrorActionPreference = 'Stop'

$telegramDir = 'C:\Users\diana\Downloads\Telegram Desktop'
$files = @(
    (Join-Path $telegramDir 'class_diagram.drawio')
)

$architecture = Get-ChildItem -LiteralPath $telegramDir -Filter '*.drawio' |
    Where-Object { $_.Name -ne 'class_diagram.drawio' -and $_.Length -gt 15000 -and $_.Length -lt 25000 } |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1 -ExpandProperty FullName

if (-not $architecture) {
    throw 'Architecture drawio file was not found.'
}

$files += $architecture
$encoding = New-Object System.Text.UTF8Encoding($false)

foreach ($file in $files) {
    [xml]$xml = Get-Content -LiteralPath $file -Raw

    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Encoding = $encoding
    $settings.Indent = $true
    $settings.OmitXmlDeclaration = $true
    $settings.NewLineChars = "`n"

    $stringBuilder = New-Object System.Text.StringBuilder
    $writer = [System.Xml.XmlWriter]::Create($stringBuilder, $settings)
    $xml.Save($writer)
    $writer.Close()

    [System.IO.File]::WriteAllText($file, $stringBuilder.ToString(), $encoding)
}
