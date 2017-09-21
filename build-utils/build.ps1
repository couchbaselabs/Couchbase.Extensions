# is this a tagged build?
if ($env:APPVEYOR_REPO_TAG -eq "true") {
    # use tag as version
    $versionNumber = "$env:APPVEYOR_REPO_TAG_NAME"
} else {
    # create pre-release build number based on AppVeyor build number
    $buildCounter = "$env:APPVEYOR_BUILD_NUMBER".PadLeft(6, "0")
    $versionNumber = .\build-utils\AutoVersionNumber.ps1 -VersionSuffix "alpha-$buildCounter"
}

Write-Host "Using version: $versionNumber"
Update-AppveyorBuild -Version $versionNumber

# clean then build with snk & version number creating nuget package
msbuild Couchbase.Extensions.sln /t:Clean,Restore,Pack /p:Configuration=Release /p:version=$versionNumber /p:PackageOutputPath=..\..\ /p:IncludeSymbols=true /p:IncludeSource=true /v:quiet
