# EC2 on MountAws

The EC2 virtual filesystem is stuctured the following way:

`instances/<instance-id>`

## Filtering

While you can list all of the ec2 instances by running `dir instances`, if you are in a large account, that may be overwhelming and a bit slow.
Fortunately, MountAws supports filtering the instances. For example:

```powershell
cd instances

# will list all ec2 instances in the us-east-1a availability zone only
dir -filter "availability-zone=us-east-1a"
```

All filters supported by the EC2 api should be supported. You can find them [documented here](https://docs.aws.amazon.com/cli/latest/reference/ec2/describe-instances.html#options).

### Shortcuts

There are a couple shortcuts offered for convenience. If no key is provided in the filter, its assumed to be an IP address or name filter (depending on whether it looks like an ip address or not).

For example, to filter to instances that have a name that begin with `myapp`, you can do the following:

```powershell
ls -filter 'myapp*'
```

To filter instances that match a particular private IP address, you can do the following:

```powershell
ls -filter '10.51.102.*'
```

The filtering syntax also works for `Get-Item` calls as well. For example, the examples above could also be written as:

```powershell
Get-Item myapp*
gi 10.51.102.*
```

