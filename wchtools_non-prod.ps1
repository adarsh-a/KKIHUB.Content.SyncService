npx wchtools init --url https://content-eu-1.content-cms.com/api/37dd7bf6-5628-4aac-8464-f4894ddfb8c4 --user adarsh.bhautoo@hangarww.com --password Ad1108bh_hangarMU
$artifactPath = "$((Get-Location).path)\Artifacts\"
npx wchtools push -c -f --dir $artifactPath --password Ad1108bh_hangarMU
# Read-Host "Wait for a key press to exit"
