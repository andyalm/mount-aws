function Switch-MountAwsProfile {
    [Alias('Switch-AwsProfile')]
    param(
        [Parameter(Mandatory=$true, Position=0)]
        $ProfileName
    )
    
    Switch-MountAwsPathSegment 0 profile $ProfileName
}

function Switch-MountAwsRegion {
    [Alias('Switch-AwsRegion')]
    [Alias('Switch-Region')]
    param(
        [Parameter(Mandatory=$true, Position=0)]
        $RegionName
    )

    Switch-MountAwsPathSegment 1 region $RegionName
}

function Switch-MountAwsPathSegment {
    param(
        [Parameter(Mandatory=$true, Position=0)]
        [int]
        $SegmentIndex,
    
        [Parameter(Mandatory=$true, Position=1)]
        [string]
        $SegmentName,
    
        [Parameter(Mandatory=$true, Position=2)]
        [string]
        $NewValue
    )

    $CurrentLocation = Get-Location
    if($CurrentLocation.Provider.Name -ne "MountAws") {
        throw "This command must be called from a MountAws location"
    }

    $PathSeparator = [IO.Path]::DirectorySeparatorChar
    $PathParts = $CurrentLocation.Path.Split($PathSeparator)
    $CurrentValue = $PathParts[$SegmentIndex + 1]
    if(-not $CurrentValue) {
        throw "This command must be called from within a $SegmentName directory of the MountAws provider"
    }

    $PathParts[$SegmentIndex + 1] = $NewValue

    $NewLocation = $PathParts | Join-String -Separator $PathSeparator

    Set-Location $NewLocation
}