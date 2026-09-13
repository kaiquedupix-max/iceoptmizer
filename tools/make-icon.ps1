$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.Drawing
$bitmap=New-Object System.Drawing.Bitmap 256,256
$graphics=[System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
$graphics.Clear([System.Drawing.Color]::FromArgb(11,21,35))
$polygons=@(
 @{Color='#C3F8FF';Points=@(128,28,60,96,128,124)},
 @{Color='#54D3F0';Points=@(128,28,196,96,128,124)},
 @{Color='#286FAB';Points=@(60,96,128,228,128,124)},
 @{Color='#82DFF7';Points=@(196,96,128,124,128,228)}
)
foreach($poly in $polygons){
 $points=New-Object 'System.Collections.Generic.List[System.Drawing.PointF]'
 for($i=0;$i -lt $poly.Points.Count;$i+=2){$points.Add([System.Drawing.PointF]::new($poly.Points[$i],$poly.Points[$i+1]))}
 $brush=New-Object System.Drawing.SolidBrush ([System.Drawing.ColorTranslator]::FromHtml($poly.Color))
 $graphics.FillPolygon($brush,$points.ToArray());$brush.Dispose()
}
$pen=New-Object System.Drawing.Pen ([System.Drawing.ColorTranslator]::FromHtml('#A8F0FF')),2
$graphics.DrawLine($pen,128,28,128,228);$graphics.DrawLine($pen,60,96,196,96)
$bitmap.Save((Join-Path (Get-Location) 'assets/ice-logo.png'),[System.Drawing.Imaging.ImageFormat]::Png)
$memory=New-Object System.IO.MemoryStream
$bitmap.Save($memory,[System.Drawing.Imaging.ImageFormat]::Png)
$bytes=$memory.ToArray()
$file=[IO.File]::Create((Join-Path (Get-Location) 'assets/ice.ico'))
$writer=New-Object System.IO.BinaryWriter $file
$writer.Write([uint16]0);$writer.Write([uint16]1);$writer.Write([uint16]1)
$writer.Write([byte]0);$writer.Write([byte]0);$writer.Write([byte]0);$writer.Write([byte]0)
$writer.Write([uint16]1);$writer.Write([uint16]32);$writer.Write([uint32]$bytes.Length);$writer.Write([uint32]22);$writer.Write($bytes)
$writer.Dispose();$memory.Dispose();$pen.Dispose();$graphics.Dispose();$bitmap.Dispose()
