# Service Fabric Graceful Shutdown Application
Test and Simulate Service Fabric Graceful shutdown. (This is not currently working. Further investigation is required)

## Requirements
- .NET Core 2.2
- Service Fabric SDK 3.2.162.9494

## Running
When building the solution, Ensure that Build and Deploy has been ticked for SFGracefulShutdownApp (Service Fabric project).  Call the following endpoint to test API call: http://localhost:19081/SFGracefulShutdownApp/MyService.Api/api/values or http://localhost:19081/SFGracefulShutdownApp/MyService.Api/api/values?delay=5 to simulate a long running request (this example says delay by 5 seconds before completing).

To simulate a graceful shutdown, use the following powershell command (ref https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-testability-actions):

```
$connection = "localhost:19000"

Connect-ServiceFabricCluster $connection

$OperationId = New-Guid

Start-ServiceFabricPartitionRestart -OperationId $OperationId -RestartPartitionMode AllReplicasOrInstances -ServiceName "fabric:/SFGracefulShutdownApp/MyService.Api" -PartitionKindSingleton
```

## References
https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-services-lifecycle
https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-testability-actions
https://www.youtube.com/watch?v=h74NPW-oaBg&list=PL9XzOCngAkqs0Q8ZRdafnSYExKQurZrBY&index=32
