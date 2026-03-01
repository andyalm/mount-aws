# Secrets Manager on MountAws

## Path Hierarchy

```
-- secretsmanager
   |-- secrets
       |-- my-secret                    # A secret with a simple name
       |-- my-app                       # A virtual folder (prefix of other secrets)
           |-- prod                     # Another virtual folder
               |-- database-credentials # A secret named "my-app/prod/database-credentials"
```

Secret names containing `/` are automatically organized into a virtual folder hierarchy, so you can browse them like a directory tree.

## Browsing Secrets

```powershell
# List all top-level secrets and folders
dir aws:/default/us-east-1/secretsmanager/secrets

# Navigate into a folder
cd aws:/default/us-east-1/secretsmanager/secrets/my-app/prod
dir
```

## Reading Secret Values

Use `Get-Content` to read the raw secret string value:

```powershell
Get-Content aws:/default/us-east-1/secretsmanager/secrets/my-secret
```

For secrets that contain a JSON object, use `Get-ItemProperty` to read individual key/value pairs:

```powershell
# Get all properties
Get-ItemProperty aws:/default/us-east-1/secretsmanager/secrets/my-app/prod/database-credentials

# Get a specific property
Get-ItemProperty aws:/default/us-east-1/secretsmanager/secrets/my-app/prod/database-credentials -Name password
```

## Writing Secret Values

Use `Set-Content` to write a new raw secret string value:

```powershell
Set-Content aws:/default/us-east-1/secretsmanager/secrets/my-secret -Value "new-value"
```

For JSON secrets, use `Set-ItemProperty` to update individual keys without overwriting the entire secret:

```powershell
Set-ItemProperty aws:/default/us-east-1/secretsmanager/secrets/my-app/prod/database-credentials -Name password -Value "new-password"
```

## Item Properties

Each secret item contains all of the properties contained on a [SecretListEntry](https://docs.aws.amazon.com/secretsmanager/latest/apireference/API_SecretListEntry.html). This includes the following properties:

| Property | Description |
|---|---|
| Name | Last segment of the secret name |
| SecretName | Full secret name (e.g., `my-app/prod/database-credentials`) |
| Description | Secret description |
| Arn | Secret ARN |
| LastChangedDate | When the secret was last modified |
| CreatedDate | When the secret was created |
