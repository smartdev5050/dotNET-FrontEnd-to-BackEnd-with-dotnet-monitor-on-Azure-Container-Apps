param name string
param location string = resourceGroup().location
param containerAppEnvironmentId string
param repositoryImage string = 'mcr.microsoft.com/azuredocs/containerapps-banking-app:latest'
param azureBlobStorageAccountUri string = 'https://somestorage.blob.core.windows.net'
param envVars array = []
param registry string
param minReplicas int = 1
param maxReplicas int = 1
param port int = 80
param externalIngress bool = false
param allowInsecure bool = true
param transport string = 'http'
param registryUsername string

@secure()
param registryPassword string

@secure()
param storageKey string

param secrets array = [
  {
    name: 'storagekey'
    value: storageKey
  }
]

resource containerApp 'Microsoft.App/containerApps@2022-01-01-preview' ={
  name: name
  location: location
  properties:{
    managedEnvironmentId: containerAppEnvironmentId
    configuration: {
      activeRevisionsMode: 'single'
      secrets: [
        {
          name: 'container-registry-password'
          value: registryPassword
        }
      ]      
      registries: [
        {
          server: registry
          username: registryUsername
          passwordSecretRef: 'container-registry-password'
        }
      ]
      ingress: {
        external: externalIngress
        targetPort: port
        transport: transport
        allowInsecure: allowInsecure
      }
    }
    template: {
      volumes: [
        {
          name: 'diagport'
          storageType: 'EmptyDir'
        }
      ]
      containers: [
        {
          image: repositoryImage
          name: name
          env: envVars
          volumeMounts:[
            {
              mountPath: '/diagport'
              volumeName: 'diagport'              
            } 
          ]          
          env: [
            {
              name: 'DOTNET_DiagnosticPorts'
              value: '/diagport/port.sock'
            }
          ]
        }

        {
          image: 'mcr.microsoft.com/dotnet/monitor:6'
          name: 'dotnet-monitor'
          volumeMounts:[
            {
              mountPath: '/diagport'
              volumeName: 'diagport'              
            } 
          ]
          command: [
            'dotnet-monitor'
          ]
          args: [
            'collect'
            '--no-auth'
          ]
          env: [
            {
              name: 'DotnetMonitor_DiagnosticPort__ConnectionMode'
              value: 'Listen'
            }
            {
              name: 'DotnetMonitor_Storage__DumpTempFolder'
              value: '/diagport'
            }
            {
              name: 'DotnetMonitor_Urls'
              value: 'http://*:52323'
            }
            {
              name: 'DotnetMonitor_DiagnosticPort__EndpointName'
              value: '/diagport/port.sock'
            }
            {
              name: 'Egress__AzureBlobStorage__monitorBlob__AccountUri'
              value: azureBlobStorageAccountUri
            }
            {
              name: 'Egress__AzureBlobStorage__monitorBlob__ContainerName'
              value: 'dotnet-monitor'
            }
            {
              name: 'Egress__AzureBlobStorage__monitorBlob__BlobPrefix'
              value: 'artifacts'
            }
            {
              name: 'Egress__AzureBlobStorage__monitorBlob__AccountKey'
              secretRef: 'storagekey'
            }
            {
              name: 'CollectionRules__Startup__Trigger__Type'
              value: 'EventCounter'
            }
            {
              name: 'CollectionRules__AspnetDuration__Trigger__Settings__RequestCount'
              value: '1'
            }
            {
              name: 'CollectionRules__AspnetDuration__Trigger__Settings__SlidingWindowDuration'
              value: '00:02:00'
            }
            {
              name: 'CollectionRules__AspnetDuration__Trigger__Settings__RequestDuration'
              value: '00:00:40'
            }
            {
              name: 'CollectionRules__AspnetDuration__Actions__0__Type'
              value: 'CollectDump'
            }
            {
              name: 'CollectionRules__AspnetDuration__Actions__0__Settings__Egress'
              value: 'monitorBlob'
            }
            {
              name: 'CollectionRules__AspnetDuration__Actions__0__Settings__Type'
              value: 'Full'
            }
            {
              name: 'CollectionRules__AspnetDuration__Limits__ActionCount'
              value: '1'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Trigger__Type'
              value: 'EventCounter'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Trigger__Settings__ProviderName'
              value: 'System.Runtime'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Trigger__Settings__CounterName'
              value: 'threadpool-queue-length'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Trigger__Settings__GreaterThan'
              value: '30.0'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Trigger__Settings__SlidingWindowDuration'
              value: '00:00:30'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Actions__0__Type'
              value: 'CollectDump'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Actions__0__Settings__Type'
              value: 'Full'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Limits__ActionCount'
              value: '1'
            }
            {
              name: 'CollectionRules__ThreadPoolQueuing__Actions__0__Settings__Egress'
              value: 'monitorBlob'
            }
          ]
        }


      ]

      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
