
#Create the Certificate (Client)
$cert = New-SelfSignedCertificate -certstorelocation cert:\localmachine\my -dnsname roger_example_identity_server

#Export the Certificate
$pwd = ConvertTo-SecureString -String '1234' -Force -AsPlainText
$path = 'cert:\localMachine\my\' + $cert.thumbprint
Export-PfxCertificate -cert $path -FilePath C:\Users\roger.santos\git\roger\temp\certificate\powershellcert.pfx -Password $pwd
