# MountAws
An experimental powershell provider that allows you to browse various aws services as a filesystem

## Installation

```powershell
Install-Module MountAws

# Consider adding the following line to your profile:
Import-Module MountAws
```

## Usage

A drive called `aws` is automatically added to your powershell module when you import the module.
You can run `dir` within any directory to list the objects within. This essentially provides a self-documented way
of navigating the aws service you are using. Tab completion works as well.

The top 3 levels of the virtual filesystem is constructed like this:

```
aws:/<aws-profile-name>/<region>/<aws-service/
```

 * `aws-profile-name` - An aws profile defined in your `~/.aws/config` file used to authenticate with aws
 * `region` - The region in which you wish to explore (e.g. `us-east-1`)
 * `aws-service` - The aws service you wish to explore. The supported services are documented below, or you can always run `dir` within the regions directory for a complete listing.

For an example of how it works. Here is an example of navigating an s3 bucket:

```powershell
$ cd aws:/default/us-east-1/s3/buckets
$ dir

Name
----
my-bucket
another-my-bucket

$ cd my-bucket
$ dir

Name    Description
----    -----------
policy  A virtual file whose content contains the bucket policy
objects Navigate the objects in the s3 bucket like a filesystem

$ cd objects
$ dir

Name                  ItemType
----                  --------
my-directory          Directory
another-directory     Directory

$ cd my-directory

Name                     ItemType
----                     --------
my-subdirectory          Directory
my-file.json             File

$ gc my-file.json
{ "a-key": 3 }
```

## Supported AWS Services

The following services are currently supported:

 * [EC2](docs/Services/EC2.md)
 * [ECR](docs/Services/ECR.md)
 * [ECS](docs/Services/ECS.md)
 * [ELBV2](docs/Services/ELBV2.md)
 * [Route53](docs/Services/Route53.md)
 * [S3](docs/Services/S3.md)
 * [WAFv2](docs/Services/WAFv2.md)

## Supported utility commands

 * `Switch-MountAwsProfile` - Will navigate you to the equivalent path for the given aws profile
 * `Switch-MountAwsRegion` - Will navigate you to the equivalent path for the given aws region

## Contributing

I'm only one guy, so have focused my efforts on supporting services I'm familiar with and use frequently. I plan to add support for more services as I have time. However, I also welcome contributions from others. Feel free to submit PR's that support additional services. I have put together a [developer guide](docs/DeveloperGuide.md) to help you figure out how to contribute.