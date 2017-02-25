iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))
choco install mongodb -y
Install-ChocolateyVsixPackage -PackageName "SerilogAnalyzer" `
  -VsixUrl https://marketplace.visualstudio.com/items?itemName=Suchiman.SerilogAnalyzer