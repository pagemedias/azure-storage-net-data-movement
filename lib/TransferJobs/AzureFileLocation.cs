//------------------------------------------------------------------------------
// <copyright file="AzureFileLocation.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Storage.DataMovement
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using Microsoft.WindowsAzure.Storage.Auth;
    using Microsoft.WindowsAzure.Storage.DataMovement.SerializationHelper;
    using Microsoft.WindowsAzure.Storage.File;

    [Serializable]
    internal class AzureFileLocation : TransferLocation, ISerializable
    {
        private const string AzureFileName = "AzureFile";
        private const string AccessConditionName = "AccessCondition";
        private const string CheckedAccessConditionName = "CheckedAccessCondition";
        private const string RequestOptionsName = "RequestOptions";
        private const string ETagName = "ETag";

        private SerializableAccessCondition accessCondition;
        private SerializableRequestOptions requestOptions;
        private SerializableCloudFile fileSerializer;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureFileLocation"/> class.
        /// </summary>
        /// <param name="azureFile">CloudFile instance as a location in a transfer job. 
        /// It could be a source, a destination.</param>
        public AzureFileLocation(CloudFile azureFile)
        { 
            if (null == azureFile)
            {
                throw new ArgumentNullException("azureFile");
            }

            this.AzureFile = azureFile;
        }

        private AzureFileLocation(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }

            this.fileSerializer = (SerializableCloudFile)info.GetValue(AzureFileName, typeof(SerializableCloudFile));
            this.accessCondition = (SerializableAccessCondition)info.GetValue(AccessConditionName, typeof(SerializableAccessCondition));
            this.CheckedAccessCondition = info.GetBoolean(CheckedAccessConditionName);
            this.requestOptions = (SerializableRequestOptions)info.GetValue(RequestOptionsName, typeof(SerializableRequestOptions));
            this.ETag = info.GetString(ETagName);
        }

        /// <summary>
        /// Gets transfer location type.
        /// </summary>
        public override TransferLocationType Type
        {
            get
            {
                return TransferLocationType.AzureFile;
            }
        }

        /// <summary>
        /// Gets or sets access condition for this location.
        /// This property only takes effact when the location is a blob or an azure file.
        /// </summary>
        public AccessCondition AccessCondition 
        {
            get
            {
                return SerializableAccessCondition.GetAccessCondition(this.accessCondition);
            }

            set
            {
                SerializableAccessCondition.SetAccessCondition(ref this.accessCondition, value);
            }
        }

        /// <summary>
        /// Gets the type for this location.
        /// </summary>
        public TransferLocationType TransferLocationType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets azure file location in this instance.
        /// </summary>
        public CloudFile AzureFile
        {
            get
            {
                return SerializableCloudFile.GetFile(this.fileSerializer);
            }

            private set
            {
                SerializableCloudFile.SetFile(ref this.fileSerializer, value);
            }
        }

        internal string ETag
        {
            get;
            set;
        }

        internal bool CheckedAccessCondition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets FileRequestOptions when send request to this location.
        /// </summary>
        internal FileRequestOptions FileRequestOptions
        {
            get
            {
                return (FileRequestOptions)SerializableRequestOptions.GetRequestOptions(this.requestOptions);
            }

            set
            {
                SerializableRequestOptions.SetRequestOptions(ref this.requestOptions, value);
            }
        }


        /// <summary>
        /// Validates the transfer location.
        /// </summary>
        public override void Validate()
        {
            this.AzureFile.Parent.FetchAttributes(null, Transfer_RequestOptions.DefaultFileRequestOptions);
        }

        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="info">Serialization info object.</param>
        /// <param name="context">Streaming context.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new System.ArgumentNullException("info");
            }

            info.AddValue(AzureFileName, this.fileSerializer, typeof(SerializableCloudFile));
            info.AddValue(AccessConditionName, this.accessCondition, typeof(SerializableAccessCondition));
            info.AddValue(CheckedAccessConditionName, this.CheckedAccessCondition);
            info.AddValue(RequestOptionsName, this.requestOptions, typeof(SerializableRequestOptions));
            info.AddValue(ETagName, this.ETag);
        }

        /// <summary>
        /// Update credentials of blob or azure file location.
        /// </summary>
        /// <param name="credentials">Storage credentials to be updated in blob or azure file location.</param>
        public void UpdateCredentials(StorageCredentials credentials)
        {
            this.fileSerializer.UpdateStorageCredentials(credentials);
        }

        //
        // Summary:
        //     Returns a string that represents the transfer location.
        //
        // Returns:
        //     A string that represents the transfer location.
        public override string ToString()
        {
            return this.AzureFile.Uri.ToString();
        }
    }
}
