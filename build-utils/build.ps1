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
msbuild src/Couchbase.Extensions.DependencyInjection/Couchbase.Extensions.DependencyInjection.csproj /t:Clean,Restore,Pack /p:Configuration=Release /p:version=$versionNumber /p:PackageOutputPath=..\..\ /p:IncludeSymbols=true /p:IncludeSource=true /v:quiet
msbuild src/Couchbase.Extensions.Caching/Couchbase.Extensions.Caching.csproj /t:Clean,Restore,Pack /p:Configuration=Release /p:version=$versionNumber /p:PackageOutputPath=..\..\ /p:IncludeSymbols=true /p:IncludeSource=true /v:quiet
msbuild src/Couchbase.Extensions.Session/Couchbase.Extensions.Session.csproj /t:Clean,Restore,Pack /p:Configuration=Release /p:version=$versionNumber /p:PackageOutputPath=..\..\ /p:IncludeSymbols=true /p:IncludeSource=true /v:quiet
msbuild src/Couchbase.Extensions.DnsDiscovery/Couchbase.Extensions.DnsDiscovery.csproj /t:Clean,Restore,Pack /p:Configuration=Release /p:version=$versionNumber /p:PackageOutputPath=..\..\ /p:IncludeSymbols=true /p:IncludeSource=true /v:quiet
