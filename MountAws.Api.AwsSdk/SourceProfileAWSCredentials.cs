/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

/*
 * This class has been forked from the AssumeRoleAWSCredentials because the AWSSDK assemblies
 * are being loaded in a custom assembly load context (to avoid assembly conflicts with the aws
 * powershell sdk) and that breaks the existing AssumeRoleAWSCredentials class loads the
 * AWSSDK.SecurityToken assembly.
 */

using System.Globalization;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Internal.Util;
using Amazon.Runtime.SharedInterfaces;
using Amazon.SecurityToken;

namespace MountAws.Api.AwsSdk;

public class SourceProfileAWSCredentials : RefreshingAWSCredentials
  {
    private RegionEndpoint DefaultSTSClientRegion = RegionEndpoint.USEast1;
    private Logger _logger = Logger.GetLogger(typeof (AssumeRoleAWSCredentials));

    public AWSCredentials SourceCredentials { get; }

    public string RoleArn { get; }

    public string RoleSessionName { get; }

    public AssumeRoleAWSCredentialsOptions? Options { get; }

    public SourceProfileAWSCredentials(
      AWSCredentials sourceCredentials,
      string roleArn,
      string roleSessionName)
      : this(sourceCredentials, roleArn, roleSessionName, new AssumeRoleAWSCredentialsOptions())
    {
    }

    /// <summary>Constructs an SourceProfileAWSCredentials object.</summary>
    /// <param name="sourceCredentials">The credentials of the user that will be used to call AssumeRole.</param>
    /// <param name="roleArn">The Amazon Resource Name (ARN) of the role to assume.</param>
    /// <param name="roleSessionName">An identifier for the assumed role session.</param>
    /// <param name="options">Options to be used in the call to AssumeRole.</param>
    public SourceProfileAWSCredentials(
      AWSCredentials sourceCredentials,
      string roleArn,
      string roleSessionName,
      AssumeRoleAWSCredentialsOptions options)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      SourceCredentials = sourceCredentials;
      RoleArn = roleArn;
      RoleSessionName = roleSessionName;
      Options = options;
      PreemptExpiryTime = TimeSpan.FromMinutes(5.0);
    }

    protected override CredentialsRefreshState GenerateNewCredentials()
    {
      string awsRegion = AWSConfigs.AWSRegion;
      RegionEndpoint regionEndpoint = string.IsNullOrEmpty(awsRegion) ? this.DefaultSTSClientRegion : RegionEndpoint.GetBySystemName(awsRegion);
      ICoreAmazonSTS serviceFromAssembly;
      try
      {
        var serviceConfig = new AmazonSecurityTokenServiceConfig();
        serviceConfig.RegionEndpoint = regionEndpoint;
        if (Options != null && Options.ProxySettings != null)
          serviceConfig.SetWebProxy(Options.ProxySettings);
        serviceFromAssembly = new AmazonSecurityTokenServiceClient(SourceCredentials, serviceConfig);
      }
      catch (Exception ex)
      {
        InvalidOperationException operationException = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Assembly {0} could not be found or loaded. This assembly must be available at runtime to use Amazon.Runtime.SourceProfileAWSCredentials.", (object) "AWSSDK.SecurityToken"), ex);
        Logger.GetLogger(typeof (Amazon.Runtime.AssumeRoleAWSCredentials)).Error((Exception) operationException, operationException.Message, Array.Empty<object>());
        throw operationException;
      }
      AssumeRoleImmutableCredentials credentials = serviceFromAssembly.CredentialsFromAssumeRoleAuthentication(this.RoleArn, this.RoleSessionName, this.Options);
      _logger.InfoFormat("New credentials created for assume role that expire at {0}", new object[1]
      {
        credentials.Expiration.ToString("yyyy-MM-ddTHH:mm:ss.fffffffK", CultureInfo.InvariantCulture)
      });
      return new CredentialsRefreshState(credentials, credentials.Expiration);
    }
  }